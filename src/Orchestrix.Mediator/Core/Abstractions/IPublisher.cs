// ReSharper disable once CheckNamespace

namespace Orchestrix.Mediator;

/// <summary>
/// Defines a contract for publishing notifications or events to one or more handlers.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for managing the distribution of notifications
/// to their respective handlers. Notifications can either be dispatched as specific types or as general objects.
/// </remarks>
public interface IPublisher
{
    /// <summary>
    /// Publishes a notification to all its subscribers.
    /// </summary>
    /// <param name="notification">
    /// The notification object to publish. Must implement <see cref="INotification"/> or a generic interface of <see cref="INotification"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to observe for cancellation request. Optional.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> that represents the asynchronous operation.
    /// </returns>
    ValueTask Publish(object notification, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a notification to all relevant handlers, using the specified notification instance.
    /// Supports both strongly-typed notifications and notifications provided as an object.
    /// </summary>
    /// <typeparam name="TNotification">The type of the notification to publish. It must implement <see cref="INotification"/>.</typeparam>
    /// <param name="notification">
    /// The notification instance to publish. If the notification is provided as an object, ensure all relevant handlers support the notification type.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> used to cancel the publish operation if needed. Defaults to <see cref="CancellationToken.None"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> that represents the asynchronous operation. This operation does not return a value.
    /// </returns>
    ValueTask Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification;

    /// <summary>
    /// Attempts to publish a notification asynchronously, ensuring that any exceptions
    /// during the publishing process are handled and logged.
    /// </summary>
    /// <param name="notification">
    /// The notification object to be published. This cannot be null.
    /// </param>
    /// <param name="cancellationToken">
    /// Optional. A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The result of the task indicates whether
    /// the notification was successfully published (true) or failed (false).
    /// </returns>
    ValueTask<bool> TryPublish(object notification, CancellationToken cancellationToken = default);
}