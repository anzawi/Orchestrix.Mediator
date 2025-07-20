# 📖 Orchestrix.Mediator API Reference

This document provides a categorized list of all core interfaces, types, and contracts available in **Orchestrix.Mediator**, including optional extensions:

- ✅ CQRS
- 📡 Streaming
- 🪝 Hooks
- 🧬 Source Generator Support

---

## 📦 Core Interfaces

| Interface   | Description                                  |
|-------------|----------------------------------------------|
| `IMediator` | Combines `ISender` and `IPublisher`          |
| `ISender`   | Handles `Send(...)`, `TrySend(...)`, `CreateStream(...)` |
| `IPublisher`| Handles `Publish(...)`, `TryPublish(...)`    |

---

## 🧾 Request Interfaces

| Interface                              | Description                                 |
|----------------------------------------|---------------------------------------------|
| `IRequest`                             | Non-generic command without return          |
| `IRequest<T>`                          | Generic request/command that returns `T`    |
| `IRequestHandler<T>`                   | Handles a non-generic `IRequest`            |
| `IRequestHandler<TRequest, TResponse>` | Handles a generic `IRequest<TResponse>`     |

---

## 📢 Notification Interfaces

| Interface                             | Description                           |
|---------------------------------------|---------------------------------------|
| `INotification`                       | Marker interface for events           |
| `INotificationHandler<T>`            | Sequential handler                    |
| `IParallelNotificationHandler<T>`    | Parallel handler (fan-out)            |

---

## 📡 Streaming Interfaces

| Interface                                     | Description                             |
|----------------------------------------------|-----------------------------------------|
| `IStreamRequest<T>`                          | Request for `IAsyncEnumerable<T>`       |
| `IStreamRequestHandler<TRequest, TResponse>` | Handles stream requests                 |
| `ISender.CreateStream(...)`                  | Dispatches streaming requests           |

---

## 🪝 Hook Interfaces

| Interface        | Description                          |
|------------------|--------------------------------------|
| `ISendHook`      | Hooks into `Send(...)` lifecycle     |
| `IPublishHook`   | Hooks into `Publish(...)` lifecycle  |
| `IStreamHook`    | Hooks into streaming lifecycle       |
| `IHookExecutor`  | Internal executor for registered hooks|

### Hook Method Summary

| Method                             | Trigger         |
|------------------------------------|------------------|
| `OnSendStart`, `OnPublishStart`, `OnStreamStart`     | Before execution  |
| `OnSendComplete`, `OnPublishComplete`                | On success         |
| `OnSendError`, `OnPublishError`, `OnStreamError`     | On failure         |

---

## ⚙️ Pipelines

| Interface                               | Description                                 |
|-----------------------------------------|---------------------------------------------|
| `IPipelineBehavior<TRequest, TResponse>`| Middleware wrapper around request execution |
| `RequestHandlerDelegate<T>`             | Delegate to call next behavior or handler   |


---

## 🧩 CQRS Extension (Orchestrix.Mediator.Cqrs)

| Interface                     | Inherits          | Description                         |
|-------------------------------|-------------------|-------------------------------------|
| `ICommand`                    | `IRequest`        | Command without return              |
| `ICommand<T>`                 | `IRequest<T>`     | Command with return                 |
| `IQuery<T>`                   | `IRequest<T>`     | Query with return                   |
| `ICommandHandler<T>`          | `IRequestHandler<T>`      | Handles void-return command   |
| `ICommandHandler<T, R>`       | `IRequestHandler<T, R>`   | Handles command with result   |
| `IQueryHandler<T, R>`         | `IRequestHandler<T, R>`   | Handles query                |

---

## 🧬 Source Generator Output (Orchestrix.Mediator.SourceGenerators)

| Interface              | Description                                 |
|------------------------|---------------------------------------------|
| `IOrchestrixDispatcher` | Generated interface for static dispatching  |

### Dispatcher Methods
> No need to use it directly.

| Method                                         | Description                          |
|------------------------------------------------|--------------------------------------|
| `Dispatch<TResponse>(IRequest<TResponse>)`     | Dispatches generic request           |
| `DispatchVoid(IRequest)`                       | Dispatches void-returning request    |
| `Dispatch(object)`                             | Dispatches dynamic request           |
| `DispatchPublish<T>(T notification)`           | Publishes a notification             |
| `DispatchStream<T>(IStreamRequest<T>)`         | Streams response from handler        |
| `TryDispatch(...)`                             | Safe variants with fallback behavior |

---

## 🧪 Try Variants

| Method             | Description                            |
|--------------------|----------------------------------------|
| `TrySend(object)`  | Returns `false` if no handler exists   |
| `TryPublish(object)` | Returns `false` if no handlers exist |

---

## 📚 Summary

| Area               | Key Interfaces / Types                                                 |
|--------------------|------------------------------------------------------------------------|
| Core Dispatch      | `IMediator`, `ISender`, `IPublisher`                                   |
| Requests           | `IRequest`, `IRequest<T>`, `IRequestHandler<>`                         |
| Notifications      | `INotification`, `INotificationHandler<>`, `IParallelNotificationHandler<>` |
| Streaming          | `IStreamRequest<T>`, `IStreamRequestHandler<>`                         |
| Pipelines          | `IPipelineBehavior<>`, `RequestHandlerDelegate<>`                      |
| Hooks              | `ISendHook`, `IPublishHook`, `IStreamHook`                             |
| CQRS               | `ICommand`, `IQuery`, `ICommandHandler`, `IQueryHandler`               |
| SourceGen          | `IOrchestrixDispatcher`                                                 |
