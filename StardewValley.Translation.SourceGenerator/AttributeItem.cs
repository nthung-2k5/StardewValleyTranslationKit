using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StardewValley.Translation.SourceGenerator;

internal class AttributeItem
{
    public const string MetadataName = "StardewValley.Translation.JsonClass.JsonClassAttribute`1";
    public string BaseType { get; private set; } = null!;
    public string ClassType { get; private set; } = null!;
    public TypeDeclarationSyntax ClassSyntax { get; private set; } = null!;

    private static readonly HashSet<ITypeSymbol> UsedTypes = new(SymbolEqualityComparer.Default);
    public static void Finish() => UsedTypes.Clear();

    public static AttributeItem? From(TypeDeclarationSyntax syntax, INamedTypeSymbol classSymbol, AttributeData attribute)
    {
        var type = attribute.AttributeClass!.TypeArguments[0];
        if (!UsedTypes.Add(type))
        {
            // TODO: Add diagnostics
            return null;
        }
        
        return new AttributeItem
        {
            ClassSyntax = syntax,
            BaseType = type.ToDisplayString(),
            ClassType = classSymbol.ToDisplayString()
        };
        
    }
}
internal enum JsonContainer
{
    Array,
    Object
}