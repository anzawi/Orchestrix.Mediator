using Orchestrix.Mediator.Core.Execution.Interfaces;

namespace Orchestrix.Mediator.Core.Execution;

/// <inheritdoc />
internal sealed class HandlerInvokerWrapper<TRequest, TResponse> : IHandlerInvoker<TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Static instance of the <see cref="PipelineExecutor{TRequest, TResponse}"/> used to execute
    /// a specific request and response pipeline workflow for a given request type and response type.
    /// </summary>
    /// <typeparam name="TRequest">
    /// The type of the request being processed, derived from <see cref="IRequest{TResponse}"/>.
    /// </typeparam>
    /// <typeparam name="TResponse">
    /// The type of the response expected after processing the request.
    /// </typeparam>
    private static readonly PipelineExecutor<TRequest, TResponse> Executor = new();

    /// <inheritdoc />
    public async ValueTask<TResponse> Invoke(IServiceProvider provider, object request, CancellationToken ct)
        => await Executor.Execute((TRequest)request, provider, ct);
}

/// <inheritdoc />
/// <summary>
/// A wrapper class for invoking handlers associated with specific request types in a mediator-based pipeline.
/// </summary>
/// <typeparam name="TRequest">The type of the request being handled.</typeparam>
internal sealed class HandlerInvokerWrapper<TRequest> : IHandlerInvoker<VoidMarker>
    where TRequest : IRequest
{
    /// <summary>
    /// Represents a centralized executor that facilitates the execution of pipeline behaviors
    /// and handler invocation for a specific request type.
    /// </summary>
    /// <remarks>
    /// This variable is responsible for invoking the configured mediator pipeline, ensuring
    /// that all registered behaviors and handlers are executed in the correct order for the provided request.
    /// It acts as the core component to bridge request objects with their respective handlers, allowing
    /// seamless execution within the defined pipeline.
    /// </remarks>
    private static readonly PipelineExecutorVoid<TRequest> Executor = new();

    /// <inheritdoc />
    public async ValueTask<VoidMarker> Invoke(IServiceProvider provider, object request, CancellationToken ct)
    {
        await Executor.Execute((TRequest)request, provider, ct);
        return default;
    }
}