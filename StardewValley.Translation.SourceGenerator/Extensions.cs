using System.CodeDom.Compiler;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StardewValley.Translation.SourceGenerator;


internal static class Extensions
{
    // determine the namespace the class/enum/struct is declared in, if any
    private static string GetNamespace(SyntaxNode syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        string nameSpace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        var potentialNamespaceParent = syntax.Parent;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent != null &&
                potentialNamespaceParent is not NamespaceDeclarationSyntax
                && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is not BaseNamespaceDeclarationSyntax namespaceParent) return nameSpace;
        
        // We have a namespace. Use that as the type
        nameSpace = namespaceParent.Name.ToString();

        // Keep moving "out" of the namespace declarations until we 
        // run out of nested namespace declarations
        while (namespaceParent.Parent is NamespaceDeclarationSyntax parent)
        {
            // Add the outer namespace as a prefix to the final namespace
            nameSpace = $"{namespaceParent.Name}.{nameSpace}";
            namespaceParent = parent;
        }

        // return the final namespace
        return nameSpace;
    }

    public static string GetResource(TypeDeclarationSyntax syntax, Func<ParentClass, string> targetClassImplementation)
    {
        string ns = GetNamespace(syntax);
        var parentClass = ParentClass.GetParentClasses(syntax);
        var baseTextWriter = new StringWriter();

        var writer = new IndentedTextWriter(baseTextWriter, " ");

        // If we don't have a namespace, generate the code in the "default"
        // namespace, either global:: or a different <RootNamespace>
        if (!string.IsNullOrEmpty(ns))
        {
            writer.WriteLine($"namespace {ns};");
            writer.WriteLine();
        }

        // Loop through the full parent type hierarchy, starting with the outermost
        while (parentClass.Child is not null)
        {
            writer.Indent++;
            writer.WriteLine($"partial {parentClass.Keyword} {parentClass.Name} {parentClass.Constraints}");
            writer.WriteLine("{");
            
            parentClass = parentClass.Child; // repeat with the next child
        }

        string res = targetClassImplementation(parentClass);
        using (var reader = new StringReader(res))
        {
            while (reader.ReadLine() is { } line)
            {
                writer.WriteLine(line);
            }
        }

        // We need to "close" each of the parent types, so write
        // the required number of '}'
        while (writer.Indent > 0)
        {
            writer.WriteLine("}");
            writer.Indent--;
        }

        return baseTextWriter.ToString();
    }
}