namespace Orchestrix.Mediator.Core.Execution;

/// <summary>
/// A static utility class for building request processing pipelines used by Orchestrix.Mediator.
/// </summary>
/// <remarks>
/// The PipelineBuilder class facilitates the construction of execution pipelines for handling requests
/// within the Orchestrix.Mediator framework. It allows chaining of custom pipeline behaviors, culminating at a terminal handler.
/// </remarks>
internal static class PipelineBuilder
{
    /// <summary>
    /// Constructs a pipeline of behaviors and a terminal delegate for handling a request with a response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request to be passed through the pipeline.</param>
    /// <param name="terminal">
    /// The terminal delegate that represents the final handler for the request,
    /// invoked after all behaviors in the pipeline.
    /// </param>
    /// <param name="behaviors">A collection of pipeline behaviors to be executed in order, wrapping the terminal.</param>
    /// <returns>
    /// A delegate that represents the composed pipeline, which processes the request through all defined behaviors
    /// and finally the terminal.
    /// </returns>
    public static RequestHandlerDelegate<TResponse> BuildWithResult<TRequest, TResponse>(
        TRequest request,
        Func<TRequest, CancellationToken, ValueTask<TResponse>> terminal,
        IEnumerable<IPipelineBehavior<TRequest, TResponse>> behaviors)
        where TRequest : IRequest<TResponse>
    {
        RequestHandlerDelegate<TResponse> next = ct => terminal(request, ct);

        foreach (var behavior in behaviors.Reverse())
        {
            var copy = next;
            next = ct => behavior.Handle(request, copy, ct);
        }

        return next;
    }

    /// <summary>
    /// Constructs a delegate pipeline for void-returning request handlers by chaining
    /// multiple pipeline behaviors and a terminal handler.
    /// </summary>
    /// <typeparam name="TRequest">The type of request being handled, which must implement <see cref="IRequest"/>.</typeparam>
    /// <param name="request">The request instance being processed.</param>
    /// <param name="terminal">The terminal delegate representing the final handler for the request.</param>
    /// <param name="behaviors">A collection of pipeline behaviors to process the request.</param>
    /// <returns>
    /// A delegate of type <see cref="RequestHandlerDelegate{VoidMarker}"/> that represents the
    /// composed execution pipeline for handling the request, with behaviors executed in reverse order added.
    /// </returns>
    public static RequestHandlerDelegate<VoidMarker> BuildVoid<TRequest>(
        TRequest request,
        Func<TRequest, CancellationToken, ValueTask> terminal,
        IEnumerable<IPipelineBehavior<TRequest>> behaviors)
        where TRequest : IRequest
    {
        RequestHandlerDelegate<VoidMarker> next = async ct =>
        {
            await terminal(request, ct);
            return default;
        };

        foreach (var behavior in behaviors.Reverse())
        {
            var copy = next;
            next = ct => behavior.Handle(request, copy, ct);
        }

        return next;
    }
}