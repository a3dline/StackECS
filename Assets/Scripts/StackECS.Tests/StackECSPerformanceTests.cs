using NUnit.Framework;
using StackECS;
using Unity.PerformanceTesting;

namespace StackECSTests
{
    public class StackEcsPerformanceTests
    {
        [Test]
        [Performance]
        public void QueryTests()
        {
            const int count = 256 * 256;
            var client = new StackEcsClient();
            for (var i = 0; i < count; i++)
            {
                var entity = client.Ecs.CreateEntity();
                entity.AddComponent<int>();
            }

            Measure.Method(() =>
                   {
                       foreach (var entity in client.Ecs.Query.Include<int>()) { }
                   })
                   .WarmupCount(1)
                   .MeasurementCount(100)
                   .ProfilerMarkers(new SampleGroup("Marker"), new SampleGroup("Copy"))
                   .Run();
        }
    }
}