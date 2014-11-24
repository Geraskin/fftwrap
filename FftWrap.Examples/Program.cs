using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using FftWrap.Codegen;
using FftWrap.Numerics;
using Porvem.Parallel;

namespace FftWrap.Examples
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            Perform2DMpi();

            //Perform1DTransformDirect();
            //Perform1DTransform();
        }

        private static void Perform2DMpi()
        {
            using (var mpi = new Mpi())
            {
                Console.WriteLine("rank {0} of {1}", mpi.Rank, mpi.Size);


                var size1 = (IntPtr)8;
                var size2 = (IntPtr)3;

                IntPtr ptrLocalN0;
                IntPtr ptrLocalN0Start;

                FftwfMpi.Init();


                IntPtr localSize = FftwfMpi.LocalSize2D(size1, size2, Mpi.CommWorld, out ptrLocalN0, out ptrLocalN0Start);


                IntPtr srcPtr = Fftwf.AllocComplex(localSize);

                var matrix = new NativeMatrix<SingleComplex>(srcPtr, (int)ptrLocalN0, (int)size2);

                ClearArray(mpi, (int)ptrLocalN0Start, (int)ptrLocalN0, (int)size2, matrix);

                if (mpi.Rank == 0)
                    matrix[0, 0] = SingleComplex.One;

                PrintArray(mpi, (int)ptrLocalN0Start, (int)ptrLocalN0, (int)size2, matrix);


                var plan1 = FftwfMpi.PlanDft2D(size1, size2, srcPtr, srcPtr, Mpi.CommWorld, (int)Direction.Forward, (uint)Flags.Estimate);
                var plan2 = FftwfMpi.PlanDft2D(size1, size2, srcPtr, srcPtr, Mpi.CommWorld, (int)Direction.Backward, (uint)Flags.Estimate);



                Fftwf.Execute(plan1);
                Fftwf.Execute(plan2);


                Console.WriteLine();

                PrintArray(mpi, (int)ptrLocalN0Start, (int)ptrLocalN0, (int)size2, matrix);

                Fftwf.DestroyPlan(plan1);
                Fftwf.DestroyPlan(plan2);


                FftwfMpi.Cleanup();
            }
        }


        private static void PrintArray(Mpi mpi, int n0Start, int n0Size, int size2, NativeMatrix<SingleComplex> matrix)
        {
            for (int i = 0; i < n0Size; i++)
                for (int j = 0; j < size2; j++)
                    Console.WriteLine("r:{0} m[{1},{2}]={3}", mpi.Rank, i + n0Start, j, matrix[i, j]);
        }


        private static void ClearArray(Mpi mpi, int n0Start, int n0Size, int size2, NativeMatrix<SingleComplex> matrix)
        {
            for (int i = 0; i < n0Size; i++)
                for (int j = 0; j < size2; j++)
                    matrix[i, j] = SingleComplex.Zero;
        }


        /// <summary>
        /// Example using high-level wrapping
        /// </summary>
        public static void Perform1DTransform()
        {
            var arr1 = Memory.AllocateArray<SingleComplex>(4);
            var arr2 = Memory.AllocateArray<SingleComplex>(4);

            try
            {
                arr1.SetEach(SingleComplex.Zero);
                arr2.SetEach(SingleComplex.Zero);

                arr1[0] = SingleComplex.One;
                arr2[0] = SingleComplex.ImaginaryOne;

                using (var plan1 = Plan.Create(arr1, Direction.Backward))
                using (var plan2 = Plan.Create(arr1, Direction.Backward))
                {
                    plan1.Execute();
                    plan2.Execute();

                    plan1.Execute(arr2);
                    plan2.Execute(arr2);
                }

                arr1.ForEach(c => Console.WriteLine(c));
                Console.WriteLine();
                arr2.ForEach(c => Console.WriteLine(c));
            }

            finally
            {
                Memory.FreeAllPointers();
            }
        }

        /// <summary>
        /// Using fftw directly without high-level wrapper
        /// </summary>
        public static void Perform1DTransformDirect()
        {
            int length = 100;


            IntPtr srcPtr = Fftwf.AllocComplex((IntPtr)length);
            IntPtr dstPtr = Fftwf.AllocComplex((IntPtr)length);

            try
            {
                var src = new NativeArray<SingleComplex>(srcPtr, length);
                var dst = new NativeArray<SingleComplex>(dstPtr, length);

                src.SetEach(SingleComplex.Zero);
                src[0] = SingleComplex.One;

                IntPtr plan1 = Fftwf.PlanDft1D(length, srcPtr, dstPtr, (int)Direction.Forward, (uint)Flags.Estimate);
                IntPtr plan2 = Fftwf.PlanDft1D(length, dstPtr, srcPtr, (int)Direction.Backward, (uint)Flags.Estimate);

                Fftwf.Execute(plan1);
                Fftwf.Execute(plan2);

                Fftwf.DestroyPlan(plan1);
                Fftwf.DestroyPlan(plan2);
            }

            finally
            {
                Fftwf.Free(srcPtr);
                Fftwf.Free(dstPtr);
            }
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
