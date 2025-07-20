# ✨ Source Generators (Optional)

Orchestrix.Mediator includes an optional **source generator** that eliminates runtime reflection and provides zero-cost dispatching — **without requiring any changes** to your application code.

---

## 🧬 Why Use Source Generators?

By default, Orchestrix.Mediator uses reflection to resolve handlers:

```csharp
services.AddOrchestrix(...); // MediatorMode.Reflection
```

This works great — but in performance-critical or AOT scenarios, you can switch to compile-time dispatching for:

✅ Type-safe dispatching  
⚡ No `IServiceProvider.GetService` calls at runtime  
📦 Native AOT friendliness  
💡 Improved startup and throughput performance

---

## 📦 Installing the Generator

```bash
dotnet add package Orchestrix.Mediator.SourceGenerators
```

---

## ⚙️ Enable Generator Mode

Switch the mediator mode during registration:

```csharp
services.AddOrchestrix(cfg => 
{
    cfg.UseSourceGenerator() // add this line
    .RegisterHandlersFromAssemblies(typeof(SomeHandler).Assembly);
});
```

Orchestrix.Mediator will automatically resolve a generated `GeneratedDispatcher`  
and use `DispatcherMediator` as the internal `IMediator` implementation.

---

## 🧠 Generated Dispatcher

The source generator emits a class like:

```csharp
internal sealed class GeneratedDispatcher : IOrchestrixDispatcher
```

It supports the following methods:

- `Dispatch<TResponse>(IRequest<TResponse>)`
- `DispatchVoid(IRequest)`
- `Dispatch(object)`
- `DispatchPublish<TNotification>(TNotification)`
- `DispatchStream<T>(IStreamRequest<T>)`
- `TryDispatch(...)` variants

This is resolved behind the scenes — **no need to reference it directly.**

---

## ✅ Usage is the Same

You can keep using `ISender` or `IMediator` **even with source generators enabled**:

```csharp
var id = await sender.Send(new CreateUserCommand("Mohammad"));
```

Or:

```csharp
var id = await mediator.Send(new CreateUserCommand("Mohammad"));
```

✅ **No special APIs** are required to benefit from the generator — just configure it via `MediatorMode.SourceGenerator`.

---

## ⚠️ Reflection in Object-Based Dispatch

If you use non-generic overloads like:

```csharp
await sender.Send(someRequest as object);

```
These will still use reflection, even when MediatorMode.SourceGenerator is enabled.
This is necessary because type information is erased at runtime.


---


## 💥 Fallback Behavior

If source generator mode is enabled but no `GeneratedDispatcher` is found, Orchestrix.Mediator will throw:

```plaintext
InvalidOperationException: Orchestrix.Mediator: Source-Generator mode requires the Orchestrix.Mediator.SourceGenerators package to be installed, and a valid GeneratedDispatcher must be generated.
```

Ensure that:

- ✅ You have at least one request + handler defined
- ✅ You **build** the project once
- ✅ The generator runs successfully
- ✅ Your handlers must be in **referenced assemblies** passed to `.RegisterHandlersFromAssemblies(...)`

---

## 🛠 Development Tips

- 🧼 Always **clean + rebuild** after installing the generator
- ✅ Ensure your handlers are in a **referenced assembly**
- 🔧 Generator uses **Roslyn 4.14+** and modern **C# 12+** features

---

## 🧪 Testability

Source-generated dispatch is fully testable using `ISender` or `IMediator`.  
When enabled, **no reflection is used** to locate handlers.
> The reflection used for Stream handlers only.

---

## 🧩 Summary

| Feature            | Reflection     | Source Generator    |
|--------------------|----------------|----------------------|
| Handler resolution | At runtime     | At compile-time      |
| Requires DI        | ✅ Yes         | ✅ Yes               |
| Startup cost       | Slightly higher| Lower                |
| AOT-safe           | ⚠️ Risky       | ✅ Safe              |
| Usage change       | ❌ None        | ❌ None              |
| Performance        | Good           | **Best**             |
