# 📢 Notifications in Orchestrix.Mediator

Orchestrix.Mediator supports **fire-and-forget notifications** that can be handled by one or more handlers — sequentially or in parallel.

Notifications are used to **broadcast an event** to any interested handlers without expecting a return value.

---

## 🧾 1. Interfaces Overview

| Purpose                     | Interface                                |
|-----------------------------|------------------------------------------|
| Marker interface for events | `INotification`                          |
| Sequential handler          | `INotificationHandler<TNotification>`    |
| Parallel handler            | `IParallelNotificationHandler<TNotification>` |

✅ Both types can be used at the same time — they’ll be executed appropriately.

---

## 🧱 2. Define a Notification

```csharp
public record UserRegistered(string Email) : INotification;
```

---

## 🛠 3. Define Handlers

### 🔁 Sequential Handler

```csharp
public class LogUserHandler : INotificationHandler<UserRegistered>
{
    public ValueTask Handle(UserRegistered notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[LOG] User registered: {notification.Email}");
        return default;
    }
}
```

### ⚡ Parallel Handler

```csharp
public class EmailUserHandler : IParallelNotificationHandler<UserRegistered>
{
    public async ValueTask Handle(UserRegistered notification, CancellationToken cancellationToken)
    {
        await Task.Delay(500, cancellationToken); // Simulate async email sending
        Console.WriteLine($"[EMAIL] Welcome sent to {notification.Email}");
    }
}
```

---

## 🚀 4. Publishing Notifications

You can publish using:

```csharp
await publisher.Publish(new UserRegistered("user@example.com"));
```

Or via `IMediator`:

```csharp
await mediator.Publish(new UserRegistered("user@example.com"));
```

---

## ⚙️ 5. Handler Execution Behavior

| Handler Type                       | Executed     | Order           |
|-----------------------------------|--------------|------------------|
| `INotificationHandler<T>`         | ✅ Yes       | Sequentially     |
| `IParallelNotificationHandler<T>` | ✅ Yes       | In parallel      |

✅ Handlers are discovered via DI and resolved once.  
✅ Each is awaited (in-order or concurrently).

---

## 🧠 6. Hook Support

If you've registered `IPublishHook`, the following methods will be triggered:

- `OnPublishStart(...)`
- `OnPublishComplete(...)`
- `OnPublishError(...)`

Hooks are called **once per publish**, **not per handler**.

---

## 🧬 7. Source Generator Support

When using `Orchestrix.Mediator.SourceGenerators`, the following methods are generated:

```csharp
ValueTask DispatchPublish<TNotification>(TNotification)
ValueTask DispatchPublish(object)
```

These methods:

- ✅ Directly invoke all sequential and parallel handlers
- ✅ Avoid reflection and DI lookup
- ✅ Respect handler execution strategy

---

## 🧪 8. Testing Notification Logic

You can test handlers individually, or capture output via a test publisher:

```csharp
await publisher.Publish(new UserRegistered("test@test.com"));
// Then assert logs, side effects, etc.
```

---

## 🧭 Summary

| Feature                        | Description                               |
|--------------------------------|-------------------------------------------|
| `INotification`                | Marker for fire-and-forget events         |
| `INotificationHandler<T>`      | Sequential execution                      |
| `IParallelNotificationHandler<T>` | Parallel (async fan-out)              |
| `IPublisher.Publish(...)`      | Publish events to all handlers            |
| `IPublishHook`                 | Observe publishing lifecycle              |
| SourceGen support              | ✅ Full for all handler types             |
