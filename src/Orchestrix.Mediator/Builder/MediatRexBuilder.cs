using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator.Core.Execution;
using Orchestrix.Mediator.Core.Execution.Interfaces;
using Orchestrix.Mediator.Diagnostics;

namespace Orchestrix.Mediator.Builder;

internal sealed class OrchestrixBuilder(IServiceCollection services) : IOrchestrixBuilder
{
    private readonly List<Assembly> _handlerAssemblies = [];
    private readonly List<Assembly> _hookAssemblies = [];
    private readonly List<Type> _explicitHookTypes = [];
    private HookConfiguration _hookConfig = new();
    private bool _hasExplicitHookConfig;
    private bool _useSourceGen;

    public IServiceCollection Services { get; } = services;

    public IOrchestrixBuilder RegisterHandlersFromAssemblies(params Assembly[] assemblies)
    {
        _handlerAssemblies.AddRange(assemblies);
        return this;
    }

    public IOrchestrixBuilder AddBehavior(Type behaviorType)
    {
        Services.AddScoped(behaviorType);
        return this;
    }

    public IOrchestrixBuilder AddOpenBehavior(Type openBehaviorType)
    {
        if (openBehaviorType.IsGenericTypeDefinition)
        {
            Services.AddScoped(typeof(IPipelineBehavior<,>), openBehaviorType);
        }

        return this;
    }

    public IOrchestrixBuilder AddHook<T>() where T : class
    {
        _explicitHookTypes.Add(typeof(T));
        return this;
    }

    public IOrchestrixBuilder AddHooksFromAssemblies(Action<HookConfiguration>? configure, params Assembly[] assemblies)
    {
        _hookAssemblies.AddRange(assemblies);
        configure?.Invoke(_hookConfig);
        _hasExplicitHookConfig = true;
        return this;
    }

    public IOrchestrixBuilder AddHooksFromAssemblies(params Assembly[] assemblies) =>
        AddHooksFromAssemblies(null, assemblies);

    public IOrchestrixBuilder ConfigureHooks(Action<HookConfiguration> configure)
    {
        configure?.Invoke(_hookConfig);
        _hasExplicitHookConfig = true;
        return this;
    }

    public IOrchestrixBuilder UseSourceGenerator()
    {
        _useSourceGen = true;
        return this;
    }


    internal void Apply()
    {
        var handlersCount = _handlerAssemblies.Count;
        if (handlersCount == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Cannot detect any handlers!.");
            Console.WriteLine("At least one assembly must be registered via RegisterHandlersFromAssemblies.");
            Console.ResetColor();

#if !DEBUG
    throw new InvalidOperationException("No handler assemblies registered.");
#endif
        }

        if (handlersCount > 0)
        {
            if (_useSourceGen)
            {
                ServiceRegistration.RegisterSourceGenMediator(Services);
                ServiceRegistration.TryRegisterGeneratedHandlers(Services);
            }
            else
            {
                ServiceRegistration.RegisterHandlers(Services, _handlerAssemblies.ToArray());
                ServiceRegistration.RegisterDefaultMediator(Services);
            }
        }

        Services.AddScoped<ISender>(sp => sp.GetRequiredService<IMediator>());
        Services.AddScoped<IPublisher>(sp => sp.GetRequiredService<IMediator>());
        Services.AddScoped<IHookExecutor, HookExecutor>();

        if (_hookAssemblies.Count > 0 || _explicitHookTypes.Count > 0)
        {
            var config = _hasExplicitHookConfig ? _hookConfig : new HookConfiguration();
            Services.AddSingleton(config);

            if (_hookAssemblies.Count > 0)
                HookRegistration.RegisterHooks(Services, _hookAssemblies.ToArray(), config);

            if (_explicitHookTypes.Count > 0)
                HookRegistration.RegisterExplicitHooks(Services, _explicitHookTypes);
        }
    }
}