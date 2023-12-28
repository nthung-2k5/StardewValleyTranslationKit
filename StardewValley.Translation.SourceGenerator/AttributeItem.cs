using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewValley.Translation.SourceGenerator;

internal class AttributeItem
{
    public const string MetadataName = "StardewValley.Translation.JsonClass.JsonClassAttribute`1";
    public string BaseType { get; set; } = null!;
    public string ClassType { get; set; } = null!;
    public TypeDeclarationSyntax ClassSyntax { get; set; } = null!;

    private static readonly HashSet<ITypeSymbol> UsedTypes = new();
    public static AttributeItem From(GeneratorAttributeSyntaxContext ctx)
    {
        return From((TypeDeclarationSyntax)ctx.TargetNode, (INamedTypeSymbol)ctx.TargetSymbol, ctx.Attributes[0]);
    }
    public static AttributeItem From(TypeDeclarationSyntax syntax, INamedTypeSymbol classSymbol, AttributeData attribute)
    {
        var type = GetInterfaceType(attribute.AttributeClass!);
        if (!UsedTypes.Add(type))
        {
            // TODO: Add diagnostics
            return null;
        }
        else
        {
            return new AttributeItem
            {
                ClassSyntax = syntax,
                BaseType = type.ToDisplayString(),
                ClassType = classSymbol.ToDisplayString()
            };
        }
        
    }

    private static ITypeSymbol GetInterfaceType(INamedTypeSymbol symbol) => symbol.TypeArguments[0];
}
internal enum JsonContainer
{
    Array,
    Object
}