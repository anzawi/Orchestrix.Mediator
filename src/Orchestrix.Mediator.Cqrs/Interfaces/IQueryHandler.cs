// ReSharper disable once CheckNamespace
namespace Orchestrix.Mediator.Cqrs;

/// <summary>
/// Represents a handler for processing queries in the CQRS (Command Query Responsibility Segregation) pattern.
/// A query handler is responsible for handling a specific type of query and returning a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TQuery">
/// The type of the query being handled. This must implement <see cref="IQuery{TResponse}"/>.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response returned by the handler after processing the query.
/// </typeparam>
public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>;