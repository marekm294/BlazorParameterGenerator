using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using BlazorParameterGeneratorAttributes;
using System;
using BlazorParameterGenerator.Extensions;

namespace BlazorParameterGenerator;

internal sealed class ParametersSettableSyntaxReceiver : ISyntaxReceiver
{
    private const string ATTRIBUTE_NAME = nameof(ParametersSettableAttribute);
    private static string ATTRIBUTE_SUBSTRING_NAME= ATTRIBUTE_NAME.Substring(0, ATTRIBUTE_NAME.Length - nameof(Attribute).Length);

    public List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax classDeclaration || classDeclaration.IsPartial() is false)
        {
            return;
        }

        if (classDeclaration
            .AttributeLists
            .Any(a => a
                .Attributes
                .Any(at => at.Name.ToString() == ATTRIBUTE_NAME || at.Name.ToString() == ATTRIBUTE_SUBSTRING_NAME)))
        {
            CandidateClasses.Add(classDeclaration);
        }
    }
}