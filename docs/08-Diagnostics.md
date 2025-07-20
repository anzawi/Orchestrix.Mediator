# 🔍 Diagnostics in Orchestrix.Mediator

Orchestrix.Mediator includes a built-in diagnostics system powered by **hook interfaces** that enable:

- Tracing request and notification lifecycles
- Logging execution steps and outcomes
- Observing stream flows and failures
- Integrating with metrics, APM, and tracing systems

---

## 🧠 1. Diagnostics Architecture

Orchestrix.Mediator does not rely on middleware for instrumentation.  
Instead, it uses dedicated **hook interfaces**:

| Lifecycle Target   | Hook Interface   |
|--------------------|------------------|
| `Send(...)`        | `ISendHook`      |
| `Publish(...)`     | `IPublishHook`   |
| `CreateStream(...)`| `IStreamHook`    |

These are executed internally by the dispatcher (either `Mediator` or `DispatcherMediator`) using `IHookExecutor`.

---

## 🛠 2. Hook Configuration

Hooks are discovered via DI and can be enabled with:

```csharp
services.AddOrchestrix(cfg => 
{
     cfg.AddHook<MyHook>();
});
```
or add all hooks using assembly scan

```csharp
services.AddOrchestrix(cfg => 
{
     cfg.AddHooksFromAssemblies(typeof(MyHook).Assembly);
});
```

---

## 🔩 3. Hook Interfaces Recap

### `ISendHook`

```csharp
ValueTask OnSendStart(object request, CancellationToken ct);
ValueTask OnSendComplete(object request, object? response, CancellationToken ct);
ValueTask OnSendError(object request, Exception ex, CancellationToken ct);
```

### `IPublishHook`

```csharp
ValueTask OnPublishStart(object notification, CancellationToken ct);
ValueTask OnPublishComplete(object notification, CancellationToken ct);
ValueTask OnPublishError(object notification, Exception ex, CancellationToken ct);
```

### `IStreamHook`

```csharp
ValueTask OnStreamStart(object request, CancellationToken ct);
ValueTask OnStreamError(object request, Exception ex, CancellationToken ct);
```

---

## 📋 4. Sample Logging Hook

```csharp
public class LoggingHook : ISendHook, IPublishHook, IStreamHook
{
    public ValueTask OnSendStart(object request, CancellationToken ct)
    {
        Console.WriteLine($"[SEND] → {request.GetType().Name}");
        return default;
    }

    public ValueTask OnSendComplete(object request, object? response, CancellationToken ct)
    {
        Console.WriteLine($"[SEND ✅] ← {response?.GetType().Name ?? "void"}");
        return default;
    }

    public ValueTask OnSendError(object request, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"[SEND ❌] {request.GetType().Name}: {ex.Message}");
        return default;
    }

    public ValueTask OnPublishStart(object notification, CancellationToken ct)
    {
        Console.WriteLine($"[PUBLISH] → {notification.GetType().Name}");
        return default;
    }

    public ValueTask OnPublishComplete(object notification, CancellationToken ct)
    {
        Console.WriteLine($"[PUBLISH ✅] ← {notification.GetType().Name}");
        return default;
    }

    public ValueTask OnPublishError(object notification, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"[PUBLISH ❌] {ex.Message}");
        return default;
    }

    public ValueTask OnStreamStart(object request, CancellationToken ct)
    {
        Console.WriteLine($"[STREAM] → {request.GetType().Name}");
        return default;
    }

    public ValueTask OnStreamError(object request, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"[STREAM ❌] {request.GetType().Name}: {ex.Message}");
        return default;
    }
}
```

Register it:
```csharp
builder.Services.AddOrchestrix(cfg =>
{
    cfg.RegisterHandlersFromAssemblies(typeof(SomeHandler).Assembly);
    cfg.AddHook<LoggingHook>(); // add this line
});
```

---

## 🧪 5. When Are Hooks Executed?

| Method         | Trigger                          |
|----------------|----------------------------------|
| `OnStart`      | Immediately before execution     |
| `OnComplete`   | After success                    |
| `OnError`      | After exception is caught (safe) |

✅ Hooks **never throw** and are **always awaited**.

---

## 🧬 6. Combined with Source Generators

Whether you use:

- `MediatorMode.Reflection` (default), or
- `MediatorMode.SourceGenerator` (optimized)

➡️ Hooks will still be triggered — the **generator does not bypass hooks**.

---

## 🧭 Summary

| Feature                  | Description                            |
|--------------------------|----------------------------------------|
| `ISendHook`              | Observe request dispatch lifecycle     |
| `IPublishHook`           | Observe notification publishing        |
| `IStreamHook`            | Observe stream execution               |
| Hook orchestration       | Internally via `IHookExecutor`         |
| Registration             | `AddHook`, `AddHooksFromAssemblies`    |
| SourceGen compatible     | ✅ Yes                                  |
