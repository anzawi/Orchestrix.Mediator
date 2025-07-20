# 🧠 Orchestrix.Mediator Concepts

This guide explains the core abstractions and design philosophy behind Orchestrix.Mediator.  
Whether you're coming from MediatR or starting fresh, understanding these concepts will help you use Orchestrix.Mediator effectively in real-world applications.

---

## 🧩 1. Separation of Concerns

Orchestrix.Mediator cleanly separates three responsibilities:

| Abstraction | Role                                       |
|-------------|--------------------------------------------|
| `ISender`   | Sends `IRequest` or `IRequest<T>` messages |
| `IPublisher`| Publishes `INotification` to handlers      |
| `IMediator` | Combines `ISender` + `IPublisher`          |

💡 Inject `ISender`, `IPublisher`, or `IMediator` based on what your component needs.  
Prefer narrower interfaces for better testability and responsibility.

---

## 🧾 2. Request/Response Messaging

Use `IRequest<T>` for sending messages that expect a response.

```csharp
public record GetUserQuery(Guid Id) : IRequest<UserDto>;

public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
{
    public ValueTask<UserDto> Handle(GetUserQuery request, CancellationToken ct)
    {
        return ValueTask.FromResult(new UserDto(request.Id, "Mohammad"));
    }
}
```

You can also use `IRequest` (non-generic) for "void" operations:

```csharp
public class Ping : IRequest;

public class PingHandler : IRequestHandler<Ping>
{
    public ValueTask Handle(Ping _, CancellationToken ct)
    {
        Console.WriteLine("Pong");
        return default;
    }
}
```

Both forms are supported and flow through the same pipeline.

---

## 📢 3. Notifications & Event Fan-Out

`INotification` is used for fire-and-forget events that may have multiple handlers.

```csharp
public record UserCreated(Guid Id) : INotification;
```

You can define handlers in two ways:

| Type                          | Behavior              |
|-------------------------------|------------------------|
| `INotificationHandler<T>`     | Executes sequentially |
| `IParallelNotificationHandler<T>` | Executes in parallel |

Example:

```csharp
public class LogHandler : INotificationHandler<UserCreated>
{
    public ValueTask Handle(UserCreated notification, CancellationToken ct)
    {
        Console.WriteLine($"[LOG] User created: {notification.Id}");
        return default;
    }
}

public class EmailHandler : IParallelNotificationHandler<UserCreated>
{
    public async ValueTask Handle(UserCreated notification, CancellationToken ct)
    {
        await Task.Delay(200, ct);
        Console.WriteLine($"[EMAIL] Sent welcome email to: {notification.Id}");
    }
}
```

Both types can coexist — Orchestrix.Mediator automatically orchestrates them.

---

## 📡 4. Streaming Requests

Use `IStreamRequest<T>` to return data as `IAsyncEnumerable<T>`:

```csharp
public record GetUsers : IStreamRequest<UserDto>;

public class StreamHandler : IStreamRequestHandler<GetUsers, UserDto>
{
    public async IAsyncEnumerable<UserDto> Handle(GetUsers _, [EnumeratorCancellation] CancellationToken ct)
    {
        yield return new UserDto(Guid.NewGuid(), "First");
        await Task.Delay(100, ct);
        yield return new UserDto(Guid.NewGuid(), "Second");
    }
}
```

Dispatch using:

```csharp
await foreach (var user in sender.CreateStream(new GetUsers(), ct))
{
    Console.WriteLine(user.Name);
}
```

---

## 🧱 5. Pipelines

Orchestrix.Mediator supports pipelines via `IPipelineBehavior<TRequest, TResponse>`.  
Each request is passed through registered behaviors before reaching the handler.

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        Console.WriteLine($"Handling: {typeof(TRequest).Name}");
        var response = await next(ct);
        Console.WriteLine($"Handled: {typeof(TResponse).Name}");
        return response;
    }
}
```

---

## 🪝 6. Lifecycle Hooks

Orchestrix.Mediator internally supports lifecycle instrumentation using:

- `ISendHook`
- `IPublishHook`
- `IStreamHook`

You can hook into request/notification execution for:

✅ Logging  
✅ Tracing  
✅ Timing  
✅ Audit  
✅ Observability

Configure via `HookConfiguration` and register with:

```csharp
services.RegisterOrchestrixHooks();
```

See [Hooks & Pipelines →](./05-HooksAndPipelines.md) for full details.

---

## 🧬 7. Source Generator (Optional)

Orchestrix.Mediator supports **opt-in** code generation for dispatching:

```bash
dotnet add package Orchestrix.Mediator.SourceGenerators
```

This eliminates runtime reflection and boosts performance.

To enable it:

```csharp
services.AddOrchestrix(options => 
{
    options.MediatorMode = MediatorMode.SourceGenerator;
}, assemblies);
```

Generates optimized dispatch methods via `IOrchestrixDispatcher`.  
See [Source Generators →](./04-SourceGenerators.md) for advanced usage.

---

## 🧭 8. CQRS (Optional Extension)

Install the CQRS package:

```bash
dotnet add package Orchestrix.Mediator.Cqrs
```

Adds marker interfaces:

```csharp
ICommand, ICommand<T>, IQuery<T>,
ICommandHandler<T>, ICommandHandler<T, R>, IQueryHandler<T, R>
```

See [CQRS Guide →](./03-CqrsGuide.md)

---

## 🧩 Summary of Concepts

| Concept              | Interface(s)                                 |
|----------------------|----------------------------------------------|
| One-to-One           | `IRequest<T>` + `IRequestHandler<T, R>`      |
| Fire-and-forget      | `INotification`, `INotificationHandler<T>`   |
| Parallel notifications | `IParallelNotificationHandler<T>`         |
| Streaming            | `IStreamRequest<T>`, `IStreamRequestHandler<T, R>` |
| Pipelines            | `IPipelineBehavior<T, R>`                    |
| Dispatching          | `ISender`, `IPublisher`, `IMediator`         |
| Codegen              | `IOrchestrixDispatcher` (generated)           |
| Hooks                | `ISendHook`, `IPublishHook`, `IStreamHook`  |
