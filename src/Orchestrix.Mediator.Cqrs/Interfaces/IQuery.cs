// ReSharper disable once CheckNamespace
namespace Orchestrix.Mediator.Cqrs;

/// <summary>
/// Represents a query in a CQRS (Command Query Responsibility Segregation) pattern.
/// A query is a request for information and produces a response of type <typeparamref name="TResponse"/>.
/// </summary>
/// <typeparam name="TResponse">
/// The type of the response produced by the query.
/// </typeparam>
public interface IQuery<TResponse> : IRequest<TResponse>;
