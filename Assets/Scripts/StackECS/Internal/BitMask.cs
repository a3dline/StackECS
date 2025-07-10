using System;

namespace StackECS
{
    internal struct BitMask
    {
        private readonly SpanStorage<ulong> _spanStorage;
        private readonly Slot _slot;
        private int _hash;

        public BitMask(SpanStorage<ulong> spanStorage)
        {
            _spanStorage = spanStorage;
            _slot = spanStorage.ReserveSlot();
            _hash = 0;
        }

        private BitMask(SpanStorage<ulong> spanStorage, Slot slot)
        {
            _spanStorage = spanStorage;
            _slot = slot;
            _hash = GetHash(_spanStorage.GetSpan(in _slot));
        }

        public bool this[int index]
        {
            get
            {
                var data = _spanStorage.GetSpan(in _slot);
                var ulongIndex = index / 64;
                if (ulongIndex >= data.Length)
                    return false;
                return (data[ulongIndex] & (1UL << (index % 64))) != 0;
            }
            set
            {
                var data = _spanStorage.GetSpan(in _slot);
                var ulongIndex = index / 64;
                if (value)
                    data[ulongIndex] |= 1UL << (index % 64);
                else
                    data[ulongIndex] &= ~(1UL << (index % 64));
                _hash = GetHash(data);
            }
        }

        public void Release()
        {
            _spanStorage.Release(_slot);
            _hash = 0;
        }

        public BitMask CopyWithBit(int index)
        {
            var data = _spanStorage.GetSpan(in _slot);

            var copySlot = _spanStorage.ReserveSlot();
            var copyData = _spanStorage.GetSpan(in copySlot);
            data.CopyTo(copyData);

            var copyMask = new BitMask(_spanStorage, copySlot);
            copyMask[index] = true;

            return copyMask;
        }

        public BitMask CopyWithoutBit(int index)
        {
            var data = _spanStorage.GetSpan(in _slot);

            var copySlot = _spanStorage.ReserveSlot();
            var copyData = _spanStorage.GetSpan(in copySlot);
            data.CopyTo(copyData);

            var copyMask = new BitMask(_spanStorage, copySlot);
            copyMask[index] = false;

            return copyMask;
        }

        public BitMask Copy()
        {
            var data = _spanStorage.GetSpan(in _slot);

            var copySlot = _spanStorage.ReserveSlot();
            var copyData = _spanStorage.GetSpan(in copySlot);
            data.CopyTo(copyData);

            return new BitMask(_spanStorage, copySlot);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return _hash;
        }

        public static bool operator ==(in BitMask a, in BitMask b)
        {
            var aData = a._spanStorage.GetSpan(in a._slot);
            var bData = b._spanStorage.GetSpan(in b._slot);

            for (var i = 0; i < aData.Length; i++)
                if (aData[i] != bData[i])
                    return false;

            return true;
        }

        public static bool operator !=(in BitMask a, in BitMask b)
        {
            return !(a == b);
        }

        public static BitMask operator +(in BitMask a, in BitMask b)
        {
            var aData = a._spanStorage.GetSpan(in a._slot);
            var bData = b._spanStorage.GetSpan(in b._slot);
            var len = Math.Max(aData.Length, bData.Length);
            var result = new BitMask(a._spanStorage);
            var resultData = result._spanStorage.GetSpan(in result._slot);

            for (var i = 0; i < len; i++)
            {
                var av = i < aData.Length ? aData[i] : 0UL;
                var bv = i < bData.Length ? bData[i] : 0UL;
                resultData[i] = av | bv;
            }

            return result;
        }

        public static BitMask operator -(in BitMask a, in BitMask b)
        {
            var aData = a._spanStorage.GetSpan(in a._slot);
            var bData = b._spanStorage.GetSpan(in b._slot);
            var len = Math.Max(aData.Length, bData.Length);
            var result = new BitMask(a._spanStorage);
            var resultData = result._spanStorage.GetSpan(in result._slot);

            for (var i = 0; i < len; i++)
            {
                var av = i < aData.Length ? aData[i] : 0UL;
                var bv = i < bData.Length ? bData[i] : 0UL;
                resultData[i] = av & ~bv;
            }

            return result;
        }

        private static int GetHash(Span<ulong> data)
        {
            var hash = 17;
            for (var i = 0; i < data.Length; i++) hash = hash * 31 + data[i].GetHashCode();
            return hash;
        }
    }
}