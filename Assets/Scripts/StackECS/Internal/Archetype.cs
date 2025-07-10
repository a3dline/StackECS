using System;

namespace StackECS
{
    internal class Archetype
    {
        private readonly Slot _entities;
        private readonly Slot _indexes;
        private readonly SpanStorage<int> _intStorage;
        private readonly SpanStorage<uint> _uintStorage;

        public Archetype(BitMask mask, SpanStorage<uint> uintStorage, SpanStorage<int> intStorage)
        {
            _uintStorage = uintStorage;
            _intStorage = intStorage;
            Mask = mask;
            _entities = uintStorage.ReserveSlot();
            _indexes = intStorage.ReserveSlot();
        }

        public int EntityCount { get; private set; }

        public Span<uint> Span => _uintStorage.GetSpan(_entities);
        public BitMask Mask { get; }

        public void AddEntity(uint id)
        {
            _uintStorage.GetSpan(_entities)[EntityCount] = id;
            _intStorage.GetSpan(_indexes)[(int)id] = EntityCount;
            EntityCount++;
        }

        public void RemoveEntity(uint id)
        {
            var entitySpan = _uintStorage.GetSpan(_entities);
            var indexSpan = _intStorage.GetSpan(_indexes);

            var index = indexSpan[(int)id];
            var lastEntityIndex = EntityCount - 1;

            // swap back
            if (index != EntityCount - 1)
            {
                var lastEntity = entitySpan[lastEntityIndex];
                entitySpan[index] = lastEntity;
                indexSpan[(int)lastEntity] = index;
            }

            EntityCount--;
        }

        public BitMask GetMaskWithIndex(int index)
        {
            return Mask.CopyWithBit(index);
        }

        public BitMask GetMaskWithoutIndex(int index)
        {
            return Mask.CopyWithoutBit(index);
        }

        public bool Match(BitMask include, BitMask exclude)
        {
            var summ = include + Mask;
            var delta = Mask - exclude;

            var result = summ == Mask && delta == Mask;
            summ.Release();
            delta.Release();
            return result;
        }

        public void Release()
        {
            _uintStorage.Release(_entities);
            Mask.Release();
        }
    }
}