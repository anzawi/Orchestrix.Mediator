using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator.Sagas.FluentApi;

namespace Orchestrix.Mediator.Sagas.Execution;

internal sealed class SagaStepExecutor<TInput>(IServiceProvider sp, SagaStepDefinition<TInput> step)
{
    public async ValueTask ExecuteStepAsync(TInput input, CancellationToken outerCt)
    {
        var attempt = 0;
        var maxAttempts = Math.Max(1, step.RetryCount);

        while (true)
        {
            attempt++;

            using var cts = step.Timeout.HasValue
                ? CancellationTokenSource.CreateLinkedTokenSource(outerCt)
                : null;

            if (step.Timeout is not null)
                cts!.CancelAfter(step.Timeout.Value);

            var ct = cts?.Token ?? outerCt;

            try
            {
                var handler = sp.GetRequiredService(step.StepType);
                var method = step.StepType.GetMethod("ExecuteAsync");

                if (method is null)
                    throw new InvalidOperationException($"Step '{step.Name ?? step.StepType.Name}' does not implement ExecuteAsync.");

                var task = (ValueTask)method.Invoke(handler, [input, sp.GetRequiredService<ISender>(), ct])!;
                await task;

                return; // success
            }
            catch (OperationCanceledException oce) when (cts is not null && cts.IsCancellationRequested)
            {
                throw new TimeoutException($"Step '{step.StepType.Name}' timed out after {step.Timeout!.Value.TotalSeconds}s.", oce);
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                Console.WriteLine($"[Saga] Step '{step.StepType.Name}' failed (attempt {attempt}/{maxAttempts}). Retrying...");
                await Task.Delay(250, ct); // basic backoff
                continue;
            }
            catch
            {
                throw; // rethrow on final failure
            }
        }
    }

    public async ValueTask ExecuteCompensationAsync(TInput input, CancellationToken ct)
    {
        if (step.CompensationType is null)
            return;

        var compensator = sp.GetRequiredService(step.CompensationType);
        var method = step.CompensationType.GetMethod("CompensateAsync");

        if (method is null)
            throw new InvalidOperationException($"Compensator '{step.CompensationType.Name}' does not implement CompensateAsync.");

        var task = (ValueTask)method.Invoke(compensator, [input, sp.GetRequiredService<ISender>(), ct])!;
        await task;
    }
}