// ReSharper disable once CheckNamespace
namespace Orchestrix.Mediator.Cqrs;

/// <summary>
/// Defines a contract for handling commands in a CQRS pattern without a response.
/// </summary>
/// <typeparam name="TCommand">
/// The type of the command to handle. Must implement <see cref="ICommand"/>.
/// </typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand;

/// <summary>
/// Defines a contract for handling commands in a CQRS pattern with a response.
/// </summary>
/// <typeparam name="TCommand">
/// The type of the command to handle. Must implement <see cref="ICommand{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned after processing the command.
/// </typeparam>
public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>;