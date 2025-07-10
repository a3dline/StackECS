using System.Collections.Generic;

namespace StackECS
{
    internal class StackEcs : IEcs
    {
        private readonly Dictionary<int, Archetype> _archetypes = new();
        private readonly List<Archetype> _filteredArchetypes;
        private readonly Stack<uint> _freeEntities = new();
        private readonly StackEcsParameters _parameters;
        private readonly SpanStorage<int> _spanStorageInt;
        private readonly SpanStorage<uint> _spanStorageUint;
        private readonly EntityEnumeratorEntryArrayPool _entityEnumeratorEntryArrayPool;

        private IComponentPool[] _componentPools;
        private uint _lastEntityId;
        private int _lastTypeIndex;
        private int[] _typeIndexes;

        public StackEcs(StackEcsParameters parameters = null)
        {
            _parameters = parameters ?? new StackEcsParameters();
            _typeIndexes = new int[_parameters.ComponentTypeMaxCount];
            
            _filteredArchetypes = new List<Archetype>(_parameters.ComponentTypeMaxCount);
            _componentPools = new IComponentPool[_parameters.ComponentTypeMaxCount];

            SpanStorageUlong = new SpanStorage<ulong>(_parameters.LongMaskSizeFromComponentTypeCount);
            _spanStorageInt = new SpanStorage<int>(_parameters.EntityMaxCount);
            _spanStorageUint = new SpanStorage<uint>(_parameters.EntityMaxCount);
            
            _entityEnumeratorEntryArrayPool = new EntityEnumeratorEntryArrayPool(_parameters.EntityMaxCount);
        }

        internal SpanStorage<ulong> SpanStorageUlong { get; }
        internal EntityEnumeratorEntryArrayPool EntityEnumeratorEntryArrayPool => _entityEnumeratorEntryArrayPool;

        public int EntityCount => (int)_lastEntityId - _freeEntities.Count;

        public Entity CreateEntity()
        {
            var archetype = GetArchetype(new BitMask(SpanStorageUlong));
            var entity = _freeEntities.Count > 0 ? _freeEntities.Pop() : _lastEntityId++;
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

            return new EntityEnumerator(_filteredArchetypes, _entityEnumeratorEntryArrayPool);
        }

        public Archetype GetArchetype(BitMask mask)
        {
            var hashCode = mask.GetHashCode();
            if (_archetypes.TryGetValue(hashCode, out var archetype)) return archetype;

            archetype = new Archetype(mask, _spanStorageUint, _spanStorageInt);
            _archetypes.Add(hashCode, archetype);
            return archetype;
        }

        public void RemoveArchetype(Archetype archetype)
        {
            var hashCode = archetype.Mask.GetHashCode();
            if (hashCode == 0) return; // Avoid removing the empty archetype

            _archetypes.Remove(hashCode);
            archetype.Release();
        }

        public int GetTypeIndex<T>()
        {
            var typeIdIndex = TypeId<T>.Id;
            if (_typeIndexes[typeIdIndex] == 0)
            {
                _typeIndexes[typeIdIndex] = ++_lastTypeIndex;
            }

            return _typeIndexes[typeIdIndex];
        }

        private ComponentPool<T> GetOrCreateComponentPool<T>(int typeIndex) where T : unmanaged
        {
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