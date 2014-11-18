using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace FftWrap
{
    [SuppressUnmanagedCodeSecurity]
    internal static class UnsafeNativeMethods
    {
        private const string Fftw = @"libfftwf-3.3.dll";

          private const string FftwSingle = "fftwf_";

        [DllImport(Fftw, CallingConvention = CallingConvention.Cdecl, EntryPoint = FftwSingle + "plan_dft_1d")]
        public static extern IntPtr fftw_plan_dft_1d(int n, IntPtr src, IntPtr dst, int sign, Flags flags);
    }
}
