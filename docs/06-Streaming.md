# 📡 Streaming in Orchestrix.Mediator

Orchestrix.Mediator supports asynchronous streaming requests using the built-in .NET type `IAsyncEnumerable<T>`.  
This is useful when:

- You want to stream results as they become available
- You’re processing large data sets
- You're implementing pagination, batching, or live feeds

---

## 🧾 1. Interfaces for Streaming

| Purpose                    | Interface                              |
|----------------------------|----------------------------------------|
| Marker for streaming       | `IStreamRequest<T>`                    |
| Handler interface          | `IStreamRequestHandler<TRequest, TResponse>` |

Streaming requests are fire-and-forget per item — like yielding from a generator.

---

## 🧱 2. Define a Streaming Request

```csharp
public record GetUsers(int PageSize) : IStreamRequest<UserDto>;
```

---

## 🛠 3. Implement a Streaming Handler

```csharp
public class GetUsersHandler : IStreamRequestHandler<GetUsers, UserDto>
{
    public async IAsyncEnumerable<UserDto> Handle(GetUsers request, [EnumeratorCancellation] CancellationToken ct)
    {
        for (int i = 0; i < request.PageSize; i++)
        {
            yield return new UserDto(Guid.NewGuid(), $"User-{i + 1}");
            await Task.Delay(200, ct); // simulate async work
        }
    }
}
```

> 💡 Use `[EnumeratorCancellation]` on the `CancellationToken` to support proper cancellation.

---

## 🚀 4. Dispatching a Stream

Inject `ISender` or `IMediator` and call `CreateStream<T>`:

```csharp
await foreach (var user in sender.CreateStream(new GetUsers(3), ct))
{
    Console.WriteLine(user.Name);
}
```

You can also use `.CreateStream(object)` if you only have a non-generic reference.

---

## 🧠 5. Hook Support for Streams

If you've registered hooks, any `IStreamHook` implementations will automatically run:

| Method               | When Triggered                    |
|----------------------|-----------------------------------|
| `OnStreamStart(...)` | Before streaming starts           |
| `OnStreamError(...)` | If an exception occurs mid-stream |

---

## ⚖️ 6. Comparison with Send / Publish

| Feature           | Send           | Publish         | Stream                       |
|-------------------|----------------|------------------|-------------------------------|
| One response       | ✅              | ❌               | ❌                            |
| Multiple handlers  | ❌              | ✅               | ❌                            |
| Multiple results   | ❌              | ❌               | ✅ (via yield)                |
| Hookable           | ✅ `ISendHook` | ✅ `IPublishHook`| ✅ `IStreamHook`              |
| Return type        | `TResponse`    | `void`           | `IAsyncEnumerable<T>`        |

---

## 🧪 7. Testability

You can test streaming handlers by consuming the stream as a list:

```csharp
var users = new List<UserDto>();
await foreach (var user in sender.CreateStream(new GetUsers(5)))
{
    users.Add(user);
}

Assert.Equal(5, users.Count);
```

---

## 🧬 8. Source Generator Support

If using `Orchestrix.Mediator.SourceGenerators` with `MediatorMode.SourceGenerator`, the following methods will be generated:

```csharp
IAsyncEnumerable<T> DispatchStream<T>(IStreamRequest<T>)
IAsyncEnumerable<object?> DispatchStream(object)
```

These methods directly invoke your handler — **no reflection required**.

---

## 🧭 Summary

✅ Use `IStreamRequest<T>` for streaming data  
✅ Implement `IStreamRequestHandler<T, R>`  
✅ Dispatch with `ISender.CreateStream(...)`  
✅ Fully supports hooks and cancellation  
✅ Compatible with source generator mode
