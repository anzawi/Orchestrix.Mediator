namespace Orchestrix.Mediator;

/// <summary>
/// Represents the base interface for all types of requests within the application.
/// </summary>
public interface IBaseRequest;

/// <summary>
/// Represents a basic abstraction for request messages in a mediator-based architecture.
/// This interface is implemented by request types that do not produce a response.
/// </summary>
public interface IRequest : IBaseRequest;

/// <summary>
/// Represents a marker interface that defines a request without a return type.
/// </summary>
public interface IRequest<TResponse> : IBaseRequest;