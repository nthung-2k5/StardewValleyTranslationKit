using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace StardewValley.Translation.SourceGenerator;


internal static class Extensions
{
    // determine the namespace the class/enum/struct is declared in, if any
    public static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        string nameSpace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        SyntaxNode potentialNamespaceParent = syntax.Parent;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent != null &&
                potentialNamespaceParent is not NamespaceDeclarationSyntax
                && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            nameSpace = namespaceParent.Name.ToString();

            // Keep moving "out" of the namespace declarations until we 
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                // Add the outer namespace as a prefix to the final namespace
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return nameSpace;
    }

    public static string GetResource(TypeDeclarationSyntax syntax, Func<ParentClass, string> InnerClassImpl)
    {
        var ns = GetNamespace(syntax);
        var parentClass = ParentClass.GetParentClasses(syntax);

        var sb = new StringBuilder();
        int parentsCount = 0;

        // If we don't have a namespace, generate the code in the "default"
        // namespace, either global:: or a different <RootNamespace>
        if (!string.IsNullOrEmpty(ns))
        {
            sb.AppendFormat("namespace {0};", ns).AppendLine().AppendLine();
        }

        // Loop through the full parent type hiearchy, starting with the outermost
        while (parentClass?.Child is not null)
        {
            sb.Append(' ', parentsCount * 4).Append("partial ")
              .Append(parentClass.Keyword) // e.g. class/struct/record
              .Append(' ')
              .Append(parentClass.Name) // e.g. Outer/Generic<T>
              .Append(' ')
              .Append(parentClass.Constraints) // e.g. where T: new()
              .AppendLine(@"
        {");
            parentsCount++; // keep track of how many layers deep we are
            parentClass = parentClass.Child; // repeat with the next child
        }

        var res = InnerClassImpl(parentClass!);
        using (var reader = new StringReader(res))
        {
            string line = reader.ReadLine();
            sb.Append(' ', parentsCount * 4).AppendLine(line);
        }

        // We need to "close" each of the parent types, so write
        // the required number of '}'
        for (int i = 1; i <= parentsCount; i++)
        {
            sb.Append(' ', (parentsCount - i) * 4).AppendLine("}");
        }

        return sb.ToString();
    }
}