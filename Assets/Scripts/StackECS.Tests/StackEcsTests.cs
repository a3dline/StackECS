using System;
using NUnit.Framework;
using StackECS;

namespace StackECSTests
{
    [Category("StackEcs")]
    public class StackEcsTests
    {
        [Test(Description = "Create entity. Entity index is 0.")]
        public void CreateEntity()
        {
            // Arrange
            using var stackEcs = new StackEcs(16);

            // Act
            var entity = stackEcs.CreateEntity();

            // Assert
            Assert.AreEqual(0, entity.Index);
        }

        [Test(Description = "Entity has unique index")]
        public void CreateTwoEntities()
        {
            // Arrange
            using var stackEcs = new StackEcs(16);

            // Act
            stackEcs.CreateEntity();
            var entity2 = stackEcs.CreateEntity();

            // Assert
            Assert.AreEqual(1, entity2.Index);
        }

        [Test(Description = "Ecs use deleted entity cache")]
        public void CreateEntityAfterDelete_UsedEntityFromCache()
        {
            // Arrange
            using var stackEcs = new StackEcs(16);

            // Act
            var entity = stackEcs.CreateEntity();
            entity.Delete();
            var entity2 = stackEcs.CreateEntity();

            // Assert
            Assert.AreEqual(0, entity2.Index);
        }

        [Test(Description = "Exception on modify entity after delete")]
        public void DeleteEntity_ThrowIfEntityAlreadyDeleted()
        {
            // Arrange
            using var stackEcs = new StackEcs(16);
            var entity = stackEcs.CreateEntity();

            // Act
            entity.Delete();

            Exception exception = null;
            // Assert
            try
            {
                entity.HasComponent<int>();
            }
            catch (InvalidOperationException e)
            {
                exception = e;
            }

            Assert.IsNotNull(exception);
            Assert.Pass("Entity already deleted");
        }

        [Test(Description = "Add component to entity")]
        public void AddComponentToEntity()
        {
            // Arrange
            using var stackEcs = new StackEcs(16);
            var entity = stackEcs.CreateEntity();

            // Act
            entity.AddComponent<int>();
            // Assert
            Assert.IsTrue(entity.HasComponent<int>());
        }

        [Test(Description = "Remove component from entity")]
        public void RemoveComponentFromEntity()
        {
            // Arrange
            using var stackEcs = new StackEcs(16);
            var entity = stackEcs.CreateEntity();
            entity.AddComponent<int>();

            // Act
            entity.RemoveComponent<int>();

            // Assert
            Assert.IsFalse(entity.HasComponent<int>());
        }
        
        [Test (Description = "Add component to entity. Throw if component already exists")]
        public void AddDuplicateComponent_ThrowIfComponentAlreadyExists()
        {
            // Arrange
            using var stackEcs = new StackEcs(16);
            var entity = stackEcs.CreateEntity();
            entity.AddComponent<int>();

            Exception exception = null;
            // Act
            try
            {
                entity.AddComponent<int>();
            }
            catch (InvalidOperationException e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.Pass("Component already exists");
        }
        
        [Test (Description = "Remove component from entity. Throw if component does not exist")]
        public void RemoveComponent_ThrowIfComponentDoesNotExist()
        {
            // Arrange
            using var stackEcs = new StackEcs(16);
            var entity = stackEcs.CreateEntity();

            Exception exception = null;
            // Act
            try
            {
                entity.RemoveComponent<int>();
            }
            catch (InvalidOperationException e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.Pass("Component does not exist");
        }
    }
}