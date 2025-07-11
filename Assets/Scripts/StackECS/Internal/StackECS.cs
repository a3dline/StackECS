using System;
using System.Collections.Generic;
using StackECS.Pools;

namespace StackECS
{
    internal class StackEcs : IEcs
    {
        private readonly Dictionary<int, Archetype> _archetypes = new();

        private readonly IComponentPool[] _componentPools;
        private readonly List<Archetype> _filteredArchetypes;
        private readonly Stack<uint> _freeEntities = new();
        private readonly StackEcsParameters _parameters;
        private readonly SpanStorage<int> _spanStorageInt;
        private readonly SpanStorage<uint> _spanStorageUint;
        private uint _lastEntityId;
        private int _lastTypeIndex;
        private int[] _typeIndexes = new int[64];

        public StackEcs(StackEcsParameters parameters = null)
        {
            _parameters = parameters ?? new StackEcsParameters();

            _filteredArchetypes = new List<Archetype>(_parameters.ComponentTypeMaxCount);
            _componentPools = new IComponentPool[_parameters.ComponentTypeMaxCount];

            SpanStorageUlong = new SpanStorage<ulong>(_parameters.LongMaskSizeFromComponentTypeCount);
            _spanStorageInt = new SpanStorage<int>(_parameters.EntityMaxCount);
            _spanStorageUint = new SpanStorage<uint>(_parameters.EntityMaxCount);

            EntityEnumeratorEntryArrayPool = new EntityEnumeratorEntryArrayPool(_parameters.EntityMaxCount);
        }

        internal SpanStorage<ulong> SpanStorageUlong { get; }
        internal EntityEnumeratorEntryArrayPool EntityEnumeratorEntryArrayPool { get; }
        internal int ArchetypeCount => _archetypes.Count;
        public int EntityCount => (int)_lastEntityId - _freeEntities.Count;

        public Entity CreateEntity()
        {
            var entity = _freeEntities.Count > 0 ? _freeEntities.Pop() : _lastEntityId++;

            if (_lastEntityId > _parameters.EntityMaxCount)
                throw new
                    InvalidOperationException("Entity limit reached. Increase EntityMaxCount in StackEcsParameters.");

            var archetype = GetArchetype(new BitMask(SpanStorageUlong));
            archetype.AddEntity(entity);
            return new Entity(entity, archetype, this);
        }

        public EcsQuery Query => new(this);

        public void RemoveEntity(uint entity)
        {
            _freeEntities.Push(entity);
        }

        public ref T AddComponent<T>(uint entity, int typeIndex) where T : unmanaged
        {
            var pool = GetOrCreateComponentPool<T>(typeIndex);
            return ref pool.Add(entity);
        }

        public ref T GetComponent<T>(uint entity, int typeIndex) where T : unmanaged
        {
            var pool = GetOrCreateComponentPool<T>(typeIndex);
            return ref pool.Get(entity);
        }

        public EntityEnumerator GetEntityEnumerator(BitMask include, BitMask exclude)
        {
            _filteredArchetypes.Clear();
            var values = _archetypes.Values;
            foreach (var archetype in values)
                if (archetype.Match(include, exclude))
                    _filteredArchetypes.Add(archetype);

            return new EntityEnumerator(_filteredArchetypes, EntityEnumeratorEntryArrayPool);
        }

        public Archetype GetArchetype(BitMask mask)
        {
            var hashCode = mask.GetHashCode();
            if (_archetypes.TryGetValue(hashCode, out var archetype))
            {
                mask.Release();
                return archetype;
            }

            archetype = new Archetype(mask, _spanStorageUint, _spanStorageInt);
            _archetypes.Add(hashCode, archetype);
            return archetype;
        }

        public void RemoveArchetype(Archetype archetype)
        {
            _archetypes.Remove(archetype.Mask.GetHashCode());
            archetype.Release();
        }

        public int GetTypeIndex<T>()
        {
            var typeIdIndex = TypeId<T>.Id;
            if (_typeIndexes.Length <= typeIdIndex)
            {
                // Resize the array if necessary
                var newSize = Math.Max(_typeIndexes.Length * 2, typeIdIndex + 1);
                Array.Resize(ref _typeIndexes, newSize);
            }

            if (_typeIndexes[typeIdIndex] == 0) // Zero is reserved for the empty type index
                _typeIndexes[typeIdIndex] = ++_lastTypeIndex;

            if (_lastTypeIndex > _parameters.ComponentTypeMaxCount)
                throw new
                    InvalidOperationException("Component type limit reached. Increase ComponentTypeMaxCount in StackEcsParameters.");

            return _typeIndexes[typeIdIndex];
        }

        private ComponentPool<T> GetOrCreateComponentPool<T>(int typeIndex) where T : unmanaged
        {
            typeIndex -= 1; // Zero is reserved for the empty type index
            if (_componentPools[typeIndex] == null)
            {
                var pool = new ComponentPool<T>(_parameters.EntityMaxCount);
                _componentPools[typeIndex] = pool;
                return pool;
            }

            return (ComponentPool<T>)_componentPools[typeIndex];
        }
    }
}