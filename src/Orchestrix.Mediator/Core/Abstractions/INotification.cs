// ReSharper disable once CheckNamespace

namespace Orchestrix.Mediator;

///<sammary>
/// Represents a notification that can be handled by one or more notification handlers.
/// Notifications are objects that are typically used to signal events or state changes
/// within an application. They do not return a result and are processed by handlers
/// implementing the corresponding notification handler interface.
/// </sammary>
public interface INotification;

/// <summary>
/// Defines a handler for processing notifications of a specific type.
/// </summary>
/// <typeparam name="TNotification">
/// The type of notification that this handler processes. Must implement <see cref="INotification"/>.
/// </typeparam>
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handles a specific notification of type <typeparamref name="TNotification"/>.
    /// This method executes the logic associated with processing or responding to the provided notification.
    /// </summary>
    /// <param name="notification">
    /// The notification instance of type <typeparamref name="TNotification"/> to process. This type must implement <see cref="INotification"/>.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to monitor while handling the notification.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous operation of handling the notification.
    /// </returns>
    ValueTask Handle(TNotification notification, CancellationToken cancellationToken);
}

/// <summary>
/// Represents an abstract base class for handling notifications of a specific type.
/// </summary>
/// <typeparam name="TNotification">
/// The type of notification being handled. Must implement the <see cref="INotification"/> interface.
/// </typeparam>
/// <remarks>
/// This class provides a framework for handling notifications. Any derived class must implement
/// the abstract <see cref="Handle(TNotification)"/> method to define the processing logic for
/// the specific notification type.
/// </remarks>
public abstract class NotificationHandler<TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Handles a specific notification of type <typeparamref name="TNotification"/>.
    /// This method executes the notification handling logic, typically by
    /// processing or responding to the provided notification.
    /// </summary>
    /// <param name="notification">
    /// The notification instance of type <typeparamref name="TNotification"/> to be handled.
    /// </param>
    /// <param name="cancellationToken">
    /// A <see cref="CancellationToken"/> to observe while waiting for the task to complete.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> that represents the asynchronous operation for handling the notification.
    /// </returns>
    ValueTask INotificationHandler<TNotification>.Handle(TNotification notification,
        CancellationToken cancellationToken)
    {
        Handle(notification);

        return default;
    }

    /// <summary>
    /// Handles the provided notification of type <typeparamref name="TNotification"/>.
    /// This abstract method must be implemented to define the logic for processing the notification.
    /// </summary>
    /// <param name="notification">
    /// The notification instance of type <typeparamref name="TNotification"/> to be processed. This type must implement <see cref="INotification"/>.
    /// </param>
    protected abstract void Handle(TNotification notification);
}