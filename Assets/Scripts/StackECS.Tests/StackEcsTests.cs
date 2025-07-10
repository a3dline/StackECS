using NUnit.Framework;
using StackECS;

namespace StackECSTests
{
    [Category("StackEcs")]
    public class StackEcsTests
    {
        [Test(Description = "Entity has unique index")]
        public void CreateTwoEntities_EntitiesHasUniqueIndex()
        {
            // Arrange
            var stackEcs = new StackEcs();

            // Act
            var entity1 = stackEcs.CreateEntity();
            var entity2 = stackEcs.CreateEntity();

            // Assert
            Assert.AreNotEqual(entity1.Index, entity2.Index);
        }

        [Test(Description = "Ecs use deleted entity cache")]
        public void CreateEntityAfterDelete_UsedEntityFromCache()
        {
            // Arrange
            var stackEcs = new StackEcs();

            // Act
            var entity1 = stackEcs.CreateEntity();
            var entity1Index = entity1.Index;
            entity1.Delete();
            var entity2 = stackEcs.CreateEntity();

            // Assert
            Assert.AreEqual(entity1Index, entity2.Index);
        }

        [Test(Description = "Cached entity does not have components from deleted entity")]
        public void CachedEntityDoesNotExistComponent()
        {
            // Arrange
            var stackEcs = new StackEcs();
            var entity1 = stackEcs.CreateEntity();
            entity1.AddComponent<int>();
            entity1.Delete();

            // Act
            var entity2 = stackEcs.CreateEntity();

            // Assert
            Assert.IsFalse(entity2.HasComponent<int>());
        }

        [Test(Description = "Add component to entity")]
        public void AddComponentToEntity_ComponentExist()
        {
            // Arrange
            var stackEcs = new StackEcs();
            var entity = stackEcs.CreateEntity();

            // Act
            entity.AddComponent<int>();
            // Assert
            Assert.IsTrue(entity.HasComponent<int>());
        }

        [Test(Description = "Remove component from entity")]
        public void RemoveComponentFromEntity_ComponentNotExist()
        {
            // Arrange
            var stackEcs = new StackEcs();
            var entity = stackEcs.CreateEntity();
            entity.AddComponent<int>();

            // Act
            entity.RemoveComponent<int>();

            // Assert
            Assert.IsFalse(entity.HasComponent<int>());
        }
        
        [Test(Description = "GetComponent returns correct component value after adding it")] 
        public void GetComponentAfterAdd_ComponentIsCorrect() 
        { 
            var stackEcs = new StackEcs(); 
            var entity = stackEcs.CreateEntity(); 
            entity.AddComponent<int>() = 42; 
            Assert.AreEqual(42, entity.GetComponent<int>()); 
        }

        [Test(Description = "EntityCount increases and decreases correctly")] 
        public void EntityCount_IncreasesAndDecreases() 
        { 
            var stackEcs = new StackEcs(); 
            var entity1 = stackEcs.CreateEntity(); 
            stackEcs.CreateEntity(); 
            Assert.AreEqual(2, stackEcs.EntityCount); 
            entity1.Delete(); 
            Assert.AreEqual(1, stackEcs.EntityCount); 
        }

        [Test(Description = "Add and remove different component types")] 
        public void AddRemoveDifferentComponentTypes() 
        { 
            var stackEcs = new StackEcs(); 
            var entity = stackEcs.CreateEntity(); 
            entity.AddComponent<int>(); 
            entity.AddComponent<float>(); 
            Assert.IsTrue(entity.HasComponent<int>()); 
            Assert.IsTrue(entity.HasComponent<float>()); 
            entity.RemoveComponent<int>(); 
            Assert.IsFalse(entity.HasComponent<int>()); 
            Assert.IsTrue(entity.HasComponent<float>()); 
        }
    }
}