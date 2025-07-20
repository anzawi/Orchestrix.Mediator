namespace Orchestrix.Mediator.Diagnostics.Hooks;

/// <summary>
/// Defines a contract for implementing hooks that can execute during the lifecycle
/// of streaming operations in a MediatR pipeline.
/// </summary>
public interface IStreamHook
{
    /// <summary>
    /// Triggered when a stream operation is starting. Allows for logging or other custom logic
    /// before the stream begins processing.
    /// </summary>
    /// <param name="request">The request object associated with the stream operation.</param>
    /// <param name="cancellationToken">A token that can be used to propagate cancellation to the operation.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the asynchronous operation.</returns>
    ValueTask OnStreamStart(object request, CancellationToken cancellationToken);

    /// <summary>
    /// Invoked when an error occurs during the stream operation.
    /// </summary>
    /// <param name="request">The request object associated with the stream operation.</param>
    /// <param name="exception">The exception that occurred during the stream operation.</param>
    /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    ValueTask OnStreamError(object request, Exception exception, CancellationToken cancellationToken);
}