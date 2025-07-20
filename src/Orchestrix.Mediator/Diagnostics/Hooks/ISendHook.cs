namespace Orchestrix.Mediator.Diagnostics.Hooks;

/// <summary>
/// Defines a hook that allows intervention and custom logic during the lifecycle of a send operation.
/// </summary>
public interface ISendHook
{
    /// <summary>
    /// Executes logic when the send operation starts.
    /// </summary>
    /// <param name="request">The object representing the request being sent.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask OnSendStart(object request, CancellationToken cancellationToken);

    /// <summary>
    /// Invoked when a send operation is completed successfully.
    /// </summary>
    /// <param name="request">The original request object that was sent.</param>
    /// <param name="response">The response object received, or null if no response was returned.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="ValueTask"/> representing the completion of the operation.</returns>
    ValueTask OnSendComplete(object request, object? response, CancellationToken cancellationToken);

    /// <summary>
    /// Invoked when an error occurs during the execution of a send operation.
    /// </summary>
    /// <param name="request">The request object that caused the error.</param>
    /// <param name="exception">The exception that occurred during the send operation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask OnSendError(object request, Exception exception, CancellationToken cancellationToken);
}