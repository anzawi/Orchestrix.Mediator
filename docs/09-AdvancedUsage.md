# 🧙 Advanced Usage in Orchestrix.Mediator

Beyond the basics, Orchestrix.Mediator provides a suite of **advanced features and extensibility options** to help you build resilient, performant, and flexible applications.

---

## 🧪 1. TrySend / TryPublish

### ✅ TrySend

Instead of throwing when no handler exists, use:

```csharp
bool success = await sender.TrySend(new SomeCommand());
```

Returns:

- `true` if the command was handled
- `false` if no matching handler was found

### ✅ TryPublish

Similarly, publish a notification only if handlers are available:

```csharp
bool published = await publisher.TryPublish(new SomeNotification());
```

---

## 🔀 2. Dispatching By Object (Non-Generic)

All dispatch methods have **non-generic overloads**:

```csharp
ValueTask<object?> Send(object request);
ValueTask Publish(object notification);
IAsyncEnumerable<object?> CreateStream(object request);
```

These are useful when:

- You receive dynamic request types at runtime (e.g., reflection, plugin systems)
- You're working with deserialized DTOs

> ⚠️ Note: These overloads **require reflection** — even in source-generator mode.

---

## 🔧 3. Customizing Orchestrix.Mediator Options

```csharp
services.AddOrchestrix(cfg =>
{
    cfg.UseSourceGenerator(); // Make sure  Orchestrix.Mediator.SourceGenerator installed
});
```

Combine this with hook configuration:

```csharp
services.AddOrchestrix(cfg =>
{
    cfg.UseSourceGenerator(); // Make sure  Orchestrix.Mediator.SourceGenerator installed
    cfg.AddHooksFromAssemblies(typeof(MyHook).Assembly); // or use AddHook<MyHook>()
});
```

---

## 🧱 4. Resolving IMediator / ISender / IPublisher

Choose the abstraction based on responsibility:

| Interface   | Purpose                                     |
|-------------|---------------------------------------------|
| `ISender`   | Sends requests (`Send`, `TrySend`, `Stream`)|
| `IPublisher`| Publishes notifications                     |
| `IMediator` | Combines both                               |

✅ For unit testing: mock `ISender` or `IPublisher` individually  
✅ For end-to-end testing: use `IMediator`

---

## 🧩 5. Manually Using `IOrchestrixDispatcher` (Optional, Not Recommended)

If needed, you can inject and use the **source-generated dispatcher** directly:

```csharp
var dispatcher = provider.GetRequiredService<IOrchestrixDispatcher>();

await dispatcher.Dispatch(new SomeQuery());
await dispatcher.DispatchPublish(new SomeNotification());

await foreach (var item in dispatcher.DispatchStream(new SomeStream()))
{
    Console.WriteLine(item);
}
```

> ⚠️ Only available when `MediatorMode.SourceGenerator` is enabled and the generator has run successfully.
> ⚠️ Not recommended to use `IOrchestrixDispatcher` directly its already used internally with IMediator, ISender and IPublisher

---

## 🧪 5.1 Testing with Source Generator

You can mock the dispatcher interface in tests:

```csharp
var dispatcher = new Mock<IOrchestrixDispatcher>();
dispatcher.Setup(d => d.Dispatch(It.IsAny<IRequest<string>>(), ..., ...)).ReturnsAsync("test");

var result = await dispatcher.Object.Dispatch(new SomeQuery());
```

---

## 🧰 6. Using with Minimal API and Controllers

**Minimal API:**

```csharp
app.MapPost("/users", async (ISender sender, CreateUserCommand cmd) =>
{
    var result = await sender.Send(cmd);
    return Results.Ok(result);
});
```

**MVC Controller:**

```csharp
public class MyController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> DoSomething(SomeCommand command)
    {
        var result = await mediator.Send(command);
        return Ok(result);
    }
}
```

---

## 🧠 7. Handling Multiple Handlers

| Message Type     | Multiple Handlers | Behavior                          |
|------------------|-------------------|-----------------------------------|
| `INotification`  | ✅ Supported       | Fan-out (sequential / parallel)   |
| `IRequest<T>`    | ❌ Not allowed     | Throws if more than one found     |

✅ This aligns with **CQRS intent** and avoids ambiguity.

---

## 🧬 8. Combining Pipelines and Hooks

You can use **both** simultaneously:

- `IPipelineBehavior<T, R>` → for cross-cutting behaviors (e.g., validation)
- `ISendHook`, `IPublishHook`, `IStreamHook` → for diagnostics, tracing

They **do not conflict**:

- Pipelines wrap the handler execution
- Hooks wrap the lifecycle before/after the pipeline

---

## 🚫 9. Disallowed or Unsupported Scenarios

| Scenario                                     | Support   |
|---------------------------------------------|-----------|
| Multiple `IRequestHandler<T>` per request   | ❌ Not allowed |
| Dynamic handler resolution without DI       | ❌ Not supported |
| Cancellation tokens in pipeline             | ✅ Supported     |
| Late dispatcher registration in SourceGen   | ❌ Will throw    |

---

## 🧭 Summary

| Feature                     | Description                                      |
|-----------------------------|--------------------------------------------------|
| `TrySend`, `TryPublish`     | Fire-safely without throwing                    |
| Object dispatching          | Useful for dynamic runtime scenarios            |
| `IMediator`, `ISender`, `IPublisher` | Clean separation of concerns        |
| `IOrchestrixDispatcher`      | Direct access to source-gen dispatcher          |
| Minimal API & MVC           | ✅ Fully supported                              |
| Multi-handler safety        | ✅ Notifications / ❌ Requests                  |
| Combined hooks + pipelines  | ✅ Best practice                                |
