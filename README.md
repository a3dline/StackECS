# Stack ECS for Unity Engine

## 1. What is Stack ECS?

Stack ECS is a fast and minimalistic Entity Component System (ECS) for Unity, using archetype-based storage for high performance and efficient entity filtering.

## 2. Limits

Stack ECS is configured with strict limits on the number of entities and component types. These limits are set via the `StackEcsParameters` class:

```csharp
var parameters = new StackEcsParameters {
    EntityMaxCount = 1024,         // Maximum number of entities
    ComponentTypeMaxCount = 64     // Maximum number of unique component types
};
var client = new StackEcsClient(parameters);
```

- **EntityMaxCount**: The maximum number of entities that can exist at the same time. Creating more will throw an exception.
- **ComponentTypeMaxCount**: The maximum number of unique component types that can be registered. Exceeding this will also throw an exception.

**Exceptions:**
- If you try to create more entities than `EntityMaxCount`, an `InvalidOperationException` will be thrown.
- If you try to register more component types than `ComponentTypeMaxCount`, an `InvalidOperationException` will be thrown.

These limits help keep memory usage predictable and performance optimal.

## 3. How to Use

### 3.1 Creating Systems
There are two main types of systems:
- **Init System** (`IEcsInitSystem`): Runs once during initialization.
- **Update System** (`IEcsUpdateSystem`): Runs every frame (on each `Update()` call).

**Example:**
```csharp
class MyInitSystem : IEcsInitSystem {
    public void Init(IEcs ecs) {
        var entity = ecs.CreateEntity();
        entity.AddComponent<MyComponent>();
    }
}

class MyUpdateSystem : IEcsUpdateSystem {
    public void Update(IEcs ecs) {
        foreach (var entity in ecs.Query.Include<MyComponent>()) {
            // Process entity
        }
    }
}
```

### 3.2 Client API
- Create a client: `var client = new StackEcsClient();`
- Add systems: `client.AddSystem(new MyInitSystem());`
- Initialize: `client.Init();`
- Update: `client.Update();`

**Example:**
```csharp
var client = new StackEcsClient();
client.AddSystem(new MyInitSystem());
client.AddSystem(new MyUpdateSystem());
client.Init();
client.Update();
```

### 3.3 Query API
- Filter entities by components:
```csharp
// All entities with MyComponent
foreach (var entity in ecs.Query.Include<MyComponent>()) { }

// Entities with both A and B
foreach (var entity in ecs.Query.Include<A>().Include<B>()) { }

// Entities with A but without B
foreach (var entity in ecs.Query.Include<A>().Exclude<B>()) { }
```

### 3.4 Entity API
- Add a component: `entity.AddComponent<MyComponent>()`
- Remove a component: `entity.RemoveComponent<MyComponent>()`
- Get a component: `ref var comp = ref entity.GetComponent<MyComponent>()`
- Check for a component: `entity.HasComponent<MyComponent>()`
- Delete entity: `entity.Delete()`

**Example:**
```csharp
var entity = ecs.CreateEntity();
entity.AddComponent<Health>().Value = 100;
if (entity.HasComponent<Health>()) {
    ref var health = ref entity.GetComponent<Health>();
    health.Value -= 10;
}
entity.RemoveComponent<Health>();
entity.Delete();
```

## 4. Hello World Example

See [README.hello-world.md](README.hello-world.md) for a full Unity-style Hello World example with systems, queries, and filtering.

---

Stack ECS is a fast, concise, and convenient ECS for Unity with archetype storage and flexible entity filtering.
