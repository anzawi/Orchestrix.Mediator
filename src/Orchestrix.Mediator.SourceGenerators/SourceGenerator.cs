using System;

namespace Orchestrix.Mediator.SourceGenerators;

using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

/// <inheritdoc />
[Generator]
public class SourceGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => s is ClassDeclarationSyntax,
                transform: static (ctx, _) => ctx.SemanticModel.GetDeclaredSymbol((ClassDeclarationSyntax)ctx.Node)
            )
            .Where(symbol => symbol is not null)
            .Collect();

        context.RegisterSourceOutput(classDeclarations, (ctx, symbols) =>
        {
            var targets = SymbolAnalyzer.ExtractRequestHandlerTargets(symbols!);
            if (targets.Count == 0)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    MissingMediatorHandlerDescriptor,
                    Location.None
                ));
                return;
            }

            var code = DispatcherGenerator.Generate(targets!);
            if (!string.IsNullOrWhiteSpace(code))
            {
                ctx.AddSource("GeneratedDispatcher.g.cs", SourceText.From(code, Encoding.UTF8));
            }

            // Generate DI registration
            var registrationCode = GeneratedRegistrationGenerator.Generate(targets);
            if (!string.IsNullOrWhiteSpace(registrationCode))
            {
                ctx.AddSource("GeneratedRegistration.g.cs", SourceText.From(registrationCode, Encoding.UTF8));
            }
        });
    }
    
    private static readonly DiagnosticDescriptor MissingMediatorHandlerDescriptor = new(
        id: "ORCH001",
        title: "No Orchestrix.Mediator handlers found",
        messageFormat: "There are no Orchestrix.Mediator handlers to generate. Make sure you have at least one handler with the correct interface and that the Orchestrix.Mediator package is installed.",
        category: "Orchestrix.Mediator",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}