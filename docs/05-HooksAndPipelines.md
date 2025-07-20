# 🪝 Hooks & Pipelines in Orchestrix.Mediator

Orchestrix.Mediator gives you two powerful mechanisms to intercept, inspect, and enhance request processing:

| Feature  | Description                                      |
|----------|--------------------------------------------------|
| ✅ Hooks | Lifecycle instrumentation (Send, Publish, Stream)|
| ✅ Pipelines | Middleware-style behaviors (logging, validation, etc.) |

This guide explains when and how to use each, and how they differ.

---

## 🧩 1. Hooks Overview

Hooks provide direct integration points into the internal dispatch lifecycle of:

- `Send(...)`
- `Publish(...)`
- `CreateStream(...)`

They allow you to:

✅ Measure execution time  
✅ Trace handler activity  
✅ Collect logs  
✅ Audit input/output  
✅ Capture exceptions

---

## 🧱 2. Hook Interfaces

| Hook Type | Interface        |
|-----------|------------------|
| Send      | `ISendHook`      |
| Publish   | `IPublishHook`   |
| Stream    | `IStreamHook`    |

All hooks are automatically executed by `DispatcherMediator` via the internal `IHookExecutor`.

---

## 🧠 3. Hook Lifecycle Methods

Each hook type defines three entry points:

### ISendHook
```csharp
// Executed before the operation
ValueTask OnSendStart(...);

// Executed after the operation completes successfully
ValueTask OnSendComplete(...);

// Executed when an error occurs
ValueTask OnSendError(...);
```

### IPublishHook
```csharp
// Executed before the operation
ValueTask OnPublishStart(...);

// Executed after the operation completes successfully
ValueTask OnPublishComplete(...);

// Executed when an error occurs
ValueTask OnPublishError(...);
```

### IStreamHook
```csharp
ValueTask OnStreamStart(...);
ValueTask OnStreamError(...);

```

You can implement multiple hooks for different scenarios — e.g., performance monitoring, metrics, logging, etc.

---

## 🛠️ 4. Implementing a Hook

**Example: Logging send hooks**

```csharp
public class LoggingSendHook : ISendHook
{
    public ValueTask OnSendStart(object request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Hook] Sending: {request.GetType().Name}");
        return default;
    }

    public ValueTask OnSendComplete(object request, object? response, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Hook] Completed: {request.GetType().Name}");
        return default;
    }

    public ValueTask OnSendError(object request, Exception exception, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[Hook] Error in {request.GetType().Name}: {exception.Message}");
        return default;
    }
}

```

---

## 🧩 5. Hook Registration

Register hooks using:

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

Hooks are discovered via DI and executed automatically.

---

## 🧰 6. What Is `IHookExecutor`?

`IHookExecutor` is an internal abstraction used by `DispatcherMediator` to:

- Find and cache available hooks
- Invoke them in the correct order
- Handle exceptions safely

You **don’t need to interact** with it directly — it’s auto-registered.

---

## 🔁 6.1 Hook Execution Order

All registered hooks are:

- Executed **in registration order**
- **Awaited sequentially**
- **Never short-circuit** execution (i.e., they cannot cancel dispatch)
- **Exceptions are caught and logged internally** — your business logic is never affected

Hooks behave like observers — not interceptors.

---

## 🔄 7. Pipelines Overview

Pipelines are **middleware** that wrap request execution.  
They use the classic `IPipelineBehavior<TRequest, TResponse>` interface:

```csharp
public interface IPipelineBehavior<in TRequest, TResponse>
{
    ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}
```

---

## 📦 8. Adding a Pipeline Behavior

**Example: Logging behavior**

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        Console.WriteLine($"[Pipeline] Starting {typeof(TRequest).Name}");
        var result = await next(ct);
        Console.WriteLine($"[Pipeline] Finished {typeof(TResponse).Name}");
        return result;
    }
}
```

Register it:

```csharp
services.AddOrchestrix(cfg => 
{
    cfg.AddBehavior(typeof(LoggingBehavior<SomeRequest, SomeResponse>));
});
```
Use `AddOpenBehavior(typeof(LoggingBehavior<,>))` to apply the behavior to **all requests**.  
Use `AddBehavior(typeof(LoggingBehavior<SomeRequest, SomeResponse>))` to register for **a specific type**.

```csharp
services.AddOrchestrix(cfg => 
{
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
```

⚠️ Pipelines are applied **only to `IRequest<T>`** — not to notifications or streaming.

---

## ⚖️ 9. Hooks vs Pipelines

| Feature                   | Hooks                | Pipelines              |
|---------------------------|----------------------|-------------------------|
| Scope                     | Send / Publish / Stream | `IRequest<T>` only   |
| Works with `INotification`? | ✅ Yes             | ❌ No                  |
| Works with `IStreamRequest<T>`? | ✅ Yes        | ❌ No                  |
| Order                     | Internal / centralized | Chained manually     |
| Best for                  | Tracing, diagnostics, metrics | Business middleware, validation |

✅ Use **hooks** for **technical diagnostics**  
✅ Use **pipelines** for **cross-cutting business behaviors**

---

## 🧪 10. Testability

Hooks and pipelines are completely testable via `IMediator` or `ISender`.

- Use mocks to verify that they were triggered
- Assert on side effects (logs, metrics, etc.)

---

## 🧭 Summary

✅ Hooks instrument `Send`, `Publish`, and `Stream` directly  
✅ Pipelines wrap request handlers (`IRequest<T>`) only  
✅ Both support async, DI, and multiple registrations  
✅ Use `AddHook<...>()` or `AddHooksFromAssemblies(...)` for hooks  
✅ Use `AddBehavior(...)` or `AddOpenBehavior(...)` for pipelines
