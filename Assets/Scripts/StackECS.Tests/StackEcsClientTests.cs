using NUnit.Framework;
using StackECS;

namespace StackECSTests
{
    [Category("StackEcs")]
    public class StackEcsClientTests
    {
        [Test(Description = "All empty entities removed after update")]
        public void RemoveEmptyEntitiesAfterUpdate()
        {
            // Arrange
            var client = new StackEcsClient();
            client.Init();
            
            client.Ecs.CreateEntity();
            
            // Act
            client.Update();
            
            // Assert
            Assert.AreEqual(0, client.EntityCount);
        }

        [Test(Description = "Entity count property should works correct after delete entity")]
        public void EntityCountAfterRemove()
        {
            // Arrange
            var client = new StackEcsClient();
            client.Init();
            
            client.Ecs.CreateEntity();
            var toRemove = client.Ecs.CreateEntity();
            toRemove.Delete();
            
            // Act
            var entityCount = client.EntityCount;
            
            // Assert
            Assert.AreEqual(1, entityCount);
        }
    }
}