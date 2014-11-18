using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FftWrap.Codegen
{
    public static class RoslynHelper
    {
        public static IdentifierNameSyntax ToIdentifierName(this string str)
        {
            return SyntaxFactory.IdentifierName(str);
        }

        public static NamespaceDeclarationSyntax ToNamespaceDeclaration(this string str)
        {
            return SyntaxFactory.NamespaceDeclaration(str.ToIdentifierName());
        }

        public static UsingDirectiveSyntax ToUsingDirective(this string str)
        {
            return SyntaxFactory.UsingDirective(str.ToIdentifierName());
        }
    }
}
