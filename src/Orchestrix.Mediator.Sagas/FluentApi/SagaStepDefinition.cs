namespace Orchestrix.Mediator.Sagas.FluentApi;

internal sealed class SagaStepDefinition<TInput>
{
    public string? Name { get; set; }
    public required Type StepType { get; set; }
    public Type? CompensationType { get; set; }
    public int RetryCount { get; set; } = 0;
    public TimeSpan? Timeout { get; set; }
}