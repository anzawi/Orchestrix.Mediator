using System.Collections.Generic;
using System.Linq;

namespace Orchestrix.Mediator.SourceGenerators;

/// <summary>
/// Provides methods for generating the body of dispatch methods used in the generated dispatcher class.
/// </summary>
/// <remarks>
/// This class includes methods to generate different types of dispatch logic, such as handling generic requests,
/// void responses, object responses, publish operations, and streaming operations. Each method generates
/// appropriate source code based on the provided list of request handler targets.
/// </remarks>
internal static class DispatcherBodyBuilder
{
    /// <summary>
    /// Generates the dispatch method implementation for handling generic request types.
    /// </summary>
    /// <param name="targets">A list of request handler targets that define the mappings for various "GenericRequest" handlers.</param>
    /// <returns>Returns a string containing the generated dispatch method implementation for generic request handlers.</returns>
    public static string GenerateDispatchGeneric(List<RequestHandlerTarget> targets)
    {
        var relevant = targets
            .Where(t => t.Kind == "GenericRequest")
            .ToList();

        if (relevant.Count == 0)
            return """
                   public ValueTask<TResponse> Dispatch<TResponse>(IRequest<TResponse> request, IServiceProvider provider, CancellationToken ct = default)
                       => throw new NotImplementedException("No IRequest<T> handlers found.");
                   """;

        var cases = string.Join("\n", relevant.Select((t, index) => $$"""
                                                                         {{t.RequestType}} r{{index}} => (TResponse)(object)(await provider.GetRequiredService<IRequestHandler<{{t.RequestType}}, {{t.ResponseType}}>>().Handle(r{{index}}, ct)),
                                                                      """));

        return $$"""
                 public async ValueTask<TResponse> Dispatch<TResponse>(IRequest<TResponse> request, IServiceProvider provider, CancellationToken ct = default)
                 {
                     return request switch
                     {
                         {{cases}}
                         _ => throw new InvalidOperationException($"No handler found for request type '{request.GetType().FullName}'")
                     };
                 }
                 """;
    }

    /// <summary>
    /// Generates the dispatch method implementation for handling requests of type "VoidRequest".
    /// </summary>
    /// <param name="targets">A list of request handler targets that define the request and handler mappings.</param>
    /// <returns>Returns a string containing the generated dispatch method for void-request handlers.</returns>
    public static string GenerateDispatchVoid(List<RequestHandlerTarget> targets)
    {
        var relevant = targets
            .Where(t => t.Kind == "VoidRequest")
            .ToList();

        if (relevant.Count == 0)
            return """
                   public ValueTask DispatchVoid(IRequest request, IServiceProvider provider, CancellationToken ct = default)
                       => throw new NotImplementedException("No IRequest handlers found.");
                   """;

        var cases = string.Join("\n", relevant.Select(t => $$"""
                                                               case "{{t.RequestType}}": await provider.GetRequiredService<IRequestHandler<{{t.RequestType}}>>().Handle(({{t.RequestType}})request, ct);
                                                                break;
                                                             """));

        return $$"""
                 public async ValueTask DispatchVoid(IRequest request, IServiceProvider provider, CancellationToken ct = default)
                 {
                     switch (request.GetType().FullName)
                     {
                         {{cases}}
                         default:
                             throw new InvalidOperationException($"No handler found for request type '{request.GetType().FullName}'");
                     }
                 }
                 """;
    }

    /// <summary>
    /// Generates the body of a method that constructs a dispatching object for handling requests in the Mediator pattern.
    /// </summary>
    /// <param name="targets">A list of request handler targets defining the request type, response type, and handler information.</param>
    /// <returns>A string representing the generated dispatcher method code for a specific request handler configuration.</returns>
    public static string GenerateDispatchObject(List<RequestHandlerTarget> targets)
    {
        return """
               public async ValueTask<object?> Dispatch(object request, IServiceProvider provider, CancellationToken ct = default)
               {
                   if (request is null)
                       throw new ArgumentNullException(nameof(request));

                   var type = request.GetType();

                   // IRequest<T>
                   var requestInterface = type
                       .GetInterfaces()
                       .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

                   if (requestInterface is not null)
                   {
                       var responseType = requestInterface.GetGenericArguments()[0];
                       var method = typeof(IOrchestrixDispatcher)
                           .GetMethod(nameof(DispatchGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!
                           .MakeGenericMethod(type, responseType);

                       return await (ValueTask<object?>)method.Invoke(this, new object[] { request, provider, ct })!;
                   }

                   // IRequest (non-generic)
                   if (request is IRequest nonGeneric)
                   {
                       await DispatchVoid(nonGeneric, provider, ct);
                       return null;
                   }

                   throw new InvalidOperationException($"Invalid request type: {type}");
               }

               private async ValueTask<object?> DispatchGeneric<TRequest, TResponse>(TRequest request, IServiceProvider provider, CancellationToken ct)
                   where TRequest : IRequest<TResponse>
               {
                   var result = await Dispatch<TResponse>(request, provider, ct);
                   return (object?)result;
               }
               """;
    }

