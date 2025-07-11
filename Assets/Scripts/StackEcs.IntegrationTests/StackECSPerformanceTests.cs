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
        public IEnumerator QueryOperationsSpeed()
        {
            const int count = 256 * 256;
            var parameters = new StackEcsParameters
                             {
                                 EntityMaxCount = count,
                                 ComponentTypeMaxCount = 8
                             };
            var client = new StackEcsClient(parameters);
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
            var entities1 = client.Ecs.Query.Include<PositionComponent>().GetEnumerator();
            var querySpeed1 = sw.ElapsedMilliseconds;
            Debug.Log($"Query all entities speed: {querySpeed1} ms");

            yield return null;

            sw.Restart();
            var entities2 = client.Ecs.Query.Include<MovementComponent>().GetEnumerator();
            var querySpeed2 = sw.ElapsedMilliseconds;
            Debug.Log($"Query 1/8 entities speed: {querySpeed2} ms");
        }

        [UnityTest]
        public IEnumerator ChangeAllEntitiesTests()
        {
            const int count = 256 * 256;
            var parameters = new StackEcsParameters
            {
                EntityMaxCount = count,
                ComponentTypeMaxCount = 8
            };
            var ecs = new StackEcsClient(parameters);

            //Warm up
            var warmEntity = ecs.Ecs.CreateEntity();
            warmEntity.AddComponent<PositionComponent>();
            warmEntity.AddComponent<MovementComponent>();
            warmEntity.Delete();
            
            var sw = new Stopwatch();

            yield return null;

            sw.Start();
            for (var i = 0; i < count; i++)
            {
                var entity = ecs.Ecs.CreateEntity();
            }
            
            var createAndAddComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Create entity speed: {createAndAddComponentSpeed} ms");
            
            yield return null;
            
            sw.Start();
            foreach (var entity in ecs.Ecs.Query) 
            {
                entity.AddComponent<PositionComponent>();
            }
            
            var addComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Add component speed : {addComponentSpeed} ms");

            yield return null;

            sw.Restart();
            foreach (var entity in ecs.Ecs.Query.Include<PositionComponent>()) entity.GetComponent<PositionComponent>();
            var getComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Get component speed: {getComponentSpeed} ms");

            yield return null;

            sw.Restart();
            foreach (var entity in ecs.Ecs.Query.Include<PositionComponent>()) entity.AddComponent<MovementComponent>();
            var changeArchetypeSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Change archetype speed (Add component): {changeArchetypeSpeed} ms");

            yield return null;

            sw.Restart();
            foreach (var entity in ecs.Ecs.Query.Include<PositionComponent>())
                entity.RemoveComponent<PositionComponent>();
            var removeComponentSpeed = sw.ElapsedMilliseconds;
            Debug.Log($"Change archetype speed (Remove Component): {removeComponentSpeed} ms");

            yield return new WaitForSeconds(0.33f);
        }
    }
}