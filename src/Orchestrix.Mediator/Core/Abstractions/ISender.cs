// ReSharper disable once CheckNamespace

namespace Orchestrix.Mediator;

public interface ISender
{
    /// <summary>
    /// Sends a request without expecting a return value.
    /// </summary>
    /// <param name="request">The request that implements <see cref="IRequest"/> to be sent.</param>
    /// <param name="cancellationToken">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask"/> that completes when the request has been processed.</returns>
    ValueTask<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a request to the mediator for processing without expecting a response.
    /// </summary>
    /// <param name="request">
    /// The request to be sent to the mediator. It must implement the <see cref="IRequest"/> interface.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe while waiting for the task to complete. The default value is <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous operation.
    /// </returns>
    ValueTask Send(IRequest request, CancellationToken cancellationToken = default);
    ValueTask<object?> Send(object request, CancellationToken cancellationToken = default);
    IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates an asynchronous stream of responses based on the provided request,
    /// enabling reactive data handling.
    /// </summary>
    /// <param name="request">
    /// The stream request object that implements the <see cref="IStreamRequest{TResponse}"/>
    /// interface, used to define the type of the responses.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to observe cancellation requests.
    /// </param>
    /// <returns>
    /// An asynchronous stream of responses of type object?.
    /// </returns>
    IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to send a request through the mediator. If the operation fails, it catches the exception and returns false.
    /// </summary>
    /// <param name="request">The request object to be processed. Cannot be null.</param>
    /// <param name="cancellationToken">A token to observe for operation cancellation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains <c>true</c> if the request was successfully sent; otherwise, <c>false</c>.</returns>
    ValueTask<bool> TrySend(object request, CancellationToken cancellationToken = default);
}