using Orchestrix.Mediator.Core.Execution.Interfaces;
using Orchestrix.Mediator.Diagnostics;
using Orchestrix.Mediator.Diagnostics.Hooks;

namespace Orchestrix.Mediator.Core.Execution;

/// <inheritdoc />
internal class HookExecutor(
    ISendHook? sendHook = null,
    IPublishHook? publishHook = null,
    IStreamHook? streamHook = null,
    HookConfiguration? hookConfig = null) : IHookExecutor
{
    /// <summary>
    /// Represents the configuration for hook execution within the <see cref="HookExecutor"/>.
    /// This variable determines the behavior of hooks, such as whether specific hooks
    /// (e.g., send, publish, stream) are enabled and whether errors encountered during
    /// hook execution should propagate.
    /// </summary>
    private readonly HookConfiguration _hookConfig = hookConfig ?? new HookConfiguration();

    /// <inheritdoc />
    public async ValueTask<T> ExecuteWithSendHooks<T>(
        object request,
        Func<CancellationToken, ValueTask<T>> execute,
        CancellationToken ct)
    {
        var enableHook = sendHook is not null && _hookConfig.EnableSendHooks;
        if (enableHook) await sendHook!.OnSendStart(request, ct)!;

        try
        {
            var result = await execute(ct);

            if (enableHook) await sendHook!.OnSendComplete(request, result, ct)!;
            return result;
        }
        catch (Exception ex)
        {
            if (enableHook)
            {
                await sendHook!.OnSendError(request, ex, ct)!;
                if (_hookConfig.ThrowOnHookError)
                {
                    throw;
                }
            }
            else
            {
                throw;
            }
        }

        return default!;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteWithPublishHooks(
        object notification,
        Func<CancellationToken, ValueTask> execute,
        CancellationToken ct)
    {
        var enableHook = publishHook is not null && _hookConfig.EnablePublishHooks;
        if (enableHook)
            await publishHook!.OnPublishStart(notification, ct)!;

        try
        {
            await execute(ct);

            if (enableHook)
                await publishHook!.OnPublishComplete(notification, ct)!;
        }
        catch (Exception ex)
        {
            if (enableHook)
            {
                await publishHook!.OnPublishError(notification, ex, ct)!;

                if (_hookConfig.ThrowOnHookError)
                {
                    throw;
                }
            }
            else
            {
                throw;
            }
        }
    }
}