using System;
using System.Collections.Generic;
using Unity.Collections;

namespace StackECS
{
    internal class Archetype
    {
        private readonly BitArray64 _bitArray;
        private readonly uint[] _entities;
        private readonly int[] _indexes;
        private int _entityCount;

        public Archetype(ulong hash, int capacity)
        {
            _bitArray = new BitArray64(hash);
            _entities = new uint[capacity];
            _indexes = new int[capacity];
        }

        public ulong Hash => _bitArray.Hash;
        
        internal int EntityCount => _entityCount;
        internal uint this[int index] => _entities[index];

        public void AddEntity(uint id)
        {
            _entities[_entityCount] = id;
            _indexes[id] = _entityCount;
            _entityCount++;
        }

        public void RemoveEntity(uint id)
        {
            var index = _indexes[id];

            var lastEntityIndex = _entityCount - 1;

            // swap back
            if (index != _entityCount - 1)
            {
                var lastEntity = _entities[lastEntityIndex];
                _entities[index] = lastEntity;
                _indexes[lastEntity] = index;
            }

            _entityCount--;
        }

        public ulong GetHashWithIndex(int index)
        {
            return _bitArray.WithBit(index);
        }

        public ulong GetHashWithoutIndex(int index)
        {
            return _bitArray.WithoutBit(index);
        }

        public bool Match(BitArray64 include, BitArray64 exclude)
        {
            return include + _bitArray == _bitArray && _bitArray - exclude == _bitArray;
        }
    }
}