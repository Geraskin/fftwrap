using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace FftWrap.Codegen
{
    public static class FftwHeaderParser
    {
        public static IReadOnlyCollection<Method> ParseMethodsMpi(string headerPath)
        {
            string str = File.ReadAllText(headerPath);

            // dirty removing "noise"
            str = Regex.Replace(str, @"\\", "");
            str = Regex.Replace(str, @"\s\s+", " ");
            str = Regex.Replace(str, @"const", "");

            //Console.WriteLine(str);

            const string pattern = @"FFTW_EXTERN\s+" +
                                   @"(?<return>(X\(plan\))|([\w]+))\s+" +
                                   @"(?<pointer>[*]*)" +
                                   @"(?<name>XM\(\w+\))\s*" +
                                   @"[(](?<params>[(),*\w\s]*)[)];";

            var matches = Regex.Matches(str, pattern);

            var methods = new List<Method>();

            foreach (Match match in matches)
            {
                var returnType = match.Groups["return"].ToString();
                var name = match.Groups["name"].ToString();
                var parameters = match.Groups["params"];
                var isPointer = !string.IsNullOrEmpty(match.Groups["pointer"].ToString());
                
                var parametersCollection = ParseParameters(parameters.ToString());

                methods.Add(new Method(returnType, name, isPointer, parametersCollection));
            }

            return new ReadOnlyCollection<Method>(methods);
        }
        
        
        public static IReadOnlyCollection<Method> ParseMethods(string headerPath)
        {
            string str = File.ReadAllText(headerPath);

            // dirty removing "noise"
            str = Regex.Replace(str, @"\\", "");
            str = Regex.Replace(str, @"\s\s+", " ");
            str = Regex.Replace(str, @"const", "");

            const string pattern = @"FFTW_EXTERN\s+" +
                                   @"(?<return>[(\w)]+)\s+" +
                                   @"(?<pointer>[*]*)" +
                                   @"(?<name>X[(]\w+[)])" +
                                   @"[(](?<params>[(),*\w\s]*)[)];";

            var matches = Regex.Matches(str, pattern);

            var methods = new List<Method>();

            foreach (Match match in matches)
            {
                var returnType = match.Groups["return"].ToString();
                var name = match.Groups["name"].ToString();
                var parameters = match.Groups["params"];
                var isPointer = !string.IsNullOrEmpty(match.Groups["pointer"].ToString());

                var parametersCollection = ParseParameters(parameters.ToString());

                methods.Add(new Method(returnType, name, isPointer, parametersCollection));
            }

            return new ReadOnlyCollection<Method>(methods);
        }

        private static IReadOnlyCollection<Parameter> ParseParameters(string parameters)
        {
            const string pattern = @"(?<type>[()\w]+)\s*" +
                                   @"(?<pointer>[*]*)" +
                                   @"(?<name>\w*)";

            var matches = Regex.Matches(parameters, pattern);

            var result = new List<Parameter>();

            foreach (Match match in matches)
            {
                var type = match.Groups["type"].ToString();
                var name = match.Groups["name"].ToString();
                var isPointer = !string.IsNullOrEmpty(match.Groups["pointer"].ToString());

                result.Add(new Parameter(type, name, isPointer));
            }

            return new ReadOnlyCollection<Parameter>(result);
        }
    }
}
