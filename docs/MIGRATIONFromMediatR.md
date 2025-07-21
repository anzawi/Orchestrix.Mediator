# 🔁 Migrating from MediatR to Orchestrix.Mediator

This guide will help you transition your application from **MediatR** to **Orchestrix.Mediator** — safely and incrementally — with minimal disruption.

✅ Orchestrix.Mediator is **not** a drop-in replacement, but the concepts and usage are highly familiar.

---

# 🔁 Migrating from MediatR to Orchestrix.Mediator

This guide will help you transition your application from **MediatR** to **Orchestrix.Mediator** — safely and incrementally — with minimal disruption.

✅ Orchestrix.Mediator is **not** a drop-in replacement, but the concepts and usage are highly familiar.

---

## ❓ Is Orchestrix.Mediator a Full Replacement for MediatR?

**Yes. Orchestrix.Mediator is a complete, modern, and extensible replacement for MediatR.**

It preserves all familiar concepts:
- `IRequest`, `IRequestHandler`
- `INotification`, `INotificationHandler`
- `IMediator.Send(...)`, `Publish(...)`
- Pipeline behaviors via `IPipelineBehavior<,>`

But it also introduces:
- ✅ Native support for `IAsyncEnumerable<T>` streaming
- ✅ Built-in diagnostics via hooks (Send/Publish/Stream)
- ✅ Parallel notifications via `IParallelNotificationHandler<T>`
- ✅ Optional source generator for zero-reflection dispatch
- ✅ `TrySend` and `TryPublish` support
- ✅ Modern CQRS support via extension (`ICommand`, `IQuery`, etc.)
- ✅ Fully `ValueTask`-based and AOT compatible

> 🔁 While Orchestrix.Mediator is not a binary drop-in (you must change namespaces and service registration), it is a **complete architectural upgrade** — and a safe, testable, future-proof evolution of the MediatR model.

---

## 📦 Step 1: Install Orchestrix.Mediator

First, remove MediatR (optional):

```bash
dotnet remove package MediatR
```

Then install Orchestrix.Mediator:

```bash
dotnet add package Orchestrix.Mediator
```

Optional extensions:

```bash
dotnet add package Orchestrix.Mediator.SourceGenerators
dotnet add package Orchestrix.Mediator.Cqrs
```

---

## 🛠 Step 2: Configure Services

**MediatR (old)**

```csharp
services.AddMediatR(cfg => 
{
    ...
});
```

**Orchestrix.Mediator (new)**

```csharp
services.AddOrchestrix(cfg => 
{
    ....
});
```
> It looks the same, but the options are different!

To enable source generator:

```csharp
services.AddOrchestrix(cfg => 
{
    cfg.UseSourceGenerator();
});
```

---

## 🔄 Step 3: Replace IMediator

**MediatR:**

```csharp
public class MyController(IMediator mediator) { }
```

**Orchestrix.Mediator:**

```csharp
public class MyController(IMediator mediator) { }
// or
public class MyService(ISender sender) { }
// or
public class MyHandler(IPublisher publisher) { }
```

✅ You can inject the same roles with more granularity using `ISender` and `IPublisher`.

---

## 🧾 Step 4: Replace Request Interfaces

**MediatR:**

```csharp
public class MyCommand : IRequest<string>;
public class MyHandler : IRequestHandler<MyCommand, string> { ... }
```

**Orchestrix.Mediator:**

Same code still works:

```csharp
public class MyCommand : IRequest<string>;
public class MyHandler : IRequestHandler<MyCommand, string> { ... }
```

✅ Optionally switch to CQRS-style:

```csharp
public class MyCommand : ICommand<string>;
public class MyHandler : ICommandHandler<MyCommand, string> { ... }

public class MyQuery : IQuery<string>;
public class MyQueryHandler : IQueryHandler<MyQuery, string> { ... }
```

---

## 📢 Step 5: Replace Notification Handlers

**MediatR:**

```csharp
public class MyNotification : INotification;
public class Handler1 : INotificationHandler<MyNotification> { ... }
```

**Orchestrix.Mediator:**

Sequential handler (same interface):

```csharp
public class Handler1 : INotificationHandler<MyNotification> { ... }
```

Parallel handler for fan-out:

```csharp
public class Handler2 : IParallelNotificationHandler<MyNotification> { ... }
```

✅ Both types are supported and executed appropriately.

---

## 📡 Step 6: Replace Streaming (if used)

Orchestrix.Mediator supports `IAsyncEnumerable<T>` via:

```csharp
public class MyStream : IStreamRequest<T>;

public class StreamHandler : IStreamRequestHandler<MyStream, T>
{
    public async IAsyncEnumerable<T> Handle(MyStream request, [EnumeratorCancellation] CancellationToken ct)
    {
        // yield results here
    }
}
```

Dispatch the stream:

```csharp
await foreach (var item in sender.CreateStream(new MyStream(), ct))
{
    Console.WriteLine(item);
}
```

---

## 🪝 Step 7: Add Hooks (optional)

If you were using pipeline behaviors for logging, tracing, etc.,  
you can migrate those concerns into **hooks**:

```csharp
public class MyHook : ISendHook, IPublishHook { ... }

builder.Services.AddOrchestrix(cfg =>
{
    cfg.AddHooksFromAssemblies(typeof(MyHook).Assembly); // use if you want to register all Hooks in the assembly.
    cfg.AddHook<MyHook>();
  
});
```

---

## 🚧 Common Differences

| Concern                        | MediatR                         | Orchestrix.Mediator                             |
|-------------------------------|----------------------------------|----------------------------------------|
| Dispatch                      | `IMediator.Send()`               | `ISender.Send()` or `IMediator.Send()` |
| Pipelines                     | `IPipelineBehavior`              | ✅ Still supported                     |
| Multiple `IRequestHandler<T>` | Allowed but undefined            | ❌ Throws                              |
| `ICommand`, `IQuery`          | Manual                           | ✅ Provided via `Orchestrix.Mediator.Cqrs`       |
| Streaming                     | ❌ Not built-in                  | ✅ Built-in via `IStreamRequest<T>`    |
| Tracing / Observability       | Requires extra packages          | ✅ Built-in via hooks                  |

---

## ✅ Final Notes

- Orchestrix.Mediator is **modular**, **faster**, and **fully async**
- You can **migrate gradually**, replacing parts piece by piece
- Start with a few requests and move your entire app over time

---

## ❓ Need help?

📬 [Open an issue on GitHub](https://github.com/anzawi/Orchestrix.Mediator/issues)
