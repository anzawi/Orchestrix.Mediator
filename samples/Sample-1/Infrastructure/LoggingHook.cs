using Orchestrix.Mediator.Diagnostics.Hooks;

namespace Sample_1.Infrastructure;

public class LoggingHook : ISendHook, IPublishHook, IStreamHook
{
    public ValueTask OnSendStart(object request, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] Send → {request.GetType().Name}");
        return default;
    }

    public ValueTask OnSendComplete(object request, object? response, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] Send ✅ ← {response?.GetType().Name ?? "void"}");
        return default;
    }

    public ValueTask OnSendError(object request, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] Send ❌ {ex.Message}");
        return default;
    }

    public ValueTask OnPublishStart(object notification, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] Publish → {notification.GetType().Name}");
        return default;
    }

    public ValueTask OnPublishComplete(object notification, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] Publish ✅ ← {notification.GetType().Name}");
        return default;
    }

    public ValueTask OnPublishError(object notification, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] Publish ❌ {ex.Message}");
        return default;
    }

    public ValueTask OnStreamStart(object request, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] Stream → {request.GetType().Name}");
        return default;
    }

    public ValueTask OnStreamError(object request, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] Stream ❌ {ex.Message}");
        return default;
    }
}
