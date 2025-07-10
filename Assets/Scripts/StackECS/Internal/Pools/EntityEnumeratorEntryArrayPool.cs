using System.Collections.Generic;

namespace StackECS
{
    internal class EntityEnumeratorEntryArrayPool
    {
        private readonly int _arraySize;
        private readonly Stack<EntityEnumeratorEntry[]> Pool = new();

        public EntityEnumeratorEntryArrayPool(int arraySize)
        {
            _arraySize = arraySize;
        }
        
        public EntityEnumeratorEntry[] Rent()
        {
            if (Pool.Count > 0)
            {
                return Pool.Pop();
            }
            return new EntityEnumeratorEntry[_arraySize];
        }
        
        public void Return(EntityEnumeratorEntry[] array)
        {
            Pool.Push(array);
        }
    }
}