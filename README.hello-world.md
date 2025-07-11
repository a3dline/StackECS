# Stack ECS Hello World Example

Below is a realistic Unity example. We create a MonoBehaviour, initialize the ECS client and systems in `Start`, and call `client.Update()` in `Update`. There are three systems:

1. **Entity Creation System**: Creates entities with two components: `IndexComponent` (for all) and `ValueComponent` (only for even indices, stores a value). Every fourth entity also gets a `TagComponent` (used for filtering).
2. **Value Update System**: Increments the value in `ValueComponent` for all entities that have it.
3. **Logging System**: Logs the values of all entities with `ValueComponent` to the Unity console, except those that have the `TagComponent`.

```csharp
using UnityEngine;
using StackECS;
using System.Collections.Generic;

// Components
struct IndexComponent { public int Index; }
struct ValueComponent { public int Value; }
struct TagComponent { } // Tag for filtering

// System 1: Create entities
class CreateEntitiesSystem : IEcsInitSystem {
    private readonly int _count;
    public CreateEntitiesSystem(int count) { _count = count; }
    public void Init(IEcs ecs) {
        for (int i = 0; i < _count; i++) {
            var entity = ecs.CreateEntity();
            entity.AddComponent<IndexComponent>().Index = i;
            if (i % 2 == 0)
                entity.AddComponent<ValueComponent>().Value = i * 10;
            if (i % 4 == 0)
                entity.AddComponent<TagComponent>(); // Add tag to every fourth entity
        }
    }
}

// System 2: Update ValueComponent
class ValueUpdateSystem : IEcsUpdateSystem {
    public void Update(IEcs ecs) {
        foreach (var entity in ecs.Query.Include<ValueComponent>()) {
            ref var value = ref entity.GetComponent<ValueComponent>();
            value.Value++;
        }
    }
}

// System 3: Log values, but exclude entities with TagComponent
class LoggingSystem : IEcsUpdateSystem {
    public void Update(IEcs ecs) {
        foreach (var entity in ecs.Query.Include<IndexComponent>().Include<ValueComponent>().Exclude<TagComponent>()) {
            var index = entity.GetComponent<IndexComponent>().Index;
            var value = entity.GetComponent<ValueComponent>().Value;
            Debug.Log($"Entity {index}: Value = {value}");
        }
    }
}

// MonoBehaviour to run ECS
public class EcsExampleMono : MonoBehaviour {
    private StackEcsClient _client;
    void Start() {
        _client = new StackEcsClient();
        _client.AddSystem(new CreateEntitiesSystem(10));
        _client.AddSystem(new ValueUpdateSystem());
        _client.AddSystem(new LoggingSystem());
        _client.Init();
    }
    void Update() {
        _client.Update();
    }
}
```

### Steps:
1. **Create a MonoBehaviour** (`EcsExampleMono`).
2. **In `Start`**: Initialize the ECS client and add systems.
3. **In `Update`**: Call `client.Update()` to process all systems each frame.
4. **Systems**: Handle entity creation, value updates, and logging (excluding tagged entities). 