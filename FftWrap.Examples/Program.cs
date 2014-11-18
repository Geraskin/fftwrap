using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FftWrap.Codegen;

namespace FftWrap.Examples
{
    public class Program
    {
        private static void Main(string[] args)
        {

            CodeGenerator.GenerateCSharpCodeWithRoslyn(@"UnsafeNativeMethods.cs");
        }
    }
}
