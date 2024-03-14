using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace BlazorParameterGenerator.Extensions;

internal static class SyntaxNodeExtensions
{
    public static bool TryGetNamespace(this SyntaxNode syntaxNode, out string namespaceValue)
    {
        if (syntaxNode.Parent is null)
        {
            namespaceValue = "";
            return false;
        }

        if (syntaxNode.Parent is NamespaceDeclarationSyntax namespaceDeclaration)
        {
            namespaceValue = namespaceDeclaration.Name.ToString();
            return true;
        }

        if (syntaxNode.Parent is FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDeclaration)
        {
            namespaceValue = fileScopedNamespaceDeclaration.Name.ToString();
            return true;
        }

        return TryGetNamespace(syntaxNode.Parent, out namespaceValue);
    }

    public static bool IsPartial(this ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration
            .Modifiers
            .Any(m => m.IsKind(SyntaxKind.PartialKeyword));
    }

    public static bool HasPublicAccessModifier(this PropertyDeclarationSyntax propertyDeclarationSyntax)
    {
        return propertyDeclarationSyntax
            .Modifiers
            .Any(m => m.IsKind(SyntaxKind.PublicKeyword));
    }

    public static bool HasParameterAttribute(this PropertyDeclarationSyntax propertyDeclarationSyntax)
    {
        return propertyDeclarationSyntax
            .AttributeLists
            .Any(a => a
                .Attributes
                .Any(at => at.Name.ToString() == "Parameter" || at.Name.ToString() == "ParameterAttribute"));
    }
}