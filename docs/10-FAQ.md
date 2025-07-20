# ❓ Orchestrix.Mediator FAQ

Answers to common questions about using, configuring, and extending **Orchestrix.Mediator** in real-world .NET
applications.

---

## 🧠 General

### 🔹 Is Orchestrix.Mediator a wrapper around MediatR?

**No.** Orchestrix.Mediator is a clean-room implementation with a different architecture and goals:

- ❌ No dependency on MediatR
- ✅ Full support for parallel notifications, streaming, hooks, CQRS, and source generators
- ✅ Targeted for .NET 8, 9, 10, and future releases

---

### 🔹 Is it production-ready?

✅ **Yes.** Orchestrix.Mediator is designed for production:

- Async-only (`ValueTask` based)
- Modular and composable
- Fully testable and DI-compatible
- Native AOT safe

---

## ⚙️ Configuration

### 🔹 Do I have to use the source generator?

**No.** Orchestrix.Mediator works with or without the generator:

- `Reflection` mode (default): uses `IServiceProvider`
- `SourceGenerator` mode: uses compile-time generated dispatcher

Switch via:

```csharp
services.AddOrchestrix(cfg => 
{
    cfg.UseSourceGenerator();
});
```

---

### 🔹 What happens if I enable the generator but don’t generate?

You’ll get a clear exception at startup:

```plaintext
Orchestrix.Mediator: Source-Generator mode requires the Orchestrix.Mediator.SourceGenerators package to be installed,
and a valid GeneratedDispatcher must be generated.
```

---

### 🔹 Can I use both pipeline behaviors and hooks?

✅ **Yes.**

| Use Case       | Recommended                                |
|----------------|--------------------------------------------|
| Business logic | `IPipelineBehavior`                        |
| Observability  | `ISendHook`, `IPublishHook`, `IStreamHook` |

---

## 🔀 Usage

### 🔹 How do I send a void command?

You can use either:

```csharp
public class DoSomething : IRequest;
public class DoSomethingHandler : IRequestHandler<DoSomething> { ... }
```

Or:

```csharp
public class DoSomething : IRequest<Unit>;
public class DoSomethingHandler : IRequestHandler<DoSomething, Unit> { ... }
```

Both are supported.

---

### 🔹 Can I have multiple handlers for a single request?

❌ **No.** Orchestrix.Mediator enforces **one handler per `IRequest`**.

> However, notifications (`INotification`) **can have multiple handlers** (sequential and/or parallel).

---

### 🔹 Can I publish events in parallel?

✅ **Yes.** Just implement:

```csharp
public class MyHandler : IParallelNotificationHandler<MyEvent> { ... }
```

Orchestrix.Mediator will execute all in parallel using `Task.WhenAll`.

---

### 🔹 Does `IMediator` replace both `ISender` and `IPublisher`?

✅ **Yes.** It implements both:

```csharp
public interface IMediator : ISender, IPublisher;
```

Inject whichever matches the use case.

---

### 🔹 Do hooks require source generator?

❌ **No.** Hooks work in both **Reflection** and **SourceGen** modes.

---

### 🔹 Does Orchestrix.Mediator work with Minimal API and Controllers?

✅ **Absolutely.** Just inject `ISender`, `IPublisher`, or `IMediator`.

```csharp
app.MapPost("/cmd", async (ISender sender, MyCommand cmd) => { ... });
```

---

## 🧬 CQRS

### 🔹 Is the CQRS package required?

Only if you want:

- `ICommand`, `IQuery`
- Explicit semantic separation

Otherwise, stick to:

- `IRequest`, `IRequest<T>`

---

### 🔹 Is CQRS compatible with source generator?

✅ **Yes.**  
CQRS interfaces inherit from `IRequest` — no special handling required.

---

## 🧪 Testing

### 🔹 How do I mock Orchestrix.Mediator in unit tests?

Mock any of:

- `ISender`
- `IPublisher`
- `IMediator`

```csharp
var sender = new Mock<ISender>();
sender.Setup(s => s.Send(It.IsAny<IRequest<string>>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync("test");
```

---

## 🧩 Extensibility

### 🔹 Can I define my own hook?

✅ **Yes.** Just implement one of:

- `ISendHook`
- `IPublishHook`
- `IStreamHook`

Then register:

```csharp
builder.Services.AddOrchestrix(cfg =>
{
    cfg.AddHooksFromAssemblies(typeof(MyHook).Assembly); // use to register all hooks in the assembly
    cfg.AddHook<MyHook>(); // use to register specific hook.
});
```

---

### 🔹 Can I resolve and use `IOrchestrixDispatcher` directly?

**Yes**, but only if:

- You use `MediatorMode.SourceGenerator`
- You’ve built the project with the generator enabled

Then:

```csharp
var dispatcher = provider.GetRequiredService<IOrchestrixDispatcher>();
await dispatcher.Dispatch(new SomeQuery());
```

> `IOrchestrixDispatcher` used internally with IMediator, ISender and IPublisher use it directly if you want advanced usage.
---

### 🔹 Is Orchestrix.Mediator a full replacement for MediatR?

✅ **Yes — Orchestrix.Mediator is a full, extensible, and production-grade replacement for MediatR.**

It supports everything you expect:

- `IRequest`, `IRequest<T>`, `IRequestHandler<>`
- `INotification`, `INotificationHandler<>`
- `IMediator.Send`, `Publish`, etc.
- `IPipelineBehavior<TRequest, TResponse>`

But also improves on MediatR by introducing:

- ✅ **Streaming support** with `IStreamRequest<T>` and `IAsyncEnumerable<T>`
- ✅ **Parallel notification handling** using `IParallelNotificationHandler<T>`
- ✅ **Source generator** support to eliminate all reflection in hot paths
- ✅ **Hooks** for observability, tracing, and instrumentation (`ISendHook`, `IPublishHook`, etc.)
- ✅ **TrySend / TryPublish** for safe dispatch without throwing
- ✅ Cleanly separated dispatch contracts (`ISender`, `IPublisher`, `IMediator`)
- ✅ Built-in CQRS abstraction (`ICommand`, `IQuery`, etc.) via `Orchestrix.Mediator.Cqrs`

🧱 Internally, Orchestrix.Mediator uses a modern architecture:

- `ValueTask` everywhere
- Fast, AOT-compatible code paths
- Fully async and DI-compliant

> ⚠️ The only difference: it's not a binary drop-in.  
> You'll update namespaces and registrations — but the API shape remains nearly identical, and usage is just as
> intuitive.

You can migrate gradually and enjoy modern features with minimal disruption.

---

## ❓ More Questions?

📬 Open a GitHub issue → [Orchestrix.Mediator Issues](https://github.com/MohammadAnzawi/Orchestrix.Mediator/issues)  
📚 Explore the [API Reference →](./99-ApiReference.md)
