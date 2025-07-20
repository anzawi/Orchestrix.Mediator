using Orchestrix.Mediator.Core.Execution.Interfaces;
using Orchestrix.Mediator.Extensions;

namespace Orchestrix.Mediator.Core.Execution;

/// <inheritdoc />
internal sealed class DispatcherMediator(
    IServiceProvider provider,
    IHookExecutor hookExecutor,
    IOrchestrixDispatcher dispatcher
) : IMediator
{
    /// <inheritdoc />
    public async ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        return await hookExecutor.ExecuteWithSendHooks(
            request,
            ct => dispatcher.Dispatch<TResponse>(request, provider, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask Send(IRequest request, CancellationToken cancellationToken = default)
    {
        await hookExecutor.ExecuteWithSendHooks(
            request,
            ct => dispatcher.DispatchVoid(request, provider, ct).AsTask().ContinueWith(_ => new VoidMarker(), ct)
                .WrapAsValueTask(),
            cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        return await hookExecutor.ExecuteWithSendHooks(
            request,
            ct => dispatcher.Dispatch(request, provider, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        return dispatcher.DispatchStream(request, provider, cancellationToken);
    }

    /// <inheritdoc />
    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        return dispatcher.DispatchStream(request, provider, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<bool> TrySend(object request, CancellationToken cancellationToken = default)
    {
        try
        {
            await Send(request, cancellationToken).AsTask().ContinueWith(_ => true, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask Publish(object notification, CancellationToken cancellationToken = default)
    {
        await hookExecutor.ExecuteWithPublishHooks(
            notification,
            ct => dispatcher.DispatchPublish(notification, provider, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask Publish<TNotification>(TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        await hookExecutor.ExecuteWithPublishHooks(
            notification,
            ct => dispatcher.DispatchPublish(notification, provider, ct),
            cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<bool> TryPublish(object notification, CancellationToken cancellationToken = default)
    {
        try
        {
            await Publish(notification, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}