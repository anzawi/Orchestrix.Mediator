using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator;
using Sample_2.SourceGenerator.Features;
using Sample_2.SourceGenerator.Infrastructure;


var services = new ServiceCollection();

services.AddOrchestrix(cfg =>
{
    cfg.UseSourceGenerator();
    cfg.RegisterHandlersFromAssemblies(typeof(CreateUserCommand).Assembly);
    cfg.AddHook<LoggingHook>();
});

var provider = services.BuildServiceProvider();

// ✅ Resolve the generated dispatcher
var dispatcher = provider.GetRequiredService<IOrchestrixDispatcher>();
var ct = CancellationToken.None;

// ✅ 1. Dispatch generic command
var id = await dispatcher.Dispatch(new CreateUserCommand("Mohammad"), provider, ct);
Console.WriteLine($"Returned ID: {id}");

// ✅ 2. Dispatch void command
await dispatcher.DispatchVoid(new PingCommand(), provider, ct);

// ✅ 3. Dispatch notification
await dispatcher.DispatchPublish(new UserCreated("Mohammad"), provider, ct);

// ✅ 4. Dispatch streaming
await foreach (var user in dispatcher.DispatchStream(new GetUsersStream(3), provider, ct))
{
    Console.WriteLine($"[STREAM RESULT] {user}");
}