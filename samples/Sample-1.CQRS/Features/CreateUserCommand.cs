using Orchestrix.Mediator;
using Orchestrix.Mediator.Cqrs;

namespace Sample_1.CQRS.Features;

public record CreateUserCommand(string Name) : ICommand<Guid>;

public class CreateUserHandler(IPublisher publisher) : ICommandHandler<CreateUserCommand, Guid>
{
    public async ValueTask<Guid> Handle(CreateUserCommand request, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        Console.WriteLine($"[COMMAND] User created: {request.Name} ({id})");
        await publisher.Publish(new UserCreatedNotification(request.Name), ct);
        return id;
    }
}