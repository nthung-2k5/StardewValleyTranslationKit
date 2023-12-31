using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StardewValley.Translation.SourceGenerator;

[Generator]
public class TextJsonClassSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var jsonDeclarations = context.AdditionalTextsProvider
            .Where(text => text.Path.EndsWith("test.json", StringComparison.OrdinalIgnoreCase))
            .Select(static (text, tk) => text.GetText(tk)?.ToString())
            .Where(text => text is not null)!
            .Collect<string>();
        
        var compilationsAndJsons = context.CompilationProvider.Combine(jsonDeclarations);
        context.RegisterSourceOutput(compilationsAndJsons, static (ctx, tuple) => Execute(ctx, tuple.Left, tuple.Right));
    }
    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax;

    private static (TypeDeclarationSyntax, INamedTypeSymbol, ImmutableArray<AttributeData>) GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext ctx) => ((TypeDeclarationSyntax)ctx.TargetNode, (INamedTypeSymbol)ctx.TargetSymbol, ctx.Attributes);
    private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<string> classes)
    {
        var requiredClasses = JsonSerializer.Deserialize<Dictionary<string, ClassData>>(classes.First())!;
        foreach ((string jsonClass, var data) in requiredClasses)
        {
            //context.AddSource($"{generated!.ClassType}.g.cs", SourceText.From(SourceCodeHelper.JsonPartialClassCode(generated), Encoding.UTF8));
            GenerateCode(jsonClass, data, compilation);
        }

        // if (distinctClasses.Count != 0)
        // {
        //     context.AddSource("ClassTranslation.g.cs", SourceText.From(SourceCodeHelper.ClassTranslation(distinctClasses.Select(attr => (attr!.ClassType, attr.BaseType))), Encoding.UTF8));
        // }
        
    }

    private static void GenerateCode(string jsonClass, ClassData data, Compilation compilation)
    {
        var baseClass = compilation.GetTypeByMetadataName(data.BaseClass);
        
    }
}