namespace StackECS
{
    internal class Archetype
    {
        private uint[] _entities;
        private int[] _indexes;

        public Archetype(BitMask mask, int initialCapacity)
        {
            Mask = mask;
            _entities = new uint[initialCapacity];
            _indexes = new int[initialCapacity];
        }

        public int EntityCount { get; private set; }

        public uint this[int index] => _entities[index];
        public BitMask Mask { get; }

        public void AddEntity(uint id)
        {
            ArrayUtilities.ResizeArray(ref _indexes, (int)id);
            ArrayUtilities.ResizeArray(ref _entities, EntityCount);

            _entities[EntityCount] = id;
            _indexes[id] = EntityCount;
            EntityCount++;
        }

        public void RemoveEntity(uint id)
        {
            var index = _indexes[id];
            var lastEntityIndex = EntityCount - 1;

            // swap back
            if (index != EntityCount - 1)
            {
                var lastEntity = _entities[lastEntityIndex];
                _entities[index] = lastEntity;
                _indexes[lastEntity] = index;
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
            return include + Mask == Mask && Mask - exclude == Mask;
        }
    }
}