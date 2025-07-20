// ReSharper disable once CheckNamespace

namespace Orchestrix.Mediator;

/// <summary>
/// Defines a handler for processing a specific request type.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request being handled. This must implement <see cref="IRequest{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the handler when processing the request.
/// </typeparam>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles a request and produces a response. This method is the core of the request handling mechanism.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object. It must implement <see cref="IRequest{TResponse}"/>.</typeparam>
    /// <typeparam name="TResponse">The type of the response object returned by the handler.</typeparam>
    /// <param name="request">The request object containing the data for processing.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> instance to propagate notification that the operation should be canceled.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation containing the response.</returns>
    ValueTask<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a contract for handling requests with an optional response in a mediator-based architecture.
/// </summary>
/// <typeparam name="TRequest">The type of the request to handle. Must implement <see cref="IRequest"/>.</typeparam>
public interface IRequestHandler<in TRequest>
    where TRequest : IRequest
{
    /// <summary>
    /// Handles the specified request asynchronously.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being handled.</typeparam>
    /// <param name="request">The request message to process.</param>
    /// <param name="cancellationToken">The cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task representing the completion of the request handling. The task result contains the response.</returns>
    ValueTask Handle(TRequest request, CancellationToken cancellationToken);
}