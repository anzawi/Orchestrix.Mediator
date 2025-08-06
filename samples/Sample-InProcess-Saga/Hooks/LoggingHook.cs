using Orchestrix.Mediator.Sagas.Contracts;

namespace Sample_InProcess_Saga.Hooks;

public sealed class LoggingHook : ISagaHook
{
    public ValueTask OnSagaStart(object? input, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] Starting saga for: {input?.GetType().Name}");
        return default;
    }

    public ValueTask OnSagaComplete(object? input, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] ✅ Completed saga");
        return default;
    }

    public ValueTask OnSagaError(object? input, Exception exception, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] ❌ Failed: {exception.Message}");
        return default;
    }
}