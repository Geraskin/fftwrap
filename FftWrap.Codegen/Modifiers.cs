﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace FftWrap.Codegen
{
    public static class Modifiers
    {
        public static SyntaxToken Partial = SyntaxFactory.Token(SyntaxKind.PartialKeyword);
        public static SyntaxToken Static = SyntaxFactory.Token(SyntaxKind.StaticKeyword);
        public static SyntaxToken Extern = SyntaxFactory.Token(SyntaxKind.ExternKeyword);
        public static SyntaxToken Semicolon = SyntaxFactory.Token(SyntaxKind.SemicolonToken);
    }
}