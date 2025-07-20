using Microsoft.Extensions.DependencyInjection;
using Orchestrix.Mediator.Builder;

namespace Orchestrix.Mediator;

/// <summary>
/// Provides extension methods for registering Orchestrix.Mediator services and handlers with the dependency injection system.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers Orchestrix.Mediator services and handlers into the service collection.
    /// Allows configuration of Orchestrix.Mediator builder to customize the registration process.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> where the Orchestrix.Mediator services will be registered.</param>
    /// <param name="configure">An action that configures the <see cref="IOrchestrixBuilder"/> to customize the registration.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> with Orchestrix.Mediator services registered.</returns>
    public static IServiceCollection AddOrchestrix(this IServiceCollection services,
        Action<IOrchestrixBuilder> configure)
    {
        var builder = new OrchestrixBuilder(services);
        configure(builder);
        builder.Apply();
        return services;
    }
}