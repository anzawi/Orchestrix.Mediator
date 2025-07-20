using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Orchestrix.Mediator.SourceGenerators;

/// <summary>
/// Provides utility methods for analyzing symbols to extract specific target information
/// related to request and notification handlers in the context of the Orchestrix.Mediator library.
/// </summary>
internal static class SymbolAnalyzer
{
    /// <summary>
    /// Extracts request handler targets from a given set of symbols. This method identifies
    /// types that implement specific handler interfaces (e.g., IRequestHandler, INotificationHandler)
    /// and extracts relevant metadata about the request, response, and handler types.
    /// </summary>
    /// <param name="symbols">A collection of symbols to analyze for request handler targets.</param>
    /// <returns>A list of <see cref="RequestHandlerTarget"/> objects, each representing a handler target
    /// with associated request, response, handler type, and kind information.</returns>
    public static List<RequestHandlerTarget> ExtractRequestHandlerTargets(IEnumerable<ISymbol> symbols)
    {
        return symbols
            .OfType<INamedTypeSymbol>()
            .Where(s => !s.IsAbstract && s.TypeKind == TypeKind.Class)
            .SelectMany(sym =>
            {
                var interfaces = sym.AllInterfaces;
                var targets = new List<RequestHandlerTarget>();

                foreach (var i in interfaces)
                {
                    var iface = i.OriginalDefinition.ToDisplayString();

                    // IRequestHandler<TRequest, TResponse>
                    if (iface == "Orchestrix.Mediator.IRequestHandler<TRequest, TResponse>")
                    {
                        var req = i.TypeArguments[0].ToDisplayString();
                        var resp = i.TypeArguments[1].ToDisplayString();

                        targets.Add(new RequestHandlerTarget(
                            RequestType: req,
                            ResponseType: resp,
                            HandlerType: sym.ToDisplayString(),
                            Kind: "GenericRequest"));
                    }

                    // IRequestHandler<TRequest>
                    if (iface == "Orchestrix.Mediator.IRequestHandler<TRequest>")
                    {
                        var req = i.TypeArguments[0].ToDisplayString();

                        targets.Add(new RequestHandlerTarget(
                            RequestType: req,
                            ResponseType: null,
                            HandlerType: sym.ToDisplayString(),
                            Kind: "VoidRequest"));
                    }

                    // IStreamRequestHandler<TRequest, TResponse>
                    if (iface == "Orchestrix.Mediator.IStreamRequestHandler<TRequest, TResponse>")
                    {
                        var req = i.TypeArguments[0].ToDisplayString();
                        var resp = i.TypeArguments[1].ToDisplayString();

                        targets.Add(new RequestHandlerTarget(
                            RequestType: req,
                            ResponseType: resp,
                            HandlerType: sym.ToDisplayString(),
                            Kind: "StreamRequest"));
                    }

                    // INotificationHandler<T>
                    if (iface == "Orchestrix.Mediator.INotificationHandler<TNotification>")
                    {
                        var notif = i.TypeArguments[0].ToDisplayString();

                        targets.Add(new RequestHandlerTarget(
                            RequestType: notif,
                            ResponseType: null,
                            HandlerType: sym.ToDisplayString(),
                            Kind: "Notification"));
                    }

                    // IParallelNotificationHandler<T>
                    if (iface == "Orchestrix.Mediator.IParallelNotificationHandler<TNotification>")
                    {
                        var notif = i.TypeArguments[0].ToDisplayString();

                        targets.Add(new RequestHandlerTarget(
                            RequestType: notif,
                            ResponseType: null,
                            HandlerType: sym.ToDisplayString(),
                            Kind: "ParallelNotification",
                            IsParallel: true));
                    }
                }

                return targets;
            })
            .ToList();
    }
}
