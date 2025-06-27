using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Profiling;
using UnityEngine.Pool;

namespace StackECS
{
    internal class StackEcs : IEcs, IDisposable
    {
        private readonly Dictionary<ulong, Archetype> _archetypes = new();
        private readonly IComponentPool[] _componentPools = new IComponentPool[64];
        private readonly Queue<uint> _freeEntities = new();
        private readonly int _capacity;
        private uint _lastEntityId;
        private int _lastTypeIndex;
        private int[] _typeIndexes = new int[64];

        public StackEcs(int capacity)
        {
            _capacity = capacity;
        }

        public int EntityCount => (int)_lastEntityId - _freeEntities.Count;

        public void Dispose() { }

        public Entity CreateEntity()
        {
            var archetype = GetArchetype(0);
            var entity = _freeEntities.Count > 0 ? _freeEntities.Dequeue() : _lastEntityId++;
            archetype.AddEntity(entity);
            return new Entity(entity, archetype, this);
        }

        public EcsQuery Query => new(this);

        public void RemoveEntity(uint entity)
        {
            _freeEntities.Enqueue(entity);
        }

        public ref T AddComponent<T>(uint entity, int typeIndex) where T : unmanaged
        {
            var pool = GetComponentPool<T>(typeIndex);
            return ref pool.Add(entity);
        }

        public ref T GetComponent<T>(uint entity, int typeIndex) where T : unmanaged
        {
            var pool = GetComponentPool<T>(typeIndex);
            return ref pool.Get(entity);
        }
        
        private Archetype[] _filteredArchetypes = new Archetype[64];

        public EntityEnumerator GetEntityEnumerator(BitArray64 include, BitArray64 exclude)
        {
            var values = _archetypes.Values;
            var length = 0;
            foreach (var archetype in values)
                if (archetype.Match(include, exclude))
                {
                    _filteredArchetypes[length++] = archetype;
                }

            return new EntityEnumerator(_filteredArchetypes, length, _capacity);
            
        }

        public Archetype GetArchetype(ulong hash)
        {
            if (_archetypes.TryGetValue(hash, out var pool)) return pool;

            pool = new Archetype(hash, _capacity);
            _archetypes.Add(hash, pool);
            return pool;
        }

        public int GetTypeIndex<T>()
        {
            var typeIdIndex = TypeId<T>.Id;
            if (_typeIndexes[typeIdIndex] == 0) _typeIndexes[typeIdIndex] = ++_lastTypeIndex;
            return _typeIndexes[typeIdIndex];
        }

        private ComponentPool<T> GetComponentPool<T>(int typeIndex) where T : unmanaged
        {
            if (_componentPools[typeIndex] == null)
            {
                var pool = new ComponentPool<T>(_capacity);
                _componentPools[typeIndex] = pool;
                return pool;
            }

            return (ComponentPool<T>)_componentPools[typeIndex];
        }
    }
}