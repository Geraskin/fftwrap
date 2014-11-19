using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FftWrap
{
    public class NativeArray1D<T> where T : struct
    {
        static readonly int Size = Marshal.SizeOf(typeof(T));

        private readonly int _length;
        private readonly IntPtr _ptr;

        public NativeArray1D(IntPtr ptr, int length)
        {
            _ptr = ptr;
            _length = length;
        }

        public int Length
        {
            get { return _length; }
        }

        public T this[int i]
        {
            get { return GetValue(i); }
            set { SetValue(i, value); }
        }

        private void SetValue(int i, T value)
        {
            if (i < 0 || i >= _length)
                throw new ArgumentOutOfRangeException();

            int shift = i * Size;

            Marshal.StructureToPtr(value, (IntPtr.Add(_ptr, shift)), false);
        }

        private T GetValue(int i)
        {
            if (i < 0 || i >= _length )
                throw new ArgumentOutOfRangeException();

            // in fortran 2-d indexing is reversed
            int shift = i * Size;

            return (T)Marshal.PtrToStructure(IntPtr.Add(_ptr, shift), typeof(T));
        }
    }
}
