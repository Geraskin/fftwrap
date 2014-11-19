using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FftWrap.Numerics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMatrix<T> where T : struct
    {
        private readonly int _nx;
        private readonly int _ny;
        private readonly IntPtr _ptr;

        public static readonly int ElementSize = Marshal.SizeOf(typeof(T));

        public NativeMatrix(IntPtr ptr, int nx, int ny)
        {
            _nx = nx;
            _ny = ny;
            _ptr = ptr;
        }

        public IntPtr Ptr { get { return _ptr; } }
        public int Nx { get { return _nx; } }
        public int Ny { get { return _ny; } }

        public T this[int i, int j]
        {
            get { return GetValue(i, j); }
            set { SetValue(i, j, value); }
        }

        private void SetValue(int i, int j, T value)
        {
            if (i < 0 || i >= Nx || j < 0 || j >= Ny)
                throw new ArgumentOutOfRangeException();

            int shift = CalculateShift(i, j);

            Marshal.StructureToPtr(value, (IntPtr.Add(_ptr, shift)), false);
        }

        private T GetValue(int i, int j)
        {
            if (i < 0 || i >= Nx || j < 0 || j >= Ny)
                throw new ArgumentOutOfRangeException();

            int shift = CalculateShift(i, j);

            return (T)Marshal.PtrToStructure(IntPtr.Add(_ptr, shift), typeof(SingleComplex));
        }

        private int CalculateShift(int i, int j)
        {
            // C-style indexing
            return (j * _nx + i) * ElementSize;
        }
    }
}
