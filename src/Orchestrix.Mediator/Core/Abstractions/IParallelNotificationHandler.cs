// ReSharper disable once CheckNamespace
namespace Orchestrix.Mediator;

/// <summary>
/// Defines a mechanism for processing notifications in parallel.
/// Implementations of this interface allow handling notifications concurrently,
/// as opposed to sequential processing typically found in standard notification handlers.
/// </summary>
/// <typeparam name="TNotification">
/// The type of notification to handle. Must implement <see cref="INotification"/>.
/// </typeparam>
public interface IParallelNotificationHandler<in TNotification> : INotificationHandler<TNotification>
    where TNotification : INotification
{
    // Marker interface: handled in parallel
}