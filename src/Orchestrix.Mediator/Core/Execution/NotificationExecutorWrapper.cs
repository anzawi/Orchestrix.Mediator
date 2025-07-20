/*
Exception strategy config (continue on fail?)	⏳ v2+
Logging/tracing hooks	⏳ v1.1+
Timeouts per handler	⏳ opt-in later
 */

using Microsoft.Extensions.DependencyInjection;

namespace Orchestrix.Mediator.Core.Execution;

/// <summary>
/// Represents an executor for handling notifications of a specific type.
/// </summary>
/// <typeparam name="TNotification">
/// The type of the notification that this executor processes.
/// Must implement the <see cref="INotification"/> interface.
/// </typeparam>
internal interface INotificationExecutor<in TNotification>
    where TNotification : INotification
{
    /// <summary>
    /// Executes the notification handlers for the provided notification type.
    /// Resolves and manages both sequential and parallel execution of handlers.
    /// </summary>
    /// <param name="provider">
    /// The service provider used to resolve registered notification handlers.
    /// </param>
    /// <param name="notification">
    /// The notification instance to be processed by the resolved handlers.
    /// </param>
    /// <param name="ct">
    /// A cancellation token to observe while awaiting the execution of handlers.
    /// </param>
    /// <returns>
    /// A <see cref="ValueTask"/> representing the asynchronous operation of executing notification handlers.
    /// </returns>
    ValueTask Execute(IServiceProvider provider, TNotification notification, CancellationToken ct);
}

/// <inheritdoc />
internal sealed class NotificationExecutorWrapper<TNotification> : INotificationExecutor<TNotification>
    where TNotification : INotification
{
    /// <inheritdoc />
    public async ValueTask Execute(IServiceProvider provider, TNotification notification, CancellationToken ct)
    {
        // Resolve all registered handlers for this notification
        var handlers = provider.GetServices<INotificationHandler<TNotification>>();

        var notificationHandlers = handlers as INotificationHandler<TNotification>[] ?? handlers.ToArray();
        if (!notificationHandlers.Any())
            return;

        // Partition: sequential vs parallel
        var parallel = new List<IParallelNotificationHandler<TNotification>>(capacity: 2);
        var sequential = new List<INotificationHandler<TNotification>>(capacity: 4);

        foreach (var handler in notificationHandlers)
        {
            if (handler is IParallelNotificationHandler<TNotification> parallelHandler)
                parallel.Add(parallelHandler);
            else
                sequential.Add(handler);
        }

        // Sequential execution (fire in order)
        foreach (var handler in sequential)
        {
            await handler.Handle(notification, ct);
        }

        // Parallel execution (if any)
        if (parallel.Count > 0)
        {
            var tasks = new Task[parallel.Count];

            for (var i = 0; i < parallel.Count; i++)
            {
                tasks[i] = parallel[i].Handle(notification, ct).AsTask();
            }

            await Task.WhenAll(tasks);
        }
    }
}