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
            Assert.AreEqual("Cannot create more entities than the defined capacity.", exception.Message);
        }
    }
}