using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Orchestrix.Mediator.SourceGenerators;

/// <summary>
/// Provides functionality for dynamically generating dispatcher code for request handlers.
/// </summary>
/// <remarks>
/// This class is responsible for generating the C# source code of a dispatcher class
/// based on a collection of request handler targets. The generated code is used to implement
/// the <c>IOrchestrixDispatcher</c> interface, which facilitates the dispatching of
/// various types of requests and responses.
/// The dispatcher includes methods for dispatching generic, void, and object-based
/// requests and responses, as well as publish and streaming operations.
/// </remarks>
/// <example>
/// The generated dispatcher can be integrated into applications using dependency injection
/// to enable efficient and dynamic handling of requests.
/// </example>
public static class DispatcherGenerator
{
    /// <summary>
    /// Generates source code for a dispatcher class based on a list of request handler targets.
    /// </summary>
    /// <param name="targets">A list of <see cref="RequestHandlerTarget"/> instances representing the request handlers to be included in the dispatcher.</param>
    /// <returns>
    /// A string containing the generated C# source code for the dispatcher class.
    /// If the input contains no valid targets, an empty string is returned.
    /// </returns>
    public static string Generate(List<RequestHandlerTarget> targets)
    {
        targets = targets.Where(x => x is not null).ToList();
        if (targets.Count == 0)
            return string.Empty;

        var body = $$"""
                     #nullable enable
                     using global::System;
                     using global::System.Linq;
                     using global::System.Threading;
                     using global::System.Threading.Tasks;
                     using global::System.Collections.Generic;
                     using global::Microsoft.Extensions.DependencyInjection;
                     using global::Orchestrix.Mediator;
                     using global::System.Reflection;
                     namespace Orchestrix.Mediator.Generated;

                     public sealed class GeneratedDispatcher : IOrchestrixDispatcher
                     {
                         {{DispatcherBodyBuilder.GenerateDispatchGeneric(targets!)}}
                         {{DispatcherBodyBuilder.GenerateDispatchVoid(targets!)}}
                         {{DispatcherBodyBuilder.GenerateDispatchObject(targets!)}}
                         {{DispatcherBodyBuilder.GenerateDispatchPublish(targets!)}}
                         {{DispatcherBodyBuilder.GenerateDispatchPublishGeneric(targets!)}}
                         {{DispatcherBodyBuilder.GenerateDispatchStreamGeneric(targets!)}}
                         {{DispatcherBodyBuilder.GenerateDispatchStreamObject(targets!)}}
                     }
                     """;

        var tree = CSharpSyntaxTree.ParseText(body);
        return tree.GetRoot().NormalizeWhitespace().ToFullString();
    }
}