using Unity.Profiling;

namespace StackECS
{
    public struct EcsQuery
    {
        private readonly StackEcs _ecs;
        private BitArray64 _includeMask;
        private BitArray64 _excludeMask;
        private EntityEnumerator _numerator;

        internal EcsQuery(StackEcs ecs)
        {
            _ecs = ecs;
            _includeMask = new BitArray64();
            _excludeMask = new BitArray64();
            _numerator = default;
        }

        public bool MoveNext()
        {
            return _numerator.MoveNext();
        }

        public Entity Current
        {
            get
            {
                ref var entity = ref _numerator.Current;
                return new Entity(entity.Id, entity.Archetype, _ecs);
            }
        }

        public EcsQuery GetEnumerator()
        {
            _numerator = _ecs.GetEntityEnumerator(_includeMask, _excludeMask);
            return this;
        }

        public EcsQuery Include<T>() where T : unmanaged
        {
            var typeId = _ecs.GetTypeIndex<T>();
            _includeMask[typeId] = true;
            return this;
        }

        public EcsQuery Exclude<T>() where T : unmanaged
        {
            var typeId = _ecs.GetTypeIndex<T>();
            _excludeMask[typeId] = true;
            return this;
        }
    }
}