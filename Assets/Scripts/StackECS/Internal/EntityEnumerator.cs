using System.Collections.Generic;
using StackECS.Pools;

namespace StackECS
{
    internal readonly struct EntityEnumeratorEntry
    {
        public readonly uint Id;
        public readonly Archetype Archetype;

        public EntityEnumeratorEntry(uint id, Archetype archetype)
        {
            Id = id;
            Archetype = archetype;
        }
    }

    internal struct EntityEnumerator
    {
        private readonly IEntityEnumeratorEntryListPool _pool;

        private int _index;
        private List<EntityEnumeratorEntry> _entities;

        public EntityEnumerator(IReadOnlyList<Archetype> archetypes, IEntityEnumeratorEntryListPool pool)
        {
            _pool = pool;
            _entities = pool.Rent();

            var archetypeLength = archetypes.Count;
            for (var i = 0; i < archetypeLength; i++)
            {
                var archetype = archetypes[i];
                for (var j = 0; j < archetype.EntityCount; j++)
                    _entities.Add(new EntityEnumeratorEntry(archetype[j], archetype));
            }

            _index = -1;
        }

        public bool MoveNext()
        {
            if (++_index < _entities.Count) return true;
            _pool.Return(_entities);
            _entities = null;
            return false;
        }

        public EntityEnumeratorEntry Current => _entities[_index];

        public EntityEnumerator GetEnumerator()
        {
            return this;
        }
    }
}