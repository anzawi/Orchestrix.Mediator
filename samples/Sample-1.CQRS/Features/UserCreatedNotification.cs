using Orchestrix.Mediator;

namespace Sample_1.CQRS.Features;

public record UserCreatedNotification(string Name) : INotification;

public class LogUserNotificationHandler : INotificationHandler<UserCreatedNotification>
{
    public ValueTask Handle(UserCreatedNotification notification, CancellationToken ct)
    {
        Console.WriteLine($"[NOTIFICATION - Sequential] User created: {notification.Name}");
        return default;
    }
}

public class EmailUserNotificationHandler : IParallelNotificationHandler<UserCreatedNotification>
{
    public async ValueTask Handle(UserCreatedNotification notification, CancellationToken ct)
    {
        await Task.Delay(300, ct);
        Console.WriteLine($"[NOTIFICATION - Parallel] Welcome email sent to {notification.Name}");
    }
}