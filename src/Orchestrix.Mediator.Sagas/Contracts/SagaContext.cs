namespace Orchestrix.Mediator.Sagas.Contracts;

internal sealed class SagaContext
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public DateTimeOffset StartedAt { get; init; } = DateTimeOffset.UtcNow;
    public Dictionary<string, object?> Metadata { get; } = [];
}