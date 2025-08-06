namespace Orchestrix.Mediator.Sagas.Contracts;

public interface ISagaCoordinator
{
    ValueTask ExecuteAsync<TInput>(TInput input, CancellationToken ct = default)
        where TInput : notnull;
}