using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator.Builder;
using Orchestrix.Mediator.Sagas.Contracts;
using Orchestrix.Mediator.Sagas.Execution;
using Orchestrix.Mediator.Sagas.FluentApi;

namespace Orchestrix.Mediator.Sagas.Extensions;

/// <summary>
/// builder.Services.AddOrchestrixSagas(typeof(MySaga).Assembly);
/// 
/// builder.Services.AddSagaHook{LoggingSagaHook}();
/// builder.Services.AddSagaDecorator{MetricsSagaDecorator}();
/// </summary>
public static class SagaBuilderExtensions
{
    public static IOrchestrixBuilder AddSagas(
        this IOrchestrixBuilder builder,
        params Assembly[] assemblies)
    {
        var services = builder.Services;

        foreach (var type in assemblies.SelectMany(x => x.DefinedTypes))
        {
            if (type.IsAbstract || type.IsInterface) continue;

            if (type.BaseType is { IsGenericType: true } baseType &&
                baseType.GetGenericTypeDefinition() == typeof(SagaBuilder<>))
            {
                services.AddTransient(baseType, type); // SagaBuilder<T>
            }
            
            // Register ISagaStep<T> and ISagaCompensationStep<T>
            if (type.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    (
                        i.GetGenericTypeDefinition() == typeof(ISagaStep<>) ||
                        i.GetGenericTypeDefinition() == typeof(ISagaCompensationStep<>)
                    )))
            {
                services.AddTransient(type);
            }
        }

        services.AddScoped<ISagaCoordinator, SagaCoordinator>();
        return builder;
    }

    public static IOrchestrixBuilder AddSagaHook<THook>(this IOrchestrixBuilder builder)
        where THook : class, ISagaHook
    {
        builder.Services.AddScoped<ISagaHook, THook>();
        return builder;
    }

    public static IOrchestrixBuilder AddSagaDecorator<TDecorator>(this IOrchestrixBuilder builder)
        where TDecorator : class, ISagaDecorator
    {
        builder.Services.AddScoped<ISagaDecorator, TDecorator>();
        return builder;
    }
}