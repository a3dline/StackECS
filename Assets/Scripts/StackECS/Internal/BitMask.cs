using System;
using System.Collections.Generic;
using StackECS.Pools;

namespace StackECS
{
    internal struct BitMask : IEquatable<BitMask>
    {
        private readonly IUlongListPool _ulongListPool;
        private readonly List<ulong> _data;
        private int _hash;

        public BitMask(IUlongListPool ulongListPool)
        {
            _ulongListPool = ulongListPool;
            _data = ulongListPool.Rent();
            _hash = 0;
        }

        private BitMask(IUlongListPool ulongListPool, List<ulong> data)
        {
            _ulongListPool = ulongListPool;
            _data = data;
            _hash = GetHash(data);
        }

        public bool this[int index]
        {
            get
            {
                var ulongIndex = index / 64;
                if (ulongIndex >= _data.Count)
                    return false;
                return (_data[ulongIndex] & (1UL << (index % 64))) != 0;
            }
            set
            {
                var ulongIndex = index / 64;
                EnsureCapacity(ulongIndex + 1);
                if (value)
                    _data[ulongIndex] |= 1UL << (index % 64);
                else
                    _data[ulongIndex] &= ~(1UL << (index % 64));
                _hash = GetHash(_data);
            }
        }

        public bool Equals(BitMask other)
        {
            if (_data.Count != other._data.Count) return false;
            for (var i = 0; i < _data.Count; i++)
                if (_data[i] != other._data[i])
                    return false;
            return true;
        }

        public void Release()
        {
            _ulongListPool.Return(_data);
        }

        private void EnsureCapacity(int longIndex)
        {
            while (_data.Count < longIndex) _data.Add(0UL);
        }

        public BitMask CopyWithBit(int index)
        {
            var copyData = _ulongListPool.Rent();
            copyData.AddRange(_data);
            var copy = new BitMask(_ulongListPool, copyData) { [index] = true };
            return copy;
        }

        public BitMask CopyWithoutBit(int index)
        {
            var copyData = _ulongListPool.Rent();
            copyData.AddRange(_data);
            var copy = new BitMask(_ulongListPool, copyData) { [index] = false };
            return copy;
        }

        public BitMask Copy()
        {
            var copyData = _ulongListPool.Rent();
            copyData.AddRange(_data);
            return new BitMask(_ulongListPool, copyData);
        }

        public override bool Equals(object obj)
        {
            return obj is BitMask other && Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _hash;
        }

        public static bool operator ==(BitMask a, BitMask b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(BitMask a, BitMask b)
        {
            return !a.Equals(b);
        }

        public static BitMask operator +(BitMask a, BitMask b)
        {
            var len = Math.Max(a._data.Count, b._data.Count);
            var result = new BitMask(a._ulongListPool);
            for (var i = 0; i < len; i++)
            {
                var av = i < a._data.Count ? a._data[i] : 0UL;
                var bv = i < b._data.Count ? b._data[i] : 0UL;
                result._data.Add(av | bv);
            }

            return result;
        }

        public static BitMask operator -(BitMask a, BitMask b)
        {
            var len = Math.Max(a._data.Count, b._data.Count);
            var result = new BitMask(a._ulongListPool);
            for (var i = 0; i < len; i++)
            {
                var av = i < a._data.Count ? a._data[i] : 0UL;
                var bv = i < b._data.Count ? b._data[i] : 0UL;
                result._data.Add(av & ~bv);
            }

            return result;
        }

        private static int GetHash(List<ulong> data)
        {
            var hash = 17;
            for (var i = 0; i < data.Count; i++) hash = hash * 31 + data[i].GetHashCode();
            return hash;
        }
    }
}