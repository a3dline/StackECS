using System.Collections.Generic;

namespace StackECS
{
    public class StackEcsParameters
    {
        public int InitialDataCapacity { get; set; } = 256;
        public int InitialTypeCapacity { get; set; } = 64;
    }

    public class StackEcsClient
    {
        private readonly StackEcs _ecs;
        private readonly Archetype[] _emptyArchetype = new Archetype [1];
        private readonly List<IEcsInitSystem> _initSystems = new();
        private readonly List<IEcsUpdateSystem> _updateSystems = new();

        public StackEcsClient(StackEcsParameters parameters = null)
        {
            _ecs = new StackEcs(parameters);
        }

        public IEcs Ecs => _ecs;
        public int EntityCount => _ecs.EntityCount;

        public void AddSystem<T>(T system) where T : IEcsSystem
        {
            if (system is IEcsInitSystem initSystem) _initSystems.Add(initSystem);
            if (system is IEcsUpdateSystem updateSystem) _updateSystems.Add(updateSystem);
        }

        public void Init()
        {
            _emptyArchetype[0] = _ecs.GetArchetype(new BitMask(_ecs.HeapPool));
            foreach (var system in _initSystems) system.Init(_ecs);
        }

        public void Update()
        {
            foreach (var system in _updateSystems) system.Update(_ecs);

            var emptyEntityEnumerator = new EntityEnumerator(_emptyArchetype, _ecs.HeapPool);
            foreach (var entity in emptyEntityEnumerator)
            {
                var entityInstance = new Entity(entity.Id, entity.Archetype, _ecs);
                entityInstance.Delete();
            }
        }
    }
}