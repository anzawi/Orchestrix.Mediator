using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator.Sagas.Contracts;

namespace Orchestrix.Mediator.Sagas.Decorators;

internal sealed class SagaDecoratorExecutor(IServiceProvider sp)
{
    private readonly ISagaDecorator[] _decorators = sp.GetServices<ISagaDecorator>()?.ToArray() ?? [];

    public ValueTask ExecuteAsync<TInput>(
        TInput input,
        Func<TInput, CancellationToken, ValueTask> next,
        CancellationToken ct)
        where TInput : notnull
    {
        var current = next;

        foreach (var decorator in _decorators.Reverse())
        {
            var nextCopy = current;
            current = (inp, token) => decorator.InvokeAsync(inp, nextCopy, token);
        }

        return current(input, ct);
    }
}