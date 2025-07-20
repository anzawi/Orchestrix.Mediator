using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Mediator.Core.Execution;

/// <summary>
/// Provides the execution mechanism for processing a request through a pipeline of behaviors
/// and the associated handler. This class is specifically designed for requests that
/// return a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being executed, which must implement <see cref="IRequest{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned after processing the request.
/// </typeparam>
internal sealed class PipelineExecutor<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Executes the pipeline for the given request using the provided dependency injection container and cancellation token.
    /// </summary>
    /// <param name="request">The request to be processed by the pipeline.</param>
    /// <param name="provider">The service provider used to resolve dependencies such as the request handler and pipeline behaviors.</param>
    /// <param name="ct">A cancellation token that can be used to cancel the pipeline execution.</param>
    /// <returns>A task that represents the asynchronous operation, containing the response produced by the pipeline.</returns>
    public ValueTask<TResponse> Execute(TRequest request, IServiceProvider provider, CancellationToken ct)
    {
        var handler = provider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var behaviors = provider.GetServices<IPipelineBehavior<TRequest, TResponse>>();

        var pipelineBehaviors = behaviors as IPipelineBehavior<TRequest, TResponse>[] ?? behaviors.ToArray();
        if (!pipelineBehaviors.Any())
            return handler.Handle(request, ct);

        var pipeline = PipelineBuilder.BuildWithResult(request, handler.Handle, pipelineBehaviors);
        return pipeline(ct);
    }
}

/// <summary>
/// Represents an executor responsible for handling requests that do not produce a response.
/// It integrates pipeline behaviors and the request handler to execute the request within
/// the defined mediator pipeline.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being executed.
/// Must implement <see cref="IRequest"/>.
/// </typeparam>
internal sealed class PipelineExecutorVoid<TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Executes a pipeline for the given request including any configured pipeline behaviors and the final handler.
    /// </summary>
    /// <param name="request">The request object to process.</param>
    /// <param name="provider">An IServiceProvider instance used to resolve dependencies.</param>
    /// <param name="ct">A CancellationToken to observe while waiting for the operation to complete.</param>
    /// <returns>A ValueTask that represents the asynchronous operation.</returns>
    public async ValueTask Execute(TRequest request, IServiceProvider provider, CancellationToken ct)
    {
        var handler = provider.GetRequiredService<IRequestHandler<TRequest>>();
        var behaviors = provider.GetServices<IPipelineBehavior<TRequest>>();

        var pipelineBehaviors = behaviors as IPipelineBehavior<TRequest>[] ?? behaviors.ToArray();

        if (!pipelineBehaviors.Any())
        {
            await handler.Handle(request, ct);
            return;
        }

        var pipeline = PipelineBuilder.BuildVoid(request, handler.Handle, pipelineBehaviors);
        await pipeline(ct);
    }
}