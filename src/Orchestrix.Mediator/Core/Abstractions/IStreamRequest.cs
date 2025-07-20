// ReSharper disable once CheckNamespace

namespace Orchestrix.Mediator;

/// <summary>
/// Represents an interface for a stream request that produces a sequence of responses asynchronously.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response elements returned by the stream.
/// </typeparam>
/// <remarks>
/// This interface is particularly useful for scenarios involving reactive streams, where multiple
/// responses need to be generated over time for a single request. Implementing this interface allows
/// you to define the structure of the request and enable support for asynchronous streaming of results.
/// Used in conjunction with <c>IStreamRequestHandler</c> and other MediatR-related components for
/// managing request-response flows within the system.
/// </remarks>
public interface IStreamRequest<TResponse> : IRequest<IAsyncEnumerable<TResponse>>;

/// <summary>
/// Defines a handler for processing stream-based requests that return a sequence of responses asynchronously.
/// </summary>
/// <typeparam name="TRequest">The type of the request message. Must implement <see cref="IStreamRequest{TResponse}"/>.</typeparam>
/// <typeparam name="TResponse">The type of the response elements returned by the stream.</typeparam>
public interface IStreamRequestHandler<in TRequest, out TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    /// <summary>
    /// Handles a stream request and returns an asynchronous stream of responses.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request object being handled.</typeparam>
    /// <typeparam name="TResponse">The type of the response produced by the request handler.</typeparam>
    /// <param name="request">The request instance containing the necessary details for handling.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An asynchronous stream of <typeparamref name="TResponse"/> objects.</returns>
    IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}