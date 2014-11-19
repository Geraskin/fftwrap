﻿using System;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace FftWrap.Codegen
{
    public static class Utils
    {
        public const string SinglePecisionPrefix = @"fftwf";
        public const string DoublePecisionPrefix = @"fftw";
        
        public static string NameToCSharp(this Method origin)
        {
            string coreName = ExtractCoreName(origin.Name);

            coreName = coreName.Replace('_', ' ');
            coreName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(coreName);
            coreName = coreName.Replace(" ", "");

            return coreName;
        }

        public static string NameToCSharp(this Parameter origin)
        {
            if (origin.Name == "in")
                return @"src";

            if (origin.Name == "out")
                return @"dst";

            if (string.IsNullOrEmpty(origin.Name))
                return @"";

            var name = origin.Name;
            
            name = name.Replace('_', ' ');
            name = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name);
            name = name.Replace(" ", "");

            var first = Char.ToLower(name.First());

            name = name.Replace("Howmany", "howMany");

            return first + name.Substring(1);
        }

        public static string NameToNativeSinglePrecision(this Method origin)
        {
            string coreName = ExtractCoreName(origin.Name);

            return SinglePecisionPrefix + "_" + coreName;
        }

        public static string NameToNativeDoublePrecision(this Method origin)
        {
            string coreName = ExtractCoreName(origin.Name);

            return DoublePecisionPrefix + "_" + coreName;
        }

        private static string ExtractCoreName(string name)
        {
            return Regex.Match(name, @"X[(](?<core>\w+)[)]").Groups["core"].ToString();
        }

        public static string TypeNameToCSharp(this Method origin)
        {
            if (origin.ReturnTypeIsPointer)
                return "IntPtr";

            if (origin.ReturnType == @"X(plan)")
                return "IntPtr";

            return origin.ReturnType;
        }

        public static string TypeNameToCSharp(this Parameter origin)
        {
            if (origin.IsPointer)
                return @"IntPtr";

            if (origin.Type.StartsWith(@"X("))
                return @"IntPtr";

            if (origin.Type == @"unsigned")
                return @"uint";

            if (origin.Type == @"void")
                return @"";

            if (origin.Type == @"size_t")
                return @"IntPtr";

            return origin.Type;
        }
    }
}