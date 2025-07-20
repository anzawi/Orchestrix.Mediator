using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator.Diagnostics;
using Orchestrix.Mediator.Diagnostics.Hooks;

namespace Orchestrix.Mediator.Builder;

internal static class HookRegistration
{
    public static void RegisterHooks(IServiceCollection services, Assembly[] assemblies, HookConfiguration config)
    {
        services.AddSingleton(config);

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes()
                .Where(t =>
                    t is { IsAbstract: false, IsInterface: false, IsGenericTypeDefinition: false } &&
                    t.GetInterfaces().Any(i =>
                        i == typeof(ISendHook) || i == typeof(IPublishHook) || i == typeof(IStreamHook)));

            foreach (var type in types)
            {
                foreach (var iface in type.GetInterfaces())
                {
                    if (iface == typeof(ISendHook) || iface == typeof(IPublishHook) || iface == typeof(IStreamHook))
                        services.AddScoped(iface, type);
                }
            }
        }
    }

    public static void RegisterExplicitHooks(IServiceCollection services, IEnumerable<Type> hookTypes)
    {
        foreach (var type in hookTypes)
        {
            foreach (var iface in type.GetInterfaces())
            {
                if (iface == typeof(ISendHook) || iface == typeof(IPublishHook) || iface == typeof(IStreamHook))
                    services.AddScoped(iface, type);
            }
        }
    }
}