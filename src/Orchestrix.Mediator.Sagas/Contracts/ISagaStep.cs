namespace Orchestrix.Mediator.Sagas.Contracts;

public interface ISagaStep<in TInput> : ISaga
{
    ValueTask ExecuteAsync(TInput input, ISender sender, CancellationToken ct = default);
}