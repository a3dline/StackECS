using System;

namespace StackECS
{
    public ref struct Entity
    {
        private readonly uint _id;
        private readonly StackEcs _ecs;

        private Archetype _archetype;
        private bool _removed;

        internal int Index => (int)_id;

        internal Entity(uint id, Archetype archetype, StackEcs ecs)
        {
            _id = id;
            _archetype = archetype;
            _ecs = ecs;
            _removed = false;
        }

        public ref T AddComponent<T>() where T : unmanaged
        {
            ThrowIfRemoved();

            var typeId = _ecs.GetTypeIndex<T>();
            if (_archetype.Mask[typeId]) throw new InvalidOperationException("Component already exists");

            var newMask = _archetype.Mask.CopyWithBit(typeId);
            _archetype.RemoveEntity(_id);
            if (_archetype.EntityCount == 0) _ecs.RemoveArchetype(_archetype);

            _archetype = _ecs.GetArchetype(newMask);
            _archetype.AddEntity(_id);
            return ref _ecs.AddComponent<T>(_id, typeId);
        }

        public void RemoveComponent<T>() where T : unmanaged
        {
            ThrowIfRemoved();

            var typeId = _ecs.GetTypeIndex<T>();
            if (!_archetype.Mask[typeId]) throw new InvalidOperationException("Component does not exist");

            var newMask = _archetype.Mask.CopyWithoutBit(typeId);
            _archetype.RemoveEntity(_id);
            if (_archetype.EntityCount == 0) _ecs.RemoveArchetype(_archetype);

            _archetype = _ecs.GetArchetype(newMask);
            _archetype.AddEntity(_id);
        }

        public ref T GetComponent<T>() where T : unmanaged
        {
            ThrowIfRemoved();

            var typeId = _ecs.GetTypeIndex<T>();
            if (!_archetype.Mask[typeId]) throw new InvalidOperationException("Component does not exist");

            return ref _ecs.GetComponent<T>(_id, typeId);
        }

        public bool HasComponent<T>() where T : unmanaged
        {
            ThrowIfRemoved();

            var typeId = _ecs.GetTypeIndex<T>();
            return _archetype.Mask[typeId];
        }

        public void Delete()
        {
            ThrowIfRemoved();

            _archetype.RemoveEntity(_id);
            if (_archetype.EntityCount == 0) _ecs.RemoveArchetype(_archetype);
            _ecs.RemoveEntity(_id);
            _removed = true;
        }

        private void ThrowIfRemoved()
        {
            if (_removed) throw new InvalidOperationException("Entity was removed");
        }
    }
}