    /// <summary>
    /// Generates the implementation of a method for dispatching and publishing notifications
    /// based on the provided list of request handler targets.
    /// </summary>
    /// <param name="targets">A list of request handler targets containing metadata about request types, response types, handler types, and handling kind.</param>
    /// <returns>A string representing the generated implementation for the notification dispatch and publish logic.</returns>
    public static string GenerateDispatchPublish(List<RequestHandlerTarget> targets)
    {
        var notifications = targets
            .Where(t => t.Kind is "Notification" or "ParallelNotification")
            .GroupBy(t => t.RequestType)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Sequential = g.Where(t => t.Kind == "Notification").Select(t => t.HandlerType!).Distinct().ToList(),
                    Parallel = g.Where(t => t.Kind == "ParallelNotification").Select(t => t.HandlerType!).Distinct()
                        .ToList()
                });

        if (notifications.Count == 0)
        {
            return """
                   public ValueTask DispatchPublish(object notification, IServiceProvider provider, CancellationToken ct = default)
                       => throw new NotImplementedException("No INotification handlers found.");
                   """;
        }

        var cases = string.Join("\n\n", notifications.Select(kvp =>
        {
            var notifType = kvp.Key;

            var seqBlock = $$"""
                                 foreach (var handler in provider.GetServices<INotificationHandler<{{notifType}}>>()
                                          .Where(h => h is not IParallelNotificationHandler<{{notifType}}>))
                                 {
                                     await handler.Handle(typed, ct);
                                 }
                             """;

            var parallelBlock = kvp.Value.Parallel.Count > 0
                ? $$"""
                        var parallelHandlers = provider.GetServices<IParallelNotificationHandler<{{notifType}}>>()
                            .Select(h => h.Handle(typed, ct).AsTask());
                        await Task.WhenAll(parallelHandlers);
                    """
                : "";

            return $$"""
                         case {{notifType}} typed:
                         {
                             {{seqBlock}}
                             {{parallelBlock}}
                             break;
                         }
                     """;
        }));

        return $$"""
                 public async ValueTask DispatchPublish(object notification, IServiceProvider provider, CancellationToken ct = default)
                 {
                     switch (notification)
                     {
                     {{cases}}
                         default:
                             throw new InvalidOperationException($"No handler found for notification type '{notification.GetType().FullName}'");
                     }
                 }
                 """;
    }

    /// <summary>
    /// Generates the implementation of a dispatch-publish method for generic notifications.
    /// This method handles different kinds of notifications (sequential and parallel), grouping them
    /// based on their target request type and producing appropriate dispatch logic for each case.
    /// </summary>
    /// <param name="targets">A list of request handler targets, each containing metadata about the request type,
    /// response type, handler type, and kind of notification (e.g., sequential or parallel).</param>
    /// <returns>
    /// A string containing the generated C# code for a dispatch-publish generic method.
    /// The generated method allows publishing notifications to corresponding handlers dynamically based on the notification type.
    /// </returns>
    public static string GenerateDispatchPublishGeneric(List<RequestHandlerTarget> targets)
    {
        var notifications = targets
            .Where(t => t.Kind is "Notification" or "ParallelNotification")
            .GroupBy(t => t.RequestType)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Sequential = g.Where(t => t.Kind == "Notification").Select(t => t.HandlerType!).Distinct().ToList(),
                    Parallel = g.Where(t => t.Kind == "ParallelNotification").Select(t => t.HandlerType!).Distinct()
                        .ToList()
                });

        if (notifications.Count == 0)
        {
            return """
                   public ValueTask DispatchPublish<TNotification>(TNotification notification, IServiceProvider provider, CancellationToken ct = default)
                       where TNotification : INotification
                       => throw new NotImplementedException("No INotification<T> handlers found.");
                   """;
        }

        var cases = string.Join("\n\n", notifications.Select(kvp =>
        {
            var notifType = kvp.Key;

            var seqBlock = $$"""
                                 foreach (var handler in provider.GetServices<INotificationHandler<{{notifType}}>>()
                                          .Where(h => h is not IParallelNotificationHandler<{{notifType}}>))
                                 {
                                     await handler.Handle(typed, ct);
                                 }
                             """;

            var parallelBlock = kvp.Value.Parallel.Count > 0
                ? $$"""
                        var parallelHandlers = provider.GetServices<IParallelNotificationHandler<{{notifType}}>>()
                            .Select(h => h.Handle(typed, ct).AsTask());
                        await Task.WhenAll(parallelHandlers);
                    """
                : "";

            return $$"""
                         case {{notifType}} typed:
                         {
                             {{seqBlock}}
                             {{parallelBlock}}
                             break;
                         }
                     """;
        }));

        return $$"""
                 public async ValueTask DispatchPublish<TNotification>(TNotification notification, IServiceProvider provider, CancellationToken ct = default)
                     where TNotification : INotification
                 {
                     switch (notification)
                     {
                     {{cases}}
                         default:
                             throw new InvalidOperationException($"No handler found for notification type '{typeof(TNotification).FullName}'");
                     }
                 }
                 """;
    }


    /// <summary>
    /// Generates the dispatch method implementation for handling stream-based generic request types.
    /// </summary>
    /// <param name="targets">A list of request handler target mappings that define the structure and handlers for various "StreamRequest" types.</param>
    /// <returns>Returns a string containing the generated dispatch method implementation for stream request handlers.</returns>
    public static string GenerateDispatchStreamGeneric(List<RequestHandlerTarget> targets)
    {
        var streams = targets
            .Where(t => t.Kind == "StreamRequest")
            .GroupBy(t => t.RequestType)
            .ToDictionary(g => g.Key, g => g.First());

        if (streams.Count == 0)
            return """
                   public IAsyncEnumerable<TResponse> DispatchStream<TResponse>(IStreamRequest<TResponse> request, IServiceProvider provider, CancellationToken ct = default)
                       => throw new NotImplementedException("No IStreamRequest<TResponse> handlers found.");
                   """;

        return $$"""
                 public IAsyncEnumerable<TResponse> DispatchStream<TResponse>(IStreamRequest<TResponse> request, IServiceProvider provider, CancellationToken ct = default)
                 {
                     var type = request.GetType().FullName;

                     return type switch
                     {
                         {{string.Join(",\n            ", streams.Select(kvp => $$"""
                                 "{{kvp.Key}}" => (IAsyncEnumerable<TResponse>)provider
                                     .GetRequiredService<IStreamRequestHandler<{{kvp.Key}}, {{kvp.Value.ResponseType}}>>()
                                     .Handle(({{kvp.Key}})request, ct)
                               """))}},
                         _ => throw new InvalidOperationException($"No stream handler found for request type '{type}'")
                     };
                 }
                 """;
    }


    /// <summary>
    /// Generates the dispatch method implementation for handling stream request types.
    /// </summary>
    /// <param name="targets">A list of request handler targets that define the mappings for various "StreamRequest" handlers.</param>
    /// <returns>Returns a string containing the generated dispatch method implementation for stream request handlers.</returns>
    public static string GenerateDispatchStreamObject(List<RequestHandlerTarget> targets)
    {
        var streams = targets
            .Where(t => t.Kind == "StreamRequest")
            .GroupBy(t => t.RequestType)
            .ToDictionary(g => g.Key, g => g.First());

        if (streams.Count == 0)
            return """
                   public IAsyncEnumerable<object?> DispatchStream(object request, IServiceProvider provider, CancellationToken ct = default)
                       => throw new NotImplementedException("No IStreamRequest handlers found.");
                   """;

        return $$"""
                 public IAsyncEnumerable<object?> DispatchStream(object request, IServiceProvider provider, CancellationToken ct = default)
                 {
                     var type = request.GetType().FullName;

                     return type switch
                     {
                         {{string.Join(",\n            ", streams.Select(kvp => $$"""
                               "{{kvp.Key}}" => WrapStream<{{kvp.Value.ResponseType}}>(provider.GetRequiredService<IStreamRequestHandler<{{kvp.Key}}, {{kvp.Value.ResponseType}}>>().Handle(({{kvp.Key}})request, ct))
                               """))}},
                         _ => throw new InvalidOperationException($"No stream handler found for request type '{type}'")
                     };
                 }

                 private static async IAsyncEnumerable<object?> WrapStream<T>(IAsyncEnumerable<T> stream)
                 {
                     await foreach (var item in stream)
                     {
                         yield return item;
                     }
                 }
                 """;
    }
}