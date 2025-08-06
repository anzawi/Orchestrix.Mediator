using Orchestrix.Mediator.Sagas.Contracts;

namespace Orchestrix.Mediator.Sagas.FluentApi;

public abstract class SagaBuilder<TInput> : ISaga
{
    internal List<SagaStepDefinition<TInput>> BuildSteps()
    {
        var builder = new SagaStepBuilder<TInput>();
        Build(builder);
        return builder.Build();
    }

    protected abstract void Build(SagaStepBuilder<TInput> saga);
}