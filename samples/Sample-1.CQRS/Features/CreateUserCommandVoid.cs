using Orchestrix.Mediator;
using Orchestrix.Mediator.Cqrs;

namespace Sample_1.CQRS.Features;

public record CreateUserCommandVoid(string Name) : ICommand;

public class CreateUserHandlerVoid(IPublisher publisher) : ICommandHandler<CreateUserCommandVoid>
{
    public async ValueTask Handle(CreateUserCommandVoid request, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        Console.WriteLine($"[COMMAND] User created: {request.Name} ({id})");
        await publisher.Publish(new UserCreatedNotification(request.Name), ct);
    }
}