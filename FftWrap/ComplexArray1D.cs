using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FftWrap
{
    public class ComplexArray1D : NativeArray1D<Complex>
    {
        public ComplexArray1D(IntPtr ptr, int length)
            : base(ptr, length)
        {
        }
    }
}
