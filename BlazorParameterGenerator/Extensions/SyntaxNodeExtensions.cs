using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace BlazorParameterGenerator.Extensions;

internal static class SyntaxNodeExtensions
{
    private const string PARAMETER_ATTRIBUTE_NAME = "Parameter";
    private const string PARAMETER_ATTRIBUTE_FULL_NAME = $"{PARAMETER_ATTRIBUTE_NAME}Attribute";

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
            .Any(a => a.HasParameterAttribute());
    }

    public static bool HasParameterAttribute(this AttributeListSyntax attributeListSyntax)
    {
        return attributeListSyntax
            .Attributes
            .Any(at => at.Name.ToString() == PARAMETER_ATTRIBUTE_NAME || at.Name.ToString() == PARAMETER_ATTRIBUTE_FULL_NAME);
    }

    public static bool HasCaptureUnmatchedValuesParameterAttribute(
        this PropertyDeclarationSyntax propertyDeclarationSyntax,
        bool hasParameterAttribute = false)
    {
        return propertyDeclarationSyntax
            .AttributeLists
            .Where(a => hasParameterAttribute || a.HasParameterAttribute())
            .Any(a => a
                .Attributes
                .Any(at => at.HasParameter("CaptureUnmatchedValues", SyntaxKind.FalseLiteralExpression)));
    }

    public static bool HasParameter(
        this AttributeSyntax attributeSyntax,
        string attributeName,
        SyntaxKind expressionSyntaxKind)
    {
        return attributeSyntax
            .ArgumentList?
            .Arguments
            .Any(ar => ar.GetName() == attributeName && ar.Expression.IsKind(expressionSyntaxKind)) ?? false;
    }

    public static string? GetName(
        this AttributeArgumentSyntax attributeArgumentSyntax)
    {
        return attributeArgumentSyntax.NameEquals?.Name.ToString();
    }
}