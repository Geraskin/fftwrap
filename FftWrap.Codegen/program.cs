using System;
using System.Collections.Generic;
using FftWrap.Codegen;

namespace FftWrap.Examples
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            GenerateMpiHeaderWraper();
            //GenerateHeaderWraper();
        }


        private static void GenerateHeaderWraper()
        {
            var methods = FftwHeaderParser.ParseMethods(@"..\..\Headers\fftw3.h");

            //PrintMethods(methods);

            CodeGenerator.GenerateCSharpCodeWithRoslyn(
                path: @"..\..\..\FftWrap\Fftw.cs", 
                className:@"Fftwf", 
                dllName: @"""libfftw3f-3.dll""", 
                methods: methods);
        }

        private static void GenerateMpiHeaderWraper()
        {
            var methods = FftwHeaderParser.ParseMethodsMpi(@"..\..\Headers\fftw3-mpi.h");

            //PrintMethods(methods);

            CodeGenerator.GenerateMpiCSharpCodeWithRoslyn(
                path: @"..\..\..\FftWrap\FftwMpi.cs", 
                className:@"FftwfMpi", 
                dllName: @"""libfftw3f-3.dll""", 
                methods: methods);
        }


        private static void PrintMethods(IReadOnlyCollection<Method> methods)
        {
            foreach (var method in methods)
            {
                var name = method.NameToCSharp();
                var type = method.TypeNameToCSharp();

                Console.WriteLine("\n{0} {1}", type, name);

                foreach (var parameter in method.Parameters)
                {
                    var ptype = parameter.TypeNameToCSharp();
                    Console.WriteLine("\t{0} {1}", ptype, parameter.Name);
                }
            }

            Console.WriteLine("Total {0} methods", methods.Count);
        }
    }
}
