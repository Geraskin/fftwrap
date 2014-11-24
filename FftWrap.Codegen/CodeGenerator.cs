﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace FftWrap.Codegen
{
    public static class CodeGenerator
    {
        private const string Comment =
@"/*  This code is autogenerated. 
      Please do not modify it manualy,
      otherwise all result could be lost
  */";

        public static void GenerateCSharpCodeWithRoslyn(string path, string className,  string dllName, IReadOnlyCollection<Method> methods)
        {
            var cu = SyntaxFactory.CompilationUnit()
                .AddUsings("System".ToUsingDirective())
                .AddUsings("System.Security".ToUsingDirective())
                .AddUsings("System.Runtime.InteropServices".ToUsingDirective())
                .AddMembers("FftWrap".ToNamespaceDeclaration()
                    .AddMembers(CreateClass(className, dllName, false, methods)))
                .NormalizeWhitespace();

            using (var sw = new StreamWriter(path))
                cu.WriteTo(sw);
        }

        public static void GenerateMpiCSharpCodeWithRoslyn(string path, string className, string dllName, IReadOnlyCollection<Method> methods)
        {
            var cu = SyntaxFactory.CompilationUnit()
                .AddUsings("System".ToUsingDirective())
                .AddUsings("System.Security".ToUsingDirective())
                .AddUsings("System.Runtime.InteropServices".ToUsingDirective())
                .AddMembers("FftWrap".ToNamespaceDeclaration()
                    .AddMembers(CreateClass(className, dllName, true, methods)))
                .NormalizeWhitespace();

            using (var sw = new StreamWriter(path))
                cu.WriteTo(sw);
        }

        private static ClassDeclarationSyntax CreateClass(string name, string dllName, bool mpi, IReadOnlyCollection<Method> methods)
        {
            var attr = CreateAttributeSuppress();

            return SyntaxFactory.ClassDeclaration(name)
                .AddAttributeLists(attr)
                .AddModifiers(AccessModifiers.Public)
                .AddModifiers(Modifiers.Static)
                .AddModifiers(Modifiers.Partial)
                .AddMembers(CreateField(dllName))
                .AddMembers(CreateMethods(methods, mpi));
        }

        private static MemberDeclarationSyntax CreateField(string dllName)
        {
            var field =
                SyntaxFactory.FieldDeclaration(
                    SyntaxFactory.VariableDeclaration(
                        SyntaxFactory.PredefinedType(Types.String)))
                    .AddVariable("NativeDllName", dllName)
                    .AddModifiers(AccessModifiers.Private)
                    .AddModifiers(Modifiers.Const);

            return field;
        }

        private static MemberDeclarationSyntax[] CreateMethods(IReadOnlyCollection<Method> methods, bool mpi)
        {
            return methods.Select(m=> CreateMethod (m, mpi))
                    .Cast<MemberDeclarationSyntax>()
                    .ToArray();
        }

        private static MethodDeclarationSyntax CreateMethod(Method method, bool mpi)
        {
            var parameters = CreateParameters(method.Parameters);

            var nativeMethodName = mpi ?
                 method.NameToNativeSinglePrecisionWithMpi() :
                 method.NameToNativeSinglePrecision();

            var attr = CreateAttributeDllImport("NativeDllName", nativeMethodName);

            var returnType = method.TypeNameToCSharp();
            var name = method.NameToCSharp();

            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(returnType), name)
                .AddAttributeLists(attr)
                .AddModifiers(AccessModifiers.Public)
                .AddModifiers(Modifiers.Static)
                .AddModifiers(Modifiers.Extern)
                .WithParameterList(parameters)
                .WithSemicolonToken(Modifiers.Semicolon);
        }

        private static ParameterListSyntax CreateParameters(IReadOnlyCollection<Parameter> parameters)
        {
            var list = new List<ParameterSyntax>();

            foreach (var parameter in parameters.Take(parameters.Count))
            {
                var type = SyntaxFactory.ParseTypeName(parameter.TypeNameToCSharp());
                var identifier = SyntaxFactory.Identifier(parameter.NameToCSharp());

                var p = SyntaxFactory.Parameter(
                    new SyntaxList<AttributeListSyntax>(),
                    new SyntaxTokenList(),
                    type,
                    identifier,
                    null);

                list.Add(p);
            }

            return SyntaxFactory.ParameterList(new SeparatedSyntaxList<ParameterSyntax>().AddRange(list));
        }

        private static AttributeListSyntax CreateAttributeDllImport(string dllName, string entryPoint)
        {
            string attribute =
                string.Format("[DllImport({0}, CallingConvention = CallingConvention.Cdecl, EntryPoint = {2}{1}{2})]",
                    dllName, entryPoint, '"');

            return CreateAttribute(attribute);
        }

        private static AttributeListSyntax CreateAttributeSuppress()
        {
            return
                CreateAttribute(
                    @"[SuppressUnmanagedCodeSecurity]");
        }


        private static AttributeListSyntax CreateAttribute(string declaration)
        {
            var tree = SyntaxFactory.ParseSyntaxTree(declaration);

            return tree.GetRoot().ChildNodes().First().ChildNodes().First() as AttributeListSyntax;
        }

    }
}
