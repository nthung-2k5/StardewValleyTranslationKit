using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace StardewValley.Translation.SourceGenerator;
internal class ParentClass
{
    public ParentClass(string keyword, string name, string constraints, ParentClass child)
    {
        Child = child;
        Keyword = keyword;
        Name = name;
        Constraints = constraints;
    }

    public ParentClass Child { get; }
    public string Keyword { get; }
    public string Name { get; }
    public string Constraints { get; }

    public static ParentClass GetParentClasses(BaseTypeDeclarationSyntax typeSyntax)
    {
        // Try and get the parent syntax. If it isn't a type like class/struct, this will be null
        TypeDeclarationSyntax parentSyntax = typeSyntax as TypeDeclarationSyntax;
        ParentClass parentClassInfo = null;

        // Keep looping while we're in a supported nested type
        while (parentSyntax != null && IsAllowedKind(parentSyntax.Kind()))
        {
            // Record the parent type keyword (class/struct etc), name, and constraints
            parentClassInfo = new ParentClass(
                keyword: parentSyntax.Keyword.ValueText,
                name: parentSyntax.Identifier.ToString() + parentSyntax.TypeParameterList,
                constraints: parentSyntax.ConstraintClauses.ToString(),
                child: parentClassInfo); // set the child link (null initially)

            // Move to the next outer type
            parentSyntax = (parentSyntax.Parent as TypeDeclarationSyntax);
        }

        // return a link to the outermost parent type
        return parentClassInfo;

    }

    // We can only be nested in class/struct/record
    static bool IsAllowedKind(SyntaxKind kind) =>
        kind == SyntaxKind.ClassDeclaration ||
        kind == SyntaxKind.StructDeclaration ||
        kind == SyntaxKind.RecordDeclaration;
}
