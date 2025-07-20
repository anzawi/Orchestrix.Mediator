using Orchestrix.Mediator;

namespace Sample_1.Features;

public record CreateUserCommand(string Name) : IRequest<Guid>;

public class CreateUserHandler(IPublisher publisher) : IRequestHandler<CreateUserCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        Console.WriteLine($"[COMMAND] User created: {request.Name} ({id})");
        await publisher.Publish(new UserCreatedNotification(request.Name), ct);
        return id;
    }
}