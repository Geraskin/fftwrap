using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
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
            int length = 4;

            var size = Marshal.SizeOf(typeof(Complex));


            IntPtr srcPtr = Fftwf.Malloc((IntPtr)(length * size));
            IntPtr dstPtr = Fftwf.Malloc((IntPtr)(length * size));

            var src = new ComplexArray1D(srcPtr, length);
            var dst = new ComplexArray1D(dstPtr, length);

            src[0] = Complex.One;
            src[1] = 0;
            src[2] = 0;
            src[3] = 0;

            
            Console.WriteLine("src");
            src.ForEach(c => Console.WriteLine(c));

            IntPtr plan1 = Fftwf.PlanDft1D(length, srcPtr, dstPtr, (int)Direction.Forward, (uint)Flags.Estimate);
            IntPtr plan2 = Fftwf.PlanDft1D(length, dstPtr, srcPtr, (int)Direction.Backward, (uint)Flags.Estimate);

            
            

            Fftwf.Execute(plan1);

            Console.WriteLine("\ndst\n");
            dst.ForEach(c => Console.WriteLine(c));
            
            
            Fftwf.Execute(plan2);

            Console.WriteLine("\nagain src\n");
            src.ForEach(c => Console.WriteLine(c));
            



            Fftwf.DestroyPlan(plan1);
            Fftwf.DestroyPlan(plan2);

            Fftwf.Free(srcPtr);
            Fftwf.Free(dstPtr);
        }



        private static void GenerateHeaderWraper()
        {
            var methods = FftwHeaderParser.ParseMethods(@"..\..\..\FftWrap.Codegen\Headers\fftw3.h");

            //PrintMethods(methods);

            CodeGenerator.GenerateCSharpCodeWithRoslyn(@"..\..\..\FftWrap\UnsafeNativeMethods.cs", methods);
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
