namespace Orchestrix.Mediator.Sagas.Contracts;

public interface ISagaDecorator
{
    ValueTask InvokeAsync<TInput>(
        TInput input,
        Func<TInput, CancellationToken, ValueTask> next,
        CancellationToken ct = default
    ) where TInput : notnull;
}