namespace Orchestrix.Mediator.Sagas.Contracts;

public interface ISagaCompensationStep<in TInput> : ISaga
{
    ValueTask CompensateAsync(TInput input, ISender sender, CancellationToken ct = default);
}