// ReSharper disable once CheckNamespace
namespace Orchestrix.Mediator.Cqrs;

/// <summary>
/// Represents a command in a CQRS pattern.
/// </summary>
/// <remarks>
/// This interface is used to define commands that do not return a response.
/// Commands are typically used to modify state or perform an action within a system.
/// </remarks>
public interface ICommand : IRequest;

/// <summary>
/// Represents a command in a CQRS pattern.
/// Commands are requests that modify state and return data.
/// </summary>
public interface ICommand<TResponse> : IRequest<TResponse>;