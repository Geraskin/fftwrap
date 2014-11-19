using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using FftWrap.Codegen;

namespace FftWrap.Examples
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var methods = FftwHeaderParser.ParseMethods(@"..\..\..\FftWrap.Codegen\Headers\fftw3.h");

            CodeGenerator.GenerateCSharpCodeWithRoslyn(@"..\..\..\FftWrap\UnsafeNativeMethods.cs", methods);

            //PrintMethods(methods);
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
