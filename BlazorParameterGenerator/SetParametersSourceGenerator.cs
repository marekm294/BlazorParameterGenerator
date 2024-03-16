using Microsoft.CodeAnalysis;
using BlazorParameterGenerator.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using BlazorParameterGenerator.Models;

namespace BlazorParameterGenerator;

[Generator]
internal sealed class SetParametersSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new ParametersSettableSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is not ParametersSettableSyntaxReceiver receiver)
        {
            return;
        }

        foreach(var classDeclaration in receiver.CandidateClasses)
        {
            var sourceCode = GenerateSource(classDeclaration);
            context
                .AddSource($"{classDeclaration.Identifier.Text}.g", SourceText.From(sourceCode, Encoding.UTF8));
        }
    }

    private static string GenerateSource(ClassDeclarationSyntax classDeclaration)
    {
        var sourceCodeBuilder = new SourceCodeBuilder();

        var hasNamespace = classDeclaration.TryGetNamespace(out var namespaceValue);
        if (hasNamespace is false)
        {
            throw new Exception("No namespace");
        }

        var properties = classDeclaration
            .Members
            .OfType<PropertyDeclarationSyntax>()
            .Where(p => p.HasPublicAccessModifier() && p.HasParameterAttribute());

        var propertyInformation = properties
            .Select(p => new CustomPropertyInformation()
            {
                PropertyType = p.Type.ToString(),
                PropertyName = p.Identifier.Text,
            })
            .ToList();

        var captureUnmatchedValuesPropertyInformation = properties
            .Where(p => p.HasCaptureUnmatchedValuesParameterAttribute(true))
            .Select(p => new CustomPropertyInformation()
            {
                PropertyType = p.Type.ToString(),
                PropertyName = p.Identifier.Text,
            })
            .FirstOrDefault();

        sourceCodeBuilder
            .AddNamespace(namespaceValue)
            .AddClass(classDeclaration.Modifiers.Select(m => m.Text), classDeclaration.Identifier.Text)
            .AddMethodContent(propertyInformation, captureUnmatchedValuesPropertyInformation);

        return sourceCodeBuilder.ToString();
    }
}
