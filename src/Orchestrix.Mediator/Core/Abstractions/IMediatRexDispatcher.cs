namespace Orchestrix.Mediator;

/// <summary>
/// Defines a dispatcher interface for mediating commands, requests, notifications, and streams
/// using a provided service provider for dependency resolution.
/// </summary>
public interface IOrchestrixDispatcher
{
    ValueTask<TResponse> Dispatch<TResponse>(IRequest<TResponse> request, IServiceProvider provider, CancellationToken cancellation = default);

    ValueTask DispatchVoid(IRequest request, IServiceProvider provider, CancellationToken cancellation = default);

    /// Dispatches a given request through the mediator and returns the result, if any.
    /// <param name="request">The request object to dispatch. It should be an instance of IRequest or another recognized request type.</param>
    /// <param name="provider">The service provider to resolve dependencies needed during dispatch.</param>
    /// <param name="cancellation">A token to observe while waiting for the task to complete. Defaults to CancellationToken.None.</param>
    /// <returns>A task that represents the asynchronous dispatch operation. The task result contains the response object if the request has a response, or null otherwise.</returns>
    ValueTask<object?> Dispatch(object request, IServiceProvider provider, CancellationToken cancellation = default);

    /// Publishes a notification to all registered handlers that implement the INotificationHandler interface
    /// for the notification type. This is typically used to signal an event to multiple subscribers.
    /// <typeparam name="TNotification">The type of the notification that implements the INotification interface.</typeparam>
    /// <param name="notification">The notification instance to publish.</param>
    /// <param name="provider">The service provider used for resolving dependencies.</param>
    /// <param name="cancellation">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>A ValueTask that represents the asynchronous operation.</returns>
    ValueTask DispatchPublish<TNotification>(TNotification notification, IServiceProvider provider, CancellationToken cancellation = default)
        where TNotification : INotification;
    ValueTask DispatchPublish(object notification, IServiceProvider provider, CancellationToken cancellation = default);

    /// <summary>
    /// Dispatches a stream request and returns an asynchronous stream of responses.
    /// </summary>
    /// <typeparam name="TResponse">The type of response items in the stream.</typeparam>
    /// <param name="request">The stream request to be dispatched.</param>
    /// <param name="provider">The service provider used to resolve dependencies.</param>
    /// <param name="cancellation">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An asynchronous stream of <typeparamref name="TResponse"/> items.
    /// </returns>
    IAsyncEnumerable<TResponse> DispatchStream<TResponse>(IStreamRequest<TResponse> request, IServiceProvider provider, CancellationToken cancellation = default);

    /// <summary>
    /// Dispatches a stream request and returns an asynchronous stream of responses.
    /// </summary>
    /// <param name="request">The stream request to be dispatched.</param>
    /// <param name="provider">The service provider used to resolve dependencies.</param>
    /// <param name="cancellation">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// An asynchronous stream of object? items.
    /// </returns>
    IAsyncEnumerable<object?> DispatchStream(object request, IServiceProvider provider, CancellationToken cancellation = default);
}