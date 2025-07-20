using Orchestrix.Mediator;

namespace Sample_2.SourceGenerator.Features;

public class PingCommand : IRequest;

public class PingHandler : IRequestHandler<PingCommand>
{
    public ValueTask Handle(PingCommand request, CancellationToken ct)
    {
        Console.WriteLine("[COMMAND] Ping handled (void)");
        return default;
    }
}