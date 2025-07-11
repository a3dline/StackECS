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
        private readonly EntityEnumeratorEntryArrayPool _entityEnumeratorEntryArrayPool;
        private int _index;
        private readonly EntityEnumeratorEntry[] _entities;
        private readonly int _entitiesCount;

        public EntityEnumerator(IReadOnlyList<Archetype> archetypes,
                                EntityEnumeratorEntryArrayPool entityEnumeratorEntryArrayPool)
        {
            _entityEnumeratorEntryArrayPool = entityEnumeratorEntryArrayPool;
            _entities = entityEnumeratorEntryArrayPool.Rent();

            _entitiesCount = 0;
            var archetypeLength = archetypes.Count;
            for (var i = 0; i < archetypeLength; i++)
            {
                var archetype = archetypes[i];
                var entities = archetype.Span;
                for (var j = 0; j < archetype.EntityCount; j++)
                    _entities[_entitiesCount++] = new EntityEnumeratorEntry(entities[j], archetype);
            }

            _index = -1;
        }

        public bool MoveNext()
        {
            if (++_index < _entitiesCount) return true;
            _entityEnumeratorEntryArrayPool.Return(_entities);
            return false;
        }

        public EntityEnumeratorEntry Current => _entities[_index];

        public EntityEnumerator GetEnumerator()
        {
            return this;
        }
    }
}