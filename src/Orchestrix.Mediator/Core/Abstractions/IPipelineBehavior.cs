// ReSharper disable once CheckNamespace

namespace Orchestrix.Mediator;

/// <summary>
/// Represents a behavior in the MediatR pipeline that is executed before or after the request handler.
/// This can be used to implement cross-cutting concerns such as logging, validation, or performance monitoring.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request object. It must implement <see cref="IRequest{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response object returned by the request handler.
/// </typeparam>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Processes a request within a pipeline by invoking the next delegate or performing additional operations.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being processed.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned after processing the request.</typeparam>
    /// <param name="request">The request to process within the pipeline.</param>
    /// <param name="next">The next delegate to invoke within the processing pipeline.</param>
    /// <param name="cancellationToken">A token to notify request cancellation.</param>
    /// <returns>A task representing asynchronous operation, which, when completed, contains the response of type <typeparamref name="TResponse"/>.</returns>
    ValueTask<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}

public interface IPipelineBehavior<in TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Handles the execution of a request through the pipeline by invoking the associated behaviors
    /// and a terminal delegate when applicable.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being handled.</typeparam>
    /// <param name="request">The request object being processed through the pipeline.</param>
    /// <param name="next">
    /// The delegate representing the next action in the pipeline sequence,
    /// which either proceeds to the next behavior or executes the terminal handler.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used for propagating notifications that the operation should be canceled.
    /// </param>
    /// <returns>A <see cref="ValueTask{TResponse}"/> representing the asynchronous response produced by the pipeline.</returns>
    ValueTask<VoidMarker> Handle(
        TRequest request,
        RequestHandlerDelegate<VoidMarker> next,
        CancellationToken cancellationToken);
}

/// <summary>
/// Represents a delegate that defines the next step in a request processing pipeline, where the delegate executes
/// asynchronously and ultimately produces a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response that will be produced after executing the request.
/// </typeparam>
/// <param name="cancellationToken">
/// A <see cref="CancellationToken"/> that can be used to observe cancellation requests.
/// </param>
/// <returns>
/// A <see cref="ValueTask{TResult}"/> representing the asynchronous operation and encapsulating the response of
/// type <typeparamref name="TResponse"/> produced by the delegate.
/// </returns>
public delegate ValueTask<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken cancellationToken = default);
