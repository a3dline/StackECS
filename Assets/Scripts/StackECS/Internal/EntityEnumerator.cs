using System.Collections.Generic;

namespace StackECS
{
    internal static class Pool
    {
        private static Queue<EntityEnumerator.Entry[]> _pool = new(2); 
        
        public static EntityEnumerator.Entry[] Rent(int initialCapacity)
        {
            if(_pool.Count == 0)
            {
                var array = new EntityEnumerator.Entry[initialCapacity];
                return array;
            }
            else
            {
                return _pool.Dequeue();
            }
        }
        
        public static void Return(EntityEnumerator.Entry[] array)
        {
            _pool.Enqueue(array);
        }
    }
    
    internal struct EntityEnumerator
    {
        public readonly struct Entry
        {
            public readonly uint Id;
            public readonly Archetype Archetype;

            public Entry(uint id, Archetype archetype)
            {
                Id = id;
                Archetype = archetype;
            }
        }

        private int _index;
        private Entry[] _entities;
        private int _entityCount;

        public EntityEnumerator(Archetype[] archetypes, int archetypeLength, int capacity)
        {
            _entities = Pool.Rent(capacity);

            var length = 0;
            for (int i = 0; i < archetypeLength; i++)
            {
                var archetype = archetypes[i];
                for (int j = 0; j < archetype.EntityCount; j++)
                {
                    _entities[length++] = new Entry(archetype[j], archetype);
                } 
            }
            _index = -1;
            _entityCount = length;
        }
        
        public bool MoveNext()
        {
            if (++_index < _entityCount) return true;
            Pool.Return(_entities);
            _entities = null;
            return false;
        }

        public ref Entry Current => ref _entities[_index];

        public EntityEnumerator GetEnumerator()
        {
            return this;
        }
    }
}