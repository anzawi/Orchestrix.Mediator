using Orchestrix.Mediator;

namespace Sample_1.Features;

public record CreateUserCommandVoid(string Name) : IRequest;

public class CreateUserHandlerVoid(IPublisher publisher) : IRequestHandler<CreateUserCommandVoid>
{
    public async ValueTask Handle(CreateUserCommandVoid request, CancellationToken ct)
    {
        var id = Guid.NewGuid();
        Console.WriteLine($"[COMMAND] User created: {request.Name} ({id})");
        await publisher.Publish(new UserCreatedNotification(request.Name), ct);
    }
}