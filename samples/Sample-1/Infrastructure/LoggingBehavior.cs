using Orchestrix.Mediator;

namespace Sample_1.Infrastructure;


public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async ValueTask<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        Console.WriteLine($"[PIPELINE] Start {typeof(TRequest).Name}");
        var result = await next(ct);
        Console.WriteLine($"[PIPELINE] End {typeof(TResponse).Name}");
        return result;
    }
}