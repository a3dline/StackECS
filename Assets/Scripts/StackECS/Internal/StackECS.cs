using System.Collections.Generic;
using StackECS.Pools;

namespace StackECS
{
    internal class StackEcs : IEcs
    {
        private readonly Dictionary<BitMask, Archetype> _archetypes = new();
        private readonly List<Archetype> _filteredArchetypes;
        private readonly Stack<uint> _freeEntities = new();
        private readonly StackEcsParameters _parameters;

        private IComponentPool[] _componentPools;
        private uint _lastEntityId;
        private int _lastTypeIndex;
        private int[] _typeIndexes;

        public StackEcs(StackEcsParameters parameters = null)
        {
            _parameters = parameters ?? new StackEcsParameters();
            _typeIndexes = new int[_parameters.InitialTypeCapacity];
            HeapPool = new HeapPool(_parameters.InitialTypeCapacity);
            _filteredArchetypes = new List<Archetype>(_parameters.InitialTypeCapacity);
            _componentPools = new IComponentPool[_parameters.InitialTypeCapacity];
        }

        internal HeapPool HeapPool { get; }

        public int EntityCount => (int)_lastEntityId - _freeEntities.Count;

        public Entity CreateEntity()
        {
            var archetype = GetArchetype(new BitMask(HeapPool));
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

            return new EntityEnumerator(_filteredArchetypes, HeapPool);
        }

        public Archetype GetArchetype(BitMask mask)
        {
            if (_archetypes.TryGetValue(mask, out var pool)) return pool;

            pool = new Archetype(mask, _parameters.InitialDataCapacity);
            _archetypes.Add(mask, pool);
            return pool;
        }

        public void RemoveArchetype(Archetype archetype)
        {
            if (archetype.Mask.GetHashCode() == 0) return; // Avoid removing the empty archetype
            _archetypes.Remove(archetype.Mask);
        }

        public int GetTypeIndex<T>()
        {
            var typeIdIndex = TypeId<T>.Id;
            if (_typeIndexes[typeIdIndex] == 0)
            {
                ArrayUtilities.ResizeArray(ref _typeIndexes, typeIdIndex + 1);
                _typeIndexes[typeIdIndex] = ++_lastTypeIndex;
            }

            return _typeIndexes[typeIdIndex];
        }

        private ComponentPool<T> GetOrCreateComponentPool<T>(int typeIndex) where T : unmanaged
        {
            ArrayUtilities.ResizeArray(ref _componentPools, typeIndex + 1);

            if (_componentPools[typeIndex] == null)
            {
                var pool = new ComponentPool<T>(_parameters.InitialDataCapacity);
                _componentPools[typeIndex] = pool;
                return pool;
            }

            return (ComponentPool<T>)_componentPools[typeIndex];
        }
    }
}