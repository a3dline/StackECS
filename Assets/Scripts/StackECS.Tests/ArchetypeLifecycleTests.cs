using NUnit.Framework;
using StackECS;

namespace StackECSTests
{
    [Category("StackEcs")]
    public class ArchetypeLifecycleTests
    {
        [Test]
        public void Archetype_ShouldBeReleased_WhenNoEntitiesRemain()
        {
            // Arrange
            var ecs = new StackEcs();
            var initialArchetypeCount = ecs.ArchetypeCount;

            // Act - create entity and add component
            var entity = ecs.CreateEntity();
            entity.AddComponent<TestComponentA>();

            var archetypeCountAfterAdd = ecs.ArchetypeCount;

            // Delete entity
            entity.Delete();

            var finalArchetypeCount = ecs.ArchetypeCount;

            // Assert
            Assert.Greater(archetypeCountAfterAdd,
                           initialArchetypeCount,
                           "New archetype should be created when adding component");
            Assert.AreEqual(initialArchetypeCount,
                            finalArchetypeCount,
                            "Archetype should be removed when no entities remain");
        }

        [Test]
        public void DifferentArchetypes_ShouldBeCreated_WhenComponentsAreChanged()
        {
            // Arrange
            var ecs = new StackEcs();
            var initialArchetypeCount = ecs.ArchetypeCount;

            // Act
            var entity = ecs.CreateEntity();
            var archetypeCountAfterCreate = ecs.ArchetypeCount;

            // Add the first component
            entity.AddComponent<TestComponentA>();
            var archetypeCountAfterComponentA = ecs.ArchetypeCount;

            // Add a second component
            entity.AddComponent<TestComponentB>();
            var archetypeCountAfterComponentB = ecs.ArchetypeCount;

            // Remove the first component
            entity.RemoveComponent<TestComponentA>();
            var archetypeCountAfterRemoveA = ecs.ArchetypeCount;

            // Remove the second component
            entity.RemoveComponent<TestComponentB>();
            var archetypeCountAfterRemovalB = ecs.ArchetypeCount;
            
            entity.Delete();
            var finalArchetypeCount = ecs.ArchetypeCount;

            // Assert
            // Only the empty archetype should exist initially
            Assert.That(initialArchetypeCount, Is.EqualTo(0));
            // After creating the entity, the empty archetype is created
            Assert.That(archetypeCountAfterCreate, Is.EqualTo(1));
            // Adding component A creates a new archetype [A], so now two archetypes: [empty], [A]
            Assert.That(archetypeCountAfterComponentA, Is.EqualTo(1));
            // Adding component B creates a new archetype [A,B], [A] is removed, so still one archetype
            Assert.That(archetypeCountAfterComponentB, Is.EqualTo(1));
            // Removing component A creates a new archetype [B], [A,B] is removed, so still one archetype
            Assert.That(archetypeCountAfterRemoveA, Is.EqualTo(1));
            // Removing component B returns entity to [empty] archetype, so still one archetype
            Assert.That(archetypeCountAfterRemovalB, Is.EqualTo(1));
            // After deleting the entity, no archetypes remain
            Assert.That(finalArchetypeCount, Is.EqualTo(0));
        }

        [Test]
        public void Archetype_ShouldExist_WhileEntitiesWithSameComponentsExist()
        {
            // Arrange
            var ecs = new StackEcs();

            // Act
            var entity1 = ecs.CreateEntity();
            var entity2 = ecs.CreateEntity();

            entity1.AddComponent<TestComponentA>();
            entity2.AddComponent<TestComponentA>();

            var archetypeCountWithTwoEntities = ecs.ArchetypeCount;

            // Remove one entity
            entity1.Delete();
            var archetypeCountAfterOneDelete = ecs.ArchetypeCount;

            // Remove the second entity
            entity2.Delete();
            var archetypeCountAfterAllDeleted = ecs.ArchetypeCount;

            // Assert
            Assert.That(archetypeCountWithTwoEntities, Is.EqualTo(1), "There should be one archetype for two entities with the same component");
            Assert.That(archetypeCountAfterOneDelete, Is.EqualTo(1), "Archetype should still exist after deleting one entity");
            Assert.That(archetypeCountAfterAllDeleted, Is.EqualTo(0), "Archetype should be removed after deleting all entities");
        }

        [Test]
        public void Archetype_Count_ShouldReflectComponentChanges_WithMultipleEntities()
        {
            // Arrange
            var ecs = new StackEcs();
            var entity1 = ecs.CreateEntity();
            var entity2 = ecs.CreateEntity();

            // Act & Assert
            // Both entities are empty, so only the empty archetype exists
            Assert.That(ecs.ArchetypeCount, Is.EqualTo(1), "Should be 1 archetype for empty entities");

            // Add component A to both entities
            entity1.AddComponent<TestComponentA>();
            entity2.AddComponent<TestComponentA>();
            Assert.That(ecs.ArchetypeCount, Is.EqualTo(1), "Should be 2 archetypes: [empty], [A]");

            // Add component B to entity1
            entity1.AddComponent<TestComponentB>();
            Assert.That(ecs.ArchetypeCount, Is.EqualTo(2), "Should be 3 archetypes: [empty], [A], [A,B]");

            // Add component B to entity2 (now both have [A,B], so [A] archetype should be empty and removed)
            entity2.AddComponent<TestComponentB>();
            Assert.That(ecs.ArchetypeCount, Is.EqualTo(1), "Should be 2 archetypes: [empty], [A,B]");

            // Remove component B from entity1 (now [A,B] and [A] archetypes exist)
            entity1.RemoveComponent<TestComponentB>();
            Assert.That(ecs.ArchetypeCount, Is.EqualTo(2), "Should be 3 archetypes: [empty], [A], [A,B]");

            // Remove component B from entity2 (now both have [A], so [A,B] archetype should be empty and removed)
            entity2.RemoveComponent<TestComponentB>();
            Assert.That(ecs.ArchetypeCount, Is.EqualTo(1), "Should be 2 archetypes: [empty], [A]");

            // Remove component A from both entities (all entities are empty, so only the empty archetype remains)
            entity1.RemoveComponent<TestComponentA>();
            entity2.RemoveComponent<TestComponentA>();
            Assert.That(ecs.ArchetypeCount, Is.EqualTo(1), "Should be 1 archetype for empty entities");
        }

        // Simple components for testing
        public struct TestComponentA
        {
            public int Value;
        }

        public struct TestComponentB
        {
            public float Value;
        }
    }
}