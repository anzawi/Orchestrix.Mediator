using Orchestrix.Mediator;

namespace Sample_2.SourceGenerator.Features;


public record UserCreated(string Name) : INotification;

public class LogNotificationHandler : INotificationHandler<UserCreated>
{
    public ValueTask Handle(UserCreated notification, CancellationToken ct)
    {
        Console.WriteLine($"[NOTIFICATION] UserCreated: {notification.Name}");
        return default;
    }
}