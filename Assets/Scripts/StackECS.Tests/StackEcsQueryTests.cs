using System;
using NUnit.Framework;
using StackECS;

namespace StackECSTests
{
    [Category("StackEcs")]
    public class StackEcsQueryTests
    {
        [Test(Description = "Ecs query return all entities with component from one archetype")]
        public void GetEntitiesWithComponents()
        {
            // Arrange
            var ecs = new StackEcs();

            var entity1 = ecs.CreateEntity();
            entity1.AddComponent<int>();
            var entity2 = ecs.CreateEntity();
            entity2.AddComponent<int>();
            var entity3 = ecs.CreateEntity();
            entity3.AddComponent<float>();

            // Act
            var entities = ecs.Query.Include<int>();
            var intCount = 0;
            foreach (var enity in entities) intCount++;

            entities = ecs.Query.Include<float>();
            var floatCount = 0;
            foreach (var enity in entities) floatCount++;

            // Assert
            Assert.AreEqual(2, intCount);
            Assert.AreEqual(1, floatCount);
        }

        [Test(Description = "Query can return entities with two components ore more")]
        public void GetEntitiesWithTwoComponents()
        {
            // Arrange
            var ecs = new StackEcs();

            var entity1 = ecs.CreateEntity();
            entity1.AddComponent<int>();
            entity1.AddComponent<float>();
            var entity2 = ecs.CreateEntity();
            entity2.AddComponent<int>();
            entity2.AddComponent<float>();
            var entity3 = ecs.CreateEntity();
            entity3.AddComponent<float>();

            // Act
            var entities = ecs.Query.Include<int>().Include<float>();
            var intFloatCount = 0;
            foreach (var enity in entities) intFloatCount++;

            // Assert
            Assert.AreEqual(2, intFloatCount);
        }

        [Test(Description = "Query can return entities without component")]
        public void GetEntitiesWithoutComponent()
        {
            // Arrange
            var ecs = new StackEcs();

            var entity1 = ecs.CreateEntity();
            entity1.AddComponent<int>();
            entity1.AddComponent<float>();
            var entity2 = ecs.CreateEntity();
            entity2.AddComponent<int>();
            var entity3 = ecs.CreateEntity();
            entity3.AddComponent<float>();
            var entity4 = ecs.CreateEntity();
            entity4.AddComponent<bool>();

            // Act
            var entities = ecs.Query.Exclude<float>();
            var count = 0;
            foreach (var enity in entities) count++;

            // Assert
            Assert.AreEqual(2, count);
        }

        [Test(Description = "Query can combine filters for include and exclude components")]
        public void GetEntitiesWithAndWithoutComponents()
        {
            // Arrange
            var ecs = new StackEcs();

            var entity1 = ecs.CreateEntity();
            entity1.AddComponent<int>();
            entity1.AddComponent<float>();
            var entity2 = ecs.CreateEntity();
            entity2.AddComponent<int>();
            var entity3 = ecs.CreateEntity();
            entity3.AddComponent<float>();
            var entity4 = ecs.CreateEntity();
            entity4.AddComponent<bool>();

            // Act
            var entities = ecs.Query.Include<int>().Exclude<float>();
            var count = 0;
            foreach (var enity in entities) count++;

            // Assert
            Assert.AreEqual(1, count);
        }

        [Test(Description = "Ecs query return all entities with component from different archetypes")]
        public void GetEntitiesWithComponentsFromArchetypes()
        {
            // Arrange            
            var ecs = new StackEcs();

            var entity1 = ecs.CreateEntity();
            entity1.AddComponent<int>();
            var entity2 = ecs.CreateEntity();
            entity2.AddComponent<int>();
            entity2.AddComponent<float>();
            entity2.AddComponent<bool>();
            var entity3 = ecs.CreateEntity();
            entity3.AddComponent<float>();
            var entity4 = ecs.CreateEntity();
            entity4.AddComponent<bool>();

            // Act
            var entities = ecs.Query.Include<int>();
            var intCount = 0;
            foreach (var enity in entities) intCount++;

            entities = ecs.Query.Include<float>();
            var floatCount = 0;
            foreach (var enity in entities) floatCount++;

            entities = ecs.Query.Include<bool>();
            var boolCount = 0;
            foreach (var enity in entities) boolCount++;

            // Assert
            Assert.AreEqual(2, intCount);
            Assert.AreEqual(2, floatCount);
            Assert.AreEqual(2, boolCount);
        }

        [Test(Description =
                     "Query can not be cached and reused for enumerate entities. Throw exception if try to reuse query")]
        public void CachedQuery()
        {
            // Arrange
            var ecs = new StackEcs();

            var entity1 = ecs.CreateEntity();
            entity1.AddComponent<int>();
            var entity2 = ecs.CreateEntity();
            entity2.AddComponent<int>();
            var entity3 = ecs.CreateEntity();
            entity3.AddComponent<float>();

            // Act
            var cacheQuery = ecs.Query.Include<int>();

            foreach (var _ in cacheQuery) { }

            Exception exception = null;
            try
            {
                foreach (var _ in cacheQuery) { } 
            }
            catch (InvalidOperationException e)
            {
                exception = e;
            }

            // Assert
            Assert.That(exception, Is.Not.Null);
        }

        [Test(Description = "Query can be changed after use")]
        public void ChangeQueryAfterUse()
        {
            // Arrange
            var ecs = new StackEcs();

            var entity1 = ecs.CreateEntity();
            entity1.AddComponent<int>();
            var entity2 = ecs.CreateEntity();
            entity2.AddComponent<int>();
            var entity3 = ecs.CreateEntity();
            entity3.AddComponent<float>();

            // Act
            var cacheQuery = ecs.Query.Include<int>();

            var intCount1 = 0;
            foreach (var enity in cacheQuery) intCount1++;

            cacheQuery = ecs.Query.Include<float>();

            var intFloatCount = 0;
            foreach (var entity in cacheQuery) intFloatCount++;

            // Assert
            Assert.AreEqual(2, intCount1);
            Assert.AreEqual(1, intFloatCount);
        }

        [Test(Description = "Query returns empty if no entities")]
        public void QueryReturnsEmptyIfNoEntities()
        {
            var ecs = new StackEcs();
            var entities = ecs.Query.Include<int>();
            var count = 0;
            foreach (var entity in entities) count++;
            Assert.AreEqual(0, count);
        }

        [Test(Description = "Query by non-existent component returns empty")]
        public void QueryByNonExistentComponentReturnsEmpty()
        {
            var ecs = new StackEcs();
            var e = ecs.CreateEntity();
            e.AddComponent<float>();
            var entities = ecs.Query.Include<int>();
            var count = 0;
            foreach (var entity in entities) count++;
            Assert.AreEqual(0, count);
        }

        [Test(Description = "Query after deleting all entities returns empty")]
        public void QueryAfterDeletingAllEntitiesReturnsEmpty()
        {
            var ecs = new StackEcs();
            var e1 = ecs.CreateEntity();
            var e2 = ecs.CreateEntity();
            e1.AddComponent<int>();
            e2.AddComponent<int>();
            e1.Delete();
            e2.Delete();
            var entities = ecs.Query.Include<int>();
            var count = 0;
            foreach (var entity in entities) count++;
            Assert.AreEqual(0, count);
        }
    }
}