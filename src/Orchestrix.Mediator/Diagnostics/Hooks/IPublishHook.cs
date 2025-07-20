namespace Orchestrix.Mediator.Diagnostics.Hooks;

/// <summary>
/// Defines hooks that are invoked during the publish events in a mediator pipeline.
/// </summary>
public interface IPublishHook
{
    /// <summary>
    /// Triggered at the beginning of the publish operation to perform
    /// any pre-publish processing or logging.
    /// </summary>
    /// <param name="notification">The notification object to be published.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask OnPublishStart(object notification, CancellationToken cancellationToken);

    /// <summary>
    /// Invoked when the publishing process for a notification completes successfully.
    /// </summary>
    /// <param name="notification">The notification object that was published.</param>
    /// <param name="cancellationToken">
    /// A cancellation token to observe while waiting for the task to complete.
    /// </param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    ValueTask OnPublishComplete(object notification, CancellationToken cancellationToken);

    /// <summary>
    /// Invoked when an error occurs during the execution of a publish operation.
    /// </summary>
    /// <param name="notification">The notification object that was being published when the error occurred.</param>
    /// <param name="exception">The exception that was thrown during the publish operation.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask OnPublishError(object notification, Exception exception, CancellationToken cancellationToken);
}