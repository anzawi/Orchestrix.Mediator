using Orchestrix.Mediator.Diagnostics.Hooks;

namespace Sample_2.SourceGenerator.Infrastructure;


public class LoggingHook : ISendHook, IPublishHook, IStreamHook
{
    public ValueTask OnSendStart(object request, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] → Sending {request.GetType().Name}");
        return default;
    }

    public ValueTask OnSendComplete(object request, object? response, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] ✅ Send complete: {response?.ToString() ?? "void"}");
        return default;
    }

    public ValueTask OnSendError(object request, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] ❌ Send error: {ex.Message}");
        return default;
    }

    public ValueTask OnPublishStart(object notification, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] → Publishing {notification.GetType().Name}");
        return default;
    }

    public ValueTask OnPublishComplete(object notification, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] ✅ Publish complete: {notification.GetType().Name}");
        return default;
    }

    public ValueTask OnPublishError(object notification, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] ❌ Publish error: {ex.Message}");
        return default;
    }

    public ValueTask OnStreamStart(object request, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] → Streaming {request.GetType().Name}");
        return default;
    }

    public ValueTask OnStreamError(object request, Exception ex, CancellationToken ct)
    {
        Console.WriteLine($"[HOOK] ❌ Stream error: {ex.Message}");
        return default;
    }
}