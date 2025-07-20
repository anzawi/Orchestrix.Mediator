using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator.Diagnostics;

namespace Orchestrix.Mediator.Builder;

public interface IOrchestrixBuilder
{
    IServiceCollection Services { get; }

    IOrchestrixBuilder RegisterHandlersFromAssemblies(params Assembly[] assemblies);
    IOrchestrixBuilder AddBehavior(Type behaviorType);
    IOrchestrixBuilder AddOpenBehavior(Type openBehaviorType);
    IOrchestrixBuilder AddHook<T>() where T : class;
    IOrchestrixBuilder AddHooksFromAssemblies(Action<HookConfiguration>? configure, params Assembly[] assemblies);
    IOrchestrixBuilder AddHooksFromAssemblies(params Assembly[] assemblies);
    IOrchestrixBuilder ConfigureHooks(Action<HookConfiguration> configure);
    IOrchestrixBuilder UseSourceGenerator();
}