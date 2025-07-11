using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using StackECS;
using UnityEngine.TestTools;

namespace StackEcs.IntegrationTests
{
    // Run debugger to see allocation result per frame
    public class StackEcsAllocationTests
    {
        [UnityTest]
        public IEnumerator TestGetComponent()
        {
            const int entityCount = 1024;
            const int testFrameCount = 1000;

            var client = new StackEcsClient();
            client.AddSystem(new ModifyEntitySystem(entityCount));
            client.Init();

            var frameLeft = testFrameCount;
            while (frameLeft-- > 0)
            {
                client.Update();
                yield return null;
            }
        }

        [UnityTest]
        public IEnumerator ChangeArchetype()
        {
            const int entityCount = 1024;
            const int testFrameCount = 1000;

            var client = new StackEcsClient();
            client.AddSystem(new ChangeArchetypeSystem(entityCount));
            client.Init();

            var frameLeft = testFrameCount;
            while (frameLeft-- > 0)
            {
                client.Update();
                yield return null;
            }
        }
    }

    internal class ChangeArchetypeSystem : IEcsInitSystem, IEcsUpdateSystem
    {
        private readonly int _entityCount;
        private bool _toAdd;

        public ChangeArchetypeSystem(int entityCount)
        {
            _entityCount = entityCount;
        }

        public void Init(IEcs ecs)
        {
            for (var i = 0; i < _entityCount; i++)
            {
                ecs.CreateEntity();
            }
        }
        public void Update(IEcs ecs)
        {
            _toAdd = !_toAdd;

            if (_toAdd)
            {
                foreach (var entity in ecs.Query.Exclude<PositionComponent>())
                {
                    entity.AddComponent<PositionComponent>();
                }    
            }
            else
            {
                foreach (var entity in ecs.Query.Include<PositionComponent>())
                {
                    entity.RemoveComponent<PositionComponent>();
                }
            }
        }
    }

    internal class ModifyEntitySystem : IEcsUpdateSystem, IEcsInitSystem
    {
        private readonly int _entityCount;

        public ModifyEntitySystem(int entityCount)
        {
            _entityCount = entityCount;
        }

        public void Init(IEcs ecs)
        {
            for (var i = 0; i < _entityCount; i++)
            {
                var entity = ecs.CreateEntity();
                entity.AddComponent<PositionComponent>();
            }
        }

        public void Update(IEcs ecs)
        {
            foreach (var entity in ecs.Query.Include<PositionComponent>())
            {
                ref var position = ref entity.GetComponent<PositionComponent>();
            }
        }
    }
}