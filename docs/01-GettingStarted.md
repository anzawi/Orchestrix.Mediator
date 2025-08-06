# 🏁 Getting Started with Orchestrix.Mediator

Welcome to **Orchestrix.Mediator** — a modern, extensible mediator engine for .NET 9+ applications.  
This guide will walk you through everything you need to install, configure, and start dispatching commands, queries, and notifications.

---

## 📦 1. Install the Core Package

Install the main NuGet package:

```bash
dotnet add package Orchestrix.Mediator
```

Optional extensions:

```bash
# For source-generator-based dispatch (zero reflection)
dotnet add package Orchestrix.Mediator.SourceGenerators

# For CQRS-style ICommand / IQuery support
dotnet add package Orchestrix.Mediator.Cqrs
```

---

## 🛠 2. Registering Orchestrix.Mediator

In your `Program.cs`register Orchestrix.Mediator:

```csharp
services.AddOrchestrix(cfg => cfg.RegisterHandlersFromAssemblies(typeof(SomeHandler).Assembly));
```

Enable source generator dispatch (optional):

```csharp
services.AddOrchestrix(cfg => 
{
    cfg.UseSourceGenerator() // add this line
    .RegisterHandlersFromAssemblies(typeof(SomeHandler).Assembly);
});
```

**Modes:**

- `MediatorMode.Reflection` (default) — uses reflection for dispatch
- `MediatorMode.SourceGenerator` — uses compile-time dispatch for performance

---

## 🧾 3. Creating a Request and Handler

Define a command implementing `IRequest<T>`:

```csharp
public record CreateUserCommand(string Name, string Email) : IRequest<string>;
```

Handle it using `IRequestHandler<TRequest, TResponse>`:

```csharp
public class CreateUserHandler : IRequestHandler<CreateUserCommand, string>
{
    public ValueTask<string> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Your logic here
        return ValueTask.FromResult($"User {request.Name} created.");
    }
}
```

### Void Requests

Use `IRequest` for commands without a return value (void):

```csharp
public class Ping : IRequest;

public class PingHandler : IRequestHandler<Ping>
{
    public ValueTask Handle(Ping request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Pong");
        return default;
    }
}
```

---

## 📢 4. Publishing Notifications

Define a notification:

```csharp
public record UserCreated(string Email) : INotification;
```

Handle it **sequentially**:

```csharp
public class LogUserNotification : INotificationHandler<UserCreated>
{
    public ValueTask Handle(UserCreated notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[LOG] User created: {notification.Email}");
        return default;
    }
}
```

Or **in parallel**:

```csharp
public class SendEmailNotification : IParallelNotificationHandler<UserCreated>
{
    public async ValueTask Handle(UserCreated notification, CancellationToken cancellationToken)
    {
        await Task.Delay(300); // Simulate async work
        Console.WriteLine($"[EMAIL] Welcome email sent to {notification.Email}");
    }
}
```

---

## 🚚 5. Dispatching Requests & Notifications

Inject `ISender` and `IPublisher` anywhere (Minimal API, Controllers, Services, etc.):
> Note that: you can inject `IMediator` where it include both `ISender` and `IPublisher`

```csharp
public class UsersController(ISender sender) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> Create(CreateUserCommand command)
    {
        var result = await sender.Send(command);
        return Ok(result);
    }
}
```

To publish a notification manually:

```csharp
await publisher.Publish(new UserCreated("email@test.com"));
```

---

## 🌐 6. Using in Minimal API

```csharp
app.MapPost("/users", async ([FromServices] ISender sender, CreateUserCommand command) =>
{
    var result = await sender.Send(command);
    return Results.Ok(result);
});
```

---

## 🧪 7. Writing Tests

You can easily mock `ISender` and `IPublisher` in unit tests:

```csharp
var sender = new Mock<ISender>();
sender.Setup(s => s.Send(It.IsAny<IRequest<string>>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync("test");

var result = await sender.Object.Send(new CreateUserCommand("Test", "email@test.com"));
```

---

## 🔁 8. Next Steps

✅ Explore the next guides:

- [Core Concepts →](./02-CoreConcepts.md)
- [CQRS Guide →](./03-CqrsGuide.md)
- [Source Generators →](./04-SourceGenerators.md)
- [Hooks & Pipelines →](./05-HooksAndPipelines.md)
- [API Reference →](./99-ApiReference.md)

💬 Need help? Found an issue? Open a GitHub issue and tag [@anzawi](https://github.com/anzawi)
