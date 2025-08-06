using Orchestrix.Mediator.Sagas.Contracts;

namespace Orchestrix.Mediator.Sagas.FluentApi;

public sealed class StepBuilder<TInput>
{
    private readonly SagaStepDefinition<TInput> _def;

    internal StepBuilder(SagaStepDefinition<TInput> def) => _def = def;

    public StepBuilder<TInput> Compensate<TCompensation>()
        where TCompensation : ISagaCompensationStep<TInput>
    {
        _def.CompensationType = typeof(TCompensation);
        return this;
    }

    public StepBuilder<TInput> Retry(int maxAttempts)
    {
        _def.RetryCount = maxAttempts;
        return this;
    }

    public StepBuilder<TInput> Timeout(TimeSpan timeout)
    {
        _def.Timeout = timeout;
        return this;
    }

    public StepBuilder<TInput> WithName(string name)
    {
        _def.Name = name;
        return this;
    }
}