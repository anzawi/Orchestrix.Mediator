namespace Orchestrix.Mediator.Sagas.Contracts;

public interface ISagaHook
{
    ValueTask OnSagaStart(object? input, CancellationToken ct = default);
    ValueTask OnSagaComplete(object? input, CancellationToken ct = default);
    ValueTask OnSagaError(object? input, Exception exception, CancellationToken ct = default);
}