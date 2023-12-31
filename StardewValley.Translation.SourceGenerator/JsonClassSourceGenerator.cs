using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.Editing;

namespace StardewValley.Translation.SourceGenerator;

[Generator]
public class JsonClassSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var jsonDeclarations = context.SyntaxProvider
                                      .ForAttributeWithMetadataName(
                                          AttributeItem.MetadataName,
                                          static (node, _) => IsSyntaxTargetForGeneration(node),
                                          static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                                      .SelectMany(static (info, _) => GetImmutableDataForGeneration(info))
                                      .Select(static (info, _) => AttributeItem.From(info.Item1, info.Item2, info.Item3))
                                      .Collect();
        
        context.RegisterSourceOutput(jsonDeclarations, Execute);
    }

    private static ImmutableArray<(TypeDeclarationSyntax, INamedTypeSymbol, AttributeData)> GetImmutableDataForGeneration((TypeDeclarationSyntax Type, INamedTypeSymbol Symbol, ImmutableArray<AttributeData> Attributes) info)
    {
        var arr = ImmutableArray.CreateBuilder<(TypeDeclarationSyntax, INamedTypeSymbol, AttributeData)>(info.Attributes.Length);

        foreach (var attr in info.Attributes)
        {
            arr.Add((info.Type, info.Symbol, attr));
        }

        return arr.ToImmutable();
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax;

    private static (TypeDeclarationSyntax, INamedTypeSymbol, ImmutableArray<AttributeData>) GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext ctx) => ((TypeDeclarationSyntax)ctx.TargetNode, (INamedTypeSymbol)ctx.TargetSymbol, ctx.Attributes);

    private static void Execute(SourceProductionContext context, ImmutableArray<AttributeItem?> classes)
    {
        var distinctClasses = classes.Where(item => item is not null).Distinct().ToList();
        foreach (var generated in distinctClasses)
        {
            context.AddSource($"{generated!.ClassType}.g.cs", SourceText.From(SourceCodeHelper.JsonPartialClassCode(generated), Encoding.UTF8));
        }

        if (distinctClasses.Count != 0)
        {
            context.AddSource("ClassTranslation.g.cs", SourceText.From(SourceCodeHelper.ClassTranslation(distinctClasses.Select(attr => (attr!.ClassType, attr.BaseType))), Encoding.UTF8));
        }
        AttributeItem.Finish();
    }
}