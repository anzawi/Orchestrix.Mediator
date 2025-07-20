namespace Orchestrix.Mediator;

/// <summary>
/// Represents an interface that acts as a mediator for processing requests and publishing notifications.
/// </summary>
/// <remarks>
/// IMediator combines the functionality of both sending requests and publishing notifications.
/// It extends the <see cref="ISender"/> and <see cref="IPublisher"/> interfaces to provide a unified API for:
/// - Sending synchronous or asynchronous requests and receiving responses.
/// - Publishing notifications to multiple handlers.
/// </remarks>
/// <seealso cref="ISender"/>
/// <seealso cref="IPublisher"/>
public interface IMediator: ISender, IPublisher;