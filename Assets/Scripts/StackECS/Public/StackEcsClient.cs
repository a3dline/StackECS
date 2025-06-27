using System;
using System.Collections.Generic;

namespace StackECS
{
    public class StackEcsClient : IDisposable
    {
        private readonly int _capacity;
        private readonly StackEcs _ecs;
        private readonly Archetype[] _emptyArchetype = new Archetype [1];
        private readonly List<IEcsInitSystem> _initSystems = new();
        private readonly List<IEcsUpdateSystem> _updateSystems = new();

        public StackEcsClient(int capacity)
        {
            _capacity = capacity;
            _ecs = new StackEcs(capacity);
        }

        public IEcs Ecs => _ecs;
        public int EntityCount => _ecs.EntityCount;

        public void Dispose()
        {
            _ecs.Dispose();
        }

        public void AddSystem<T>(T system) where T : IEcsSystem
        {
            if (system is IEcsInitSystem initSystem) _initSystems.Add(initSystem);
            if (system is IEcsUpdateSystem updateSystem) _updateSystems.Add(updateSystem);
        }

        public void Init()
        {
            _emptyArchetype[0] = _ecs.GetArchetype(0);
            foreach (var system in _initSystems) system.Init(_ecs);
        }

        public void Update()
        {
            foreach (var system in _updateSystems) system.Update(_ecs);

            var emptyEntityEnumerator = new EntityEnumerator(_emptyArchetype, 1, _capacity);
            foreach (var entity in emptyEntityEnumerator)
            {
                var entityInstance = new Entity(entity.Id, entity.Archetype, _ecs);
                entityInstance.Delete();
            }
        }
    }
}