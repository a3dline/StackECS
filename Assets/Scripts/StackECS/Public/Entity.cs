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

            var hash = _archetype.Hash;
            var typeId = _ecs.GetTypeIndex<T>();
            var newHash = _archetype.GetHashWithIndex(typeId);
            if (hash == newHash) throw new InvalidOperationException("Component already exists");

            _archetype.RemoveEntity(_id);
            _archetype = _ecs.GetArchetype(newHash);
            _archetype.AddEntity(_id);
            return ref _ecs.AddComponent<T>(_id, typeId);
        }

        public void RemoveComponent<T>() where T : unmanaged
        {
            ThrowIfRemoved();

            var hash = _archetype.Hash;
            var typeId = _ecs.GetTypeIndex<T>();
            var newHash = _archetype.GetHashWithoutIndex(typeId);
            if (hash == newHash) throw new InvalidOperationException("Component does not exist");

            _archetype.RemoveEntity(_id);
            _archetype = _ecs.GetArchetype(newHash);
            _archetype.AddEntity(_id);
        }

        public ref T GetComponent<T>() where T : unmanaged
        {
            ThrowIfRemoved();

            var typeId = _ecs.GetTypeIndex<T>();
            return ref _ecs.GetComponent<T>(_id, typeId);
        }

        public bool HasComponent<T>() where T : unmanaged
        {
            ThrowIfRemoved();

            var typeId = _ecs.GetTypeIndex<T>();
            var modifiedHash = _archetype.GetHashWithIndex(typeId);
            return _archetype.Hash == modifiedHash;
        }

        public void Delete()
        {
            ThrowIfRemoved();

            _archetype.RemoveEntity(_id);
            _ecs.RemoveEntity(_id);
            _removed = true;
        }

        private void ThrowIfRemoved()
        {
            if (_removed) throw new InvalidOperationException("Entity was removed");
        }
    }
}