using System;
using NUnit.Framework;
using StackECS;

namespace StackECSTests
{
    [Category("StackEcs")]
    public class StackEcsExceptionsTests
    {
        [Test(Description = "Exception on check component on deleted entity")]
        public void HasComponentOnDeletedEntity_ThrowIfEntityAlreadyDeleted()
        {
            // Arrange
            var stackEcs = new StackEcs();
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
        
        [Test(Description = "Exception on add component to deleted entity")]
        public void AddComponentToDeletedEntity_ThrowIfEntityAlreadyDeleted()
        {
            // Arrange
            var stackEcs = new StackEcs();
            var entity = stackEcs.CreateEntity();
            entity.Delete();

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
            Assert.Pass("Entity already deleted");
        }
        
        [Test(Description = "Exception on remove component from deleted entity")]
        public void RemoveComponentFromDeletedEntity_ThrowIfEntityAlreadyDeleted()
        {
            // Arrange
            var stackEcs = new StackEcs();
            var entity = stackEcs.CreateEntity();
            entity.Delete();

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
            Assert.Pass("Entity already deleted");
        }
        
        [Test(Description = "Exception on get component from deleted entity")]
        public void GetComponentFromDeletedEntity_ThrowIfEntityAlreadyDeleted()
        {
            // Arrange
            var stackEcs = new StackEcs();
            var entity = stackEcs.CreateEntity();
            entity.Delete();

            Exception exception = null;
            // Act
            try
            {
                entity.GetComponent<int>();
            }
            catch (InvalidOperationException e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.Pass("Entity already deleted");
        }
        
        [Test (Description = "Add duplicate component to entity. Throw if component already exists")]
        public void AddDuplicateComponent_ThrowIfComponentAlreadyExists()
        {
            // Arrange
            var stackEcs = new StackEcs();
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
        
        [Test (Description = "Remove component that does not exist. Throw if component does not exist")]
        public void RemoveComponentThatDoesNotExist_ThrowIfComponentDoesNotExist()
        {
            // Arrange
            var stackEcs = new StackEcs();
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
        
        [Test(Description = "Exception is try to delete entity that already deleted")]
        public void DeleteAlreadyDeletedEntity_ThrowIfEntityAlreadyDeleted()
        {
            // Arrange
            var stackEcs = new StackEcs();
            var entity = stackEcs.CreateEntity();
            entity.Delete();

            Exception exception = null;
            // Act
            try
            {
                entity.Delete();
            }
            catch (InvalidOperationException e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.Pass("Entity already deleted");
        }
    }
}