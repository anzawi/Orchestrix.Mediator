using Orchestrix.Mediator;

namespace Sample_2.SourceGenerator.Features;

public record CreateUserCommand(string Name) : IRequest<Guid>;

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Guid>
{
    public ValueTask<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        Console.WriteLine($"[COMMAND] User created: {request.Name} ({id})");
        return ValueTask.FromResult(id);
    }
}
