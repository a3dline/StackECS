using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using StackECS;
using UnityEngine;
using UnityEngine.TestTools;

namespace StackEcs.IntegrationTests
{
    public class StackEcsIntegrationTest1
    {
        [UnityTest]
        public IEnumerator Test()
        {
            var client = new StackEcsClient();

            var addMovementSystem = new AddMovementSystem();

            client.AddSystem(new CreateEntitySystem(100));
            client.AddSystem(addMovementSystem);
            client.AddSystem(new UpdatePositionSystem());

            client.Init();
            client.Update();

            yield return null;
            addMovementSystem.ChangePosition(3, new Vector2(10, 0));
            addMovementSystem.ChangePosition(7, new Vector2(5, 0));

            client.Update();
            client.Update();
            yield return null;

            var result = new Vector2[10];
            
            
            foreach (var entity in client.Ecs.Query
                                         .Include<IndexEntityComponent>()
                                         .Include<PositionComponent>())
            {
                var index = entity.GetComponent<IndexEntityComponent>().Index;
                var position = entity.GetComponent<PositionComponent>().Position;
                result[index] = position;
            }
            
            Assert.AreEqual(new Vector2(10, 0), result[3]);
            Assert.AreEqual(new Vector2(5, 0), result[7]);
        }
    }

    internal class CreateEntitySystem : IEcsInitSystem
    {
        private readonly int _count;

        public CreateEntitySystem(int count)
        {
            _count = count;
        }

        public void Init(IEcs ecs)
        {
            for (var i = 0; i < _count; i++)
            {
                var entity = ecs.CreateEntity();
                entity.AddComponent<PositionComponent>();
                entity.AddComponent<IndexEntityComponent>().Index = i;
            }
        }
    }

    internal class UpdatePositionSystem : IEcsUpdateSystem
    {
        public void Update(IEcs ecs)
        {
            foreach (var entity in ecs.Query
                                      .Include<PositionComponent>()
                                      .Include<MovementComponent>())
            {
                ref var position = ref entity.GetComponent<PositionComponent>();
                var movement = entity.GetComponent<MovementComponent>();
                position.Position += movement.Velocity;
                entity.RemoveComponent<MovementComponent>();
            }
        }
    }

    internal class AddMovementSystem : IEcsUpdateSystem
    {
        private readonly Queue<(int, Vector2)> _movementQueue = new();

        public void Update(IEcs ecs)
        {
            if (_movementQueue.Count == 0) return;
            var intent = _movementQueue.Dequeue();
            foreach (var entity in ecs.Query
                                      .Include<PositionComponent>()
                                      .Include<IndexEntityComponent>())
                if (entity.GetComponent<IndexEntityComponent>().Index == intent.Item1)
                    entity.AddComponent<MovementComponent>().Velocity = intent.Item2;
        }

        public void ChangePosition(int index, Vector2 velocity)
        {
            _movementQueue.Enqueue((index, velocity));
        }
    }

    internal struct IndexEntityComponent
    {
        public int Index;
    }

    internal struct PositionComponent
    {
        public Vector2 Position;
    }

    internal struct MovementComponent
    {
        public Vector2 Velocity;
    }
}