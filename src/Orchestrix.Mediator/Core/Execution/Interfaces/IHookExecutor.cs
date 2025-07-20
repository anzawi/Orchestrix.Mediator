namespace Orchestrix.Mediator.Core.Execution.Interfaces;

/// <summary>
/// Defines the interface for executing hooks in MediatR operations, specifically for Send and Publish actions.
/// The <see cref="IHookExecutor"/> interface provides methods to execute hooks before and after specific MediatR operations,
/// enabling the integration of custom logic, diagnostics, or tracking within the MediatR pipeline.
/// </summary>
internal interface IHookExecutor
{
    /// <summary>
    /// Executes a given request with optional send hooks, wrapping the execution
    /// process in pre- and post-processing logic if send hooks are enabled.
    /// </summary>
    /// <typeparam name="T">The type of the response produced by the execution.</typeparam>
    /// <param name="request">The request object to process.</param>
    /// <param name="execute">A function representing the core execution logic for the request.</param>
    /// <param name="ct">A <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="ValueTask"/> that represents the result of the execution.</returns>
    public ValueTask<T> ExecuteWithSendHooks<T>(
        object request,
        Func<CancellationToken, ValueTask<T>> execute,
        CancellationToken ct);

    /// Executes the provided function with publish hooks enabled. This method applies
    /// pre-publish and post-publish logic if a publish hook is provided and configured.
    /// <param name="notification">The notification object being published.</param>
    /// <param name="execute">The function to execute, representing the core publish logic.</param>
    /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
    /// <return>A ValueTask that completes when the publish hooks and the main execution function have finished executing.</return>
    public ValueTask ExecuteWithPublishHooks(
        object notification,
        Func<CancellationToken, ValueTask> execute,
        CancellationToken ct);
}