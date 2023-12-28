using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.Text;

namespace StardewValley.Translation.SourceGenerator;
#if false
[Generator]
public class JsonClassSourceGenerator : ISourceGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var jsonDeclarations = context.SyntaxProvider
                                      .ForAttributeWithMetadataName(
                                          AttributeItem.MetadataName,
                                          predicate: static (node, _) => IsSyntaxTargetForGeneration(node),
                                          transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                                      .Where(static n => n is not null)
                                      .Collect();

        context.RegisterSourceOutput(jsonDeclarations, Execute);
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax;

    private static AttributeItem GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context) => AttributeItem.From(context);

    private static void Execute(SourceProductionContext context, ImmutableArray<AttributeItem> classes)
    {
        List<AttributeItem> distinctClasses = classes.Distinct().ToList();

        foreach (var generated in distinctClasses)
        {
            context.AddSource($"{generated.ClassType}.g.cs", SourceText.From(SourceCodeHelper.JsonPartialClassCode(generated), Encoding.UTF8));
        }

        if (distinctClasses.Count != 0)
        {
            context.AddSource("ClassTranslation.g.cs", SourceText.From(SourceCodeHelper.ClassTranslation(distinctClasses.Select(attr => (attr.ClassType, attr.BaseType))), Encoding.UTF8));
        }
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {

    }
}

internal class AttributeSyntaxReceiver : ISyntaxReceiver
{
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is AttributeSyntax attr)
        {
            Console.WriteLine("hello");
        }
    }
}
#endif
#if true
[Generator]
public class JsonClassSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var jsonDeclarations = context.SyntaxProvider
                                      .ForAttributeWithMetadataName(
                                          AttributeItem.MetadataName,
                                          predicate: static (node, _) => IsSyntaxTargetForGeneration(node),
                                          transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
                                      .Where(static n => n is not null)
                                      .Collect();

        context.RegisterSourceOutput(jsonDeclarations, Execute);
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) => node is ClassDeclarationSyntax;

    private static AttributeItem GetSemanticTargetForGeneration(GeneratorAttributeSyntaxContext context) => AttributeItem.From(context);

    private static void Execute(SourceProductionContext context, ImmutableArray<AttributeItem> classes)
    {
        List<AttributeItem> distinctClasses = classes.Distinct().ToList();

        foreach (var generated in distinctClasses)
        {
            context.AddSource($"{generated.ClassType}.g.cs", SourceText.From(SourceCodeHelper.JsonPartialClassCode(generated), Encoding.UTF8));
        }

        if (distinctClasses.Count != 0)
        {
            context.AddSource("ClassTranslation.g.cs", SourceText.From(SourceCodeHelper.ClassTranslation(distinctClasses.Select(attr => (attr.ClassType, attr.BaseType))), Encoding.UTF8));
        }
    }
}
#endif