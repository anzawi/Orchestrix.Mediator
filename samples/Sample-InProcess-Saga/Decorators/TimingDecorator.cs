using System.Diagnostics;
using Orchestrix.Mediator.Sagas.Contracts;

namespace Sample_InProcess_Saga.Decorators;
public sealed class TimingDecorator : ISagaDecorator
{
    public async ValueTask InvokeAsync<TInput>(
        TInput input,
        Func<TInput, CancellationToken, ValueTask> next,
        CancellationToken ct) where TInput : notnull
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await next(input, ct);
            Console.WriteLine($"[Decorator] Saga completed in {sw.ElapsedMilliseconds}ms");
        }
        catch
        {
            Console.WriteLine($"[Decorator] Saga failed in {sw.ElapsedMilliseconds}ms");
            throw;
        }
    }
}