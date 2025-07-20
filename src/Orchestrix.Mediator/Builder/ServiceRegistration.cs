using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator.Core.Execution;

namespace Orchestrix.Mediator.Builder;

internal static class ServiceRegistration
{
    public static void RegisterHandlers(IServiceCollection services, Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes().Where(t =>
                t is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false } &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>) ||
                        i.GetGenericTypeDefinition() == typeof(IRequestHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(INotificationHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(IParallelNotificationHandler<>) ||
                        i.GetGenericTypeDefinition() == typeof(IStreamRequestHandler<,>)
                    )
                ));

            foreach (var type in types)
            {
                foreach (var iface in type.GetInterfaces())
                {
                    if (!iface.IsGenericType) continue;

                    var def = iface.GetGenericTypeDefinition();
                    if (def == typeof(IRequestHandler<,>) ||
                        def == typeof(IRequestHandler<>) ||
                        def == typeof(INotificationHandler<>) ||
                        def == typeof(IParallelNotificationHandler<>) ||
                        def == typeof(IStreamRequestHandler<,>))
                    {
                        services.AddScoped(iface, type);
                    }
                }
            }
        }
    }

    public static void RegisterDefaultMediator(IServiceCollection services)
    {
        services.AddScoped<IMediator, Core.Execution.Mediator>();
    }

    public static void RegisterSourceGenMediator(IServiceCollection services)
    {
        var dispatcherType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return []; }
            })
            .FirstOrDefault(t =>
                t.FullName == "Orchestrix.Mediator.Generated.GeneratedDispatcher" &&
                typeof(IOrchestrixDispatcher).IsAssignableFrom(t));

        if (dispatcherType is null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("""
                                  Orchestrix.Mediator: Source-Generator mode requires the Orchestrix.Mediator.SourceGenerators package to be installed,
                                  and a valid GeneratedDispatcher must be generated.
                              """);
            Console.ResetColor();

            throw new InvalidOperationException("""
                Orchestrix.Mediator: Source-Generator mode requires the Orchestrix.Mediator.SourceGenerators package to be installed,
                and a valid GeneratedDispatcher must be generated.
            """);
        }

        services.AddScoped(typeof(IOrchestrixDispatcher), dispatcherType);
        services.AddScoped<IMediator, DispatcherMediator>();
    }
    
    public static void TryRegisterGeneratedHandlers(IServiceCollection services)
    {
        var registrationType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch { return []; }
            })
            .FirstOrDefault(t =>
                t.FullName == "Orchestrix.Mediator.Generated.GeneratedRegistration" &&
                t.GetMethod("AddGeneratedOrchestrixHandlers", BindingFlags.Static | BindingFlags.Public) is not null);

        if (registrationType is not null)
        {
            var method = registrationType.GetMethod("AddGeneratedOrchestrixHandlers", BindingFlags.Static | BindingFlags.Public);
            method?.Invoke(null, [services]);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("""
                                  Orchestrix.Mediator: Source-generator mode is enabled, but the source-generated registration method 
                                 'Orchestrix.Mediator.Generated.GeneratedRegistration.AddGeneratedOrchestrixHandlers' was not found.
                              
                                 Ensure:
                                 - You have defined at least one handler (request, notification, or stream)
                                 - You have clean and rebuilt the project after installing Orchestrix.Mediator.SourceGenerators
                              """);
            Console.ResetColor();

            throw new InvalidOperationException("""
                                                    Orchestrix.Mediator: Source-generator mode is enabled, but the source-generated registration method 
                                                    'Orchestrix.Mediator.Generated.GeneratedRegistration.AddGeneratedOrchestrixHandlers' was not found.

                                                    Ensure:
                                                    - You have defined at least one handler (request, notification, or stream)
                                                    - You have clean and rebuilt the project after installing Orchestrix.Mediator.SourceGenerators
                                                """);
        }
    }
}
