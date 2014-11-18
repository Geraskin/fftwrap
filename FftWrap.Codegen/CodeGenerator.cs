using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace FftWrap.Codegen
{
    public class CodeGenerator
    {
        public static void GenerateCSharpCodeWithRoslyn(string path)
        {
            var cu = SyntaxFactory.CompilationUnit()
                .AddUsings("System".ToUsingDirective())
                .AddUsings("System.Generic".ToUsingDirective())
                .AddMembers("FftWrap".ToNamespaceDeclaration()
                    .AddMembers(CreateClass("UnsafeNativeMethods")))
                .NormalizeWhitespace();



            var tree = SyntaxFactory.ParseSyntaxTree("[DllImport] public extern void Test (int input, string output);");

            var root = tree.GetRoot();


            //foreach (var nodes in root.ChildNodes())
            //{
            //    Console.WriteLine("upper level [{0}]", nodes);

            //    foreach (var tt in nodes.ChildNodes())
            //        Console.WriteLine("cilds [{0}]", tt);

            //    foreach (var tt in nodes.ChildTokens())
            //        Console.WriteLine("tokens [{0}]", tt);
            

            //}
            

            using (var sw = new StreamWriter(path))
            {
                cu.WriteTo(sw);
            }

        }

        private static ClassDeclarationSyntax CreateClass(string name)
        {
            var attr = CreateAttributeSuppress();
            
            return SyntaxFactory.ClassDeclaration(name)
                .AddAttributeLists(attr)
                .AddModifiers(AccessModifiers.Internal)
                .AddModifiers(Modifiers.Static)
                .AddModifiers(Modifiers.Partial)
                .AddMembers(CreateMethod("RunTest"));
        }



        private static MethodDeclarationSyntax CreateMethod(string name)
        {

            var parameters = SyntaxFactory.ParseParameterList(@"(IntPtr src, IntPtr dst, int count)");

            var attr = CreateAttributeDllImport();

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("IntPtr"), name)
                .AddAttributeLists(attr)
                .AddModifiers(AccessModifiers.Public)
                .AddModifiers(Modifiers.Static)
                .AddModifiers(Modifiers.Extern)
                .WithParameterList(parameters)
                .WithSemicolonToken(Modifiers.Semicolon);
        }


        private static AttributeListSyntax CreateAttributeDllImport()
        {
            return
                CreateAttribute(
                    @"[DllImport(Fftw, CallingConvention = CallingConvention.Cdecl, EntryPoint = FftwSingle + ""plan_dft_1d"")]");
        }

        private static AttributeListSyntax CreateAttributeSuppress()
        {
            return
                CreateAttribute(
                    @"[SuppressUnmanagedCodeSecurityAttribute()]");
        }


        private static AttributeListSyntax CreateAttribute(string declaration)
        {
            var tree = SyntaxFactory.ParseSyntaxTree(declaration);

            return tree.GetRoot().ChildNodes().First().ChildNodes().First() as AttributeListSyntax;
        }

    }
}
