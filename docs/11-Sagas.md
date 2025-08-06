# 🌀 Orchestrix.Mediator.Sagas

Fluent, in-process saga orchestration engine for Orchestrix.Mediator — with retries, compensation, timeouts, hooks, and decorators.

---

## 📦 Installation

Add the NuGet package:

```csharp
dotnet add package Orchestrix.Mediator.Sagas
```

---

## ✨ Why In-Process Sagas?

In-process sagas allow you to coordinate complex business operations made of multiple steps — each with its own error handling, rollback logic, and timing constraints.

Typical use cases:
- Order fulfillment
- Multi-stage user registration
- Resource provisioning
- Long-running workflows with rollback

---

## ⚙️ Setup

Register your sagas during Orchestrix setup:

```csharp
services.AddOrchestrix(cfg => cfg
    .AddSagas(typeof(MySaga).Assembly)
    .AddSagaHook<LoggingSagaHook>()
    .AddSagaDecorator<MetricsSagaDecorator>());
```

This does the following:
- Scans the provided assembly for:
    - `SagaBuilder<TInput>`
    - `ISagaStep<TInput>`
    - `ISagaCompensationStep<TInput>`
- Registers the saga coordinator
- Optionally registers lifecycle hooks and decorators

---

## 🧱 Define Saga Steps

Create steps that implement:

```csharp
public interface ISagaStep<TInput>
{
    ValueTask ExecuteAsync(TInput input, ISender sender, CancellationToken ct);
}
```

Optional rollback logic:

```csharp
public interface ISagaCompensationStep<TInput>
{
    ValueTask CompensateAsync(TInput input, ISender sender, CancellationToken ct);
}
```

---

## 🛠 Build the Saga

Extend `SagaBuilder<TInput>` to define the saga flow:

```csharp
public sealed class PlaceOrderSaga : SagaBuilder<PlaceOrderContext>
{
    protected override void Build(SagaStepBuilder<PlaceOrderContext> saga)
    {
        saga.Step<ValidateCustomerStep>()
             .Retry(3)
             .Compensate<UndoValidateCustomerStep>();

        saga.Step<ReserveInventoryStep>()
             .Timeout(TimeSpan.FromSeconds(5));
    }
}
```

### Step Configuration Options

- `.Retry(maxAttempts)`
- `.Timeout(duration)`
- `.Compensate<TStep>()`

---

## 🚀 Execute a Saga

Use `ISagaCoordinator`:

```csharp
await coordinator.ExecuteAsync(new PlaceOrderContext(...));
```

The correct saga will be resolved based on the input type.

---

## 🪝 Lifecycle Hooks

Implement `ISagaHook` to observe saga execution:

```csharp
public interface ISagaHook
{
    ValueTask OnSagaStart(object input, CancellationToken ct);
    ValueTask OnSagaComplete(object input, CancellationToken ct);
    ValueTask OnSagaError(object input, Exception ex, CancellationToken ct);
}
```

Register your hook:

```csharp
services.AddSagaHook<LoggingSagaHook>();
```

---

## 🧩 Decorators

Use `ISagaDecorator` to wrap execution with logging, metrics, etc.

```csharp
public interface ISagaDecorator
{
    ValueTask InvokeAsync<TInput>(
        TInput input,
        Func<TInput, CancellationToken, ValueTask> next,
        CancellationToken ct
    ) where TInput : notnull;
}
```

Register your decorator:

```csharp
services.AddSagaDecorator<MetricsSagaDecorator>();
```

---

## 🧪 Full Example

```csharp
public sealed record PlaceOrderContext(Guid OrderId, Guid CustomerId);

public class ValidateCustomerStep : ISagaStep<PlaceOrderContext>
{
    public ValueTask ExecuteAsync(PlaceOrderContext input, ISender sender, CancellationToken ct)
    {
        Console.WriteLine($"Validating customer {input.CustomerId}");
        return default;
    }
}

public class UndoValidateCustomerStep : ISagaCompensationStep<PlaceOrderContext>
{
    public ValueTask CompensateAsync(PlaceOrderContext input, ISender sender, CancellationToken ct)
    {
        Console.WriteLine($"Undo validation for {input.CustomerId}");
        return default;
    }
}

public sealed class PlaceOrderSaga : SagaBuilder<PlaceOrderContext>
{
    protected override void Build(SagaStepBuilder<PlaceOrderContext> saga)
    {
        saga.Step<ValidateCustomerStep>()
             .Retry(2)
             .Compensate<UndoValidateCustomerStep>();
    }
}
```

---

## 🧠 Best Practices

| Concern               | Recommendation                              |
|------------------------|----------------------------------------------|
| Missing handlers       | Always call `.AddSagas(...)` on startup      |
| Hooks and decorators   | Opt-in to monitor and enrich execution       |
| Step registration      | Automatically handled via assembly scanning |

---
