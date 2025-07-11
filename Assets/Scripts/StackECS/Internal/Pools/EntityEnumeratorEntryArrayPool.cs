using System.Collections.Generic;

namespace StackECS.Pools
{
    internal class EntityEnumeratorEntryArrayPool
    {
        private readonly int _arraySize;
        private readonly Stack<EntityEnumeratorEntry[]> _pool = new();

        public EntityEnumeratorEntryArrayPool(int arraySize)
        {
            _arraySize = arraySize;
        }

        public EntityEnumeratorEntry[] Rent()
        {
            if (_pool.Count > 0) return _pool.Pop();
            return new EntityEnumeratorEntry[_arraySize];
        }

        public void Return(EntityEnumeratorEntry[] array)
        {
            _pool.Push(array);
        }
    }
}