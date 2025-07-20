namespace Orchestrix.Mediator.SourceGenerators;

/// <summary>
/// Represents a request handler target with associated metadata regarding the request type,
/// optional response type, the handler type, and the kind of handler.
/// </summary>
public record RequestHandlerTarget(
    string RequestType,
    string? ResponseType,
    string? HandlerType,
    string Kind, // "GenericRequest", "VoidRequest", "Notification", "StreamRequest"
    bool IsParallel = false
);