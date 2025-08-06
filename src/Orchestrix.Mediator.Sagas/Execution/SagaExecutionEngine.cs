using Orchestrix.Mediator.Sagas.Contracts;
using Orchestrix.Mediator.Sagas.FluentApi;

namespace Orchestrix.Mediator.Sagas.Execution;

internal sealed class SagaExecutionEngine<TInput>(IServiceProvider sp)
{
    public async ValueTask ExecuteAsync(
        TInput input,
        List<SagaStepDefinition<TInput>> steps,
        SagaContext context,
        CancellationToken ct)
    {
        var executed = new Stack<SagaStepDefinition<TInput>>();

        foreach (var step in steps)
        {
            var executor = new SagaStepExecutor<TInput>(sp, step);

            try
            {
                await executor.ExecuteStepAsync(input, ct);
                executed.Push(step);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Saga] Step failed: {step.StepType.Name}. Rolling back...");

                while (executed.Count > 0)
                {
                    var prev = executed.Pop();
                    if (prev.CompensationType is not null)
                    {
                        var compensator = new SagaStepExecutor<TInput>(sp, prev);
                        await compensator.ExecuteCompensationAsync(input, ct);
                    }
                }

                throw new SagaExecutionException($"Saga failed on step {step.StepType.Name}", ex);
            }
        }
    }
}