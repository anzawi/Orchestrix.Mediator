using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator.Sagas.Contracts;
using Orchestrix.Mediator.Sagas.Decorators;
using Orchestrix.Mediator.Sagas.FluentApi;
using Orchestrix.Mediator.Sagas.Hooks;

namespace Orchestrix.Mediator.Sagas.Execution;

internal sealed class SagaCoordinator(IServiceProvider sp) : ISagaCoordinator
{
    public async ValueTask ExecuteAsync<TInput>(TInput input, CancellationToken ct = default)
        where TInput : notnull
    {
        var builder = sp.GetService<SagaBuilder<TInput>>();
        if (builder is null)
            throw new InvalidOperationException($"No SagaBuilder<{typeof(TInput).Name}> found.");
        
        var hookExecutor = new SagaHookExecutor(sp);

        var steps = builder.BuildSteps();

        var context = new SagaContext();

        var engine = new SagaExecutionEngine<TInput>(sp);
        
        try
        {
            var decoratorExecutor = new SagaDecoratorExecutor(sp);
            await decoratorExecutor.ExecuteAsync(input, async (inp, token) =>
            {
                await hookExecutor.OnStartAsync(inp, token);
                await engine.ExecuteAsync(inp, steps, context, token);
                await hookExecutor.OnCompleteAsync(inp, token);
            }, ct);
        }
        catch (Exception ex)
        {
            await hookExecutor.OnErrorAsync(input, ex, ct);
            throw;
        }
    }
}