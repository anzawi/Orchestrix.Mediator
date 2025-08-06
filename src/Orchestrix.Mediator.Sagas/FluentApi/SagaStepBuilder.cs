using Orchestrix.Mediator.Sagas.Contracts;

namespace Orchestrix.Mediator.Sagas.FluentApi;

public sealed class SagaStepBuilder<TInput>
{
    private readonly List<SagaStepDefinition<TInput>> _steps = [];

    public StepBuilder<TInput> Step<TStep>()
        where TStep : ISagaStep<TInput>
    {
        var def = new SagaStepDefinition<TInput>
        {
            StepType = typeof(TStep)
        };
        _steps.Add(def);
        return new StepBuilder<TInput>(def);
    }

    internal List<SagaStepDefinition<TInput>> Build() => _steps;
}