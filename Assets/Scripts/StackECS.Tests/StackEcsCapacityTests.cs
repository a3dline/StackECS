using System;
using NUnit.Framework;
using StackECS;

namespace StackECSTests
{
    [Category("StackEcs")]
    public class StackEcsCapacityTests
    {
        [Test (Description = "Add entity more than capacity should throw correct exception")]
        public void AddEntityMoreThanCapacity_ThrowException()
        {
            // Arrange
            var parameter = new StackEcsParameters
            {
                EntityMaxCount = 2
            };
            var stackEcs = new StackEcs(parameter);

            // Act
            stackEcs.CreateEntity();
            stackEcs.CreateEntity();

            Exception exception = null;
            try
            {
                stackEcs.CreateEntity(); // This should exceed the capacity
            }
            catch (InvalidOperationException e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.Pass("Cannot create more entities than the defined capacity.");
        }
        
        [Test(Description = "Add component more than capacity should throw correct exception")]
        public void AddComponentMoreThanCapacity_ThrowException()
        {
            // Arrange
            var parameter = new StackEcsParameters
            {
                ComponentTypeMaxCount = 2
            };
            var stackEcs = new StackEcs(parameter);
            var entity = stackEcs.CreateEntity();

            // Act
            entity.AddComponent<int>();
            entity.AddComponent<float>();

            Exception exception = null;
            try
            {
                entity.AddComponent<double>(); // This should exceed the capacity
            }
            catch (InvalidOperationException e)
            {
                exception = e;
            }

            // Assert
            Assert.IsNotNull(exception);
            Assert.Pass("Cannot add more components than the defined capacity.");
        }
    }
}