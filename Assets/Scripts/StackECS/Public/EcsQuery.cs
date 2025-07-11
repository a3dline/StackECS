using System;

namespace StackECS
{
    public class EcsQuery
    {
        private readonly StackEcs _ecs;
        private BitMask _includeMask;
        private BitMask _excludeMask;
        private EntityEnumerator _numerator;
        private bool _released;

        internal EcsQuery(StackEcs ecs)
        {
            _ecs = ecs;
            _includeMask = new BitMask(ecs.SpanStorageUlong);
            _excludeMask = new BitMask(ecs.SpanStorageUlong);
            _numerator = default;
            _released = false;
        }

        public bool MoveNext()
        {
            var result = _numerator.MoveNext();
            if (!result)
            {
                _includeMask.Release();
                _excludeMask.Release();
                _released = true;
            }

            return result;
        }

        public Entity Current
        {
            get
            {
                var entity = _numerator.Current;
                return new Entity(entity.Id, entity.Archetype, _ecs);
            }
        }

        public EcsQuery GetEnumerator()
        {
            if (_released)
            {
                throw new InvalidOperationException("Query has already been enumerated. Please create a new query.");
            }
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