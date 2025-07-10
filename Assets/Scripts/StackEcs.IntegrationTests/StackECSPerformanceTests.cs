using System.Collections;
using System.Diagnostics;
using StackECS;
using UnityEngine;
using UnityEngine.TestTools;
using Debug = UnityEngine.Debug;

namespace StackEcs.IntegrationTests
{
    public class StackEcsPerformanceTests
    {
        [UnityTest]
        public IEnumerator SeparateOperationsSpeed()
        {
            const int count = 256 * 256;
            var client = new StackEcsClient();
            var sw = new Stopwatch();
            yield return null;

            for (var i = 0; i < count; i++)
            {
                var warmEntity = client.Ecs.CreateEntity();
                warmEntity.AddComponent<PositionComponent>();
                if (i % 8 == 0) warmEntity.AddComponent<MovementComponent>();
            }

            yield return null;

            sw.Restart();
            var entities = client.Ecs.Query.Include<PositionComponent>().GetEnumerator();
            var querySpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Query speed: {querySpeed} ms");

            yield return null;

            sw.Restart();
            foreach (var entity in entities) entity.GetComponent<PositionComponent>();
            var getComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Get component speed: {getComponentSpeed} ms");
        }

        [UnityTest]
        public IEnumerator ChangeEntitiesInArchetypeTest()
        {
            const int count = 256 * 256;
            var ecs = new StackEcsClient();

            var sw = new Stopwatch();

            yield return null;

            sw.Start();
            for (var i = 0; i < count; i++)
            {
                var entity = ecs.Ecs.CreateEntity();
                entity.AddComponent<PositionComponent>();
                if (i % 8 == 0) entity.AddComponent<MovementComponent>();
            }

            var createAndAddComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Create and add component speed: {createAndAddComponentSpeed} ms");

            yield return null;

            sw.Restart();
            foreach (var entity in ecs.Ecs.Query.Include<MovementComponent>()) entity.GetComponent<MovementComponent>();
            var getComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Get component speed: {getComponentSpeed} ms");

            yield return null;

            sw.Restart();
            foreach (var entity in ecs.Ecs.Query.Include<MovementComponent>())
                entity.AddComponent<IndexEntityComponent>();
            var changeArchetypeSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Change archetype speed: {changeArchetypeSpeed} ms");

            yield return null;

            sw.Restart();
            foreach (var entity in ecs.Ecs.Query.Include<MovementComponent>())
                entity.RemoveComponent<MovementComponent>();
            var removeComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Remove component with change archetype: {removeComponentSpeed} ms");

            yield return new WaitForSeconds(0.33f);
        }

        [UnityTest]
        public IEnumerator ChangeAllentitiesTests()
        {
            const int count = 256 * 256;
            var ecs = new StackEcsClient();

            var sw = new Stopwatch();

            yield return null;

            sw.Start();
            for (var i = 0; i < count; i++)
            {
                var entity = ecs.Ecs.CreateEntity();
                entity.AddComponent<PositionComponent>();
            }

            var createAndAddComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Create and add component speed: {createAndAddComponentSpeed} ms");

            yield return null;

            sw.Restart();
            foreach (var entity in ecs.Ecs.Query.Include<PositionComponent>()) entity.GetComponent<PositionComponent>();
            var getComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Get component speed: {getComponentSpeed} ms");

            yield return null;

            sw.Restart();
            foreach (var entity in ecs.Ecs.Query.Include<PositionComponent>()) entity.AddComponent<MovementComponent>();
            var changeArchetypeSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Change archetype speed: {changeArchetypeSpeed} ms");

            yield return null;

            sw.Restart();
            foreach (var entity in ecs.Ecs.Query.Include<PositionComponent>())
                entity.RemoveComponent<PositionComponent>();
            var removeComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Remove component with change archetype: {removeComponentSpeed} ms");

            yield return new WaitForSeconds(0.33f);
        }
    }
}