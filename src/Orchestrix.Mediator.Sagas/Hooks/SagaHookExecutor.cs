using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator.Sagas.Contracts;

namespace Orchestrix.Mediator.Sagas.Hooks;

internal sealed class SagaHookExecutor(IServiceProvider sp)
{
    private readonly ISagaHook[] _hooks = sp.GetServices<ISagaHook>()?.ToArray() ?? [];

    public async ValueTask OnStartAsync(object input, CancellationToken ct)
    {
        foreach (var hook in _hooks)
        {
            try { await hook.OnSagaStart(input, ct); }
            catch { /* hooks must never throw */ }
        }
    }

    public async ValueTask OnCompleteAsync(object input, CancellationToken ct)
    {
        foreach (var hook in _hooks)
        {
            try { await hook.OnSagaComplete(input, ct); }
            catch { /* silent */ }
        }
    }

    public async ValueTask OnErrorAsync(object input, Exception exception, CancellationToken ct)
    {
        foreach (var hook in _hooks)
        {
            try { await hook.OnSagaError(input, exception, ct); }
            catch { /* silent */ }
        }
    }
}