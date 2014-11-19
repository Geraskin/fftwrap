using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace FftWrap
{
    public static class NativeArray1DHelper
    {
        public static void ForEach<T>(this NativeArray1D<T> array, Action<T> func) where T : struct 
        {
            for (int i = 0; i < array.Length; i++)
                    func(array[i]);
        }

        public static void ForEach<T>(this NativeArray1D<T> array, Action<int, T> func) where T : struct 
        {
            for (int i = 0; i < array.Length; i++)
                func(i, array[i]);
        }

        public static void SetEach<T>(this NativeArray1D<T> array, T value) where T : struct 
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }
    }
}
