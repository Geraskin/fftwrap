using System;
using System.Collections.Generic;
using System.Numerics;
using FftWrap.Codegen;
using FftWrap.Numerics;
using Porvem.Parallel;

namespace FftWrap.Examples
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            using (var mpi = new Mpi())
            {
                Console.WriteLine(mpi.IsMaster);
                Console.WriteLine(mpi.IsParallel);
                
                
                FftwfMpi.Init();



                FftwfMpi.Cleanup();
            }
            
            Perform1DTransformDirect();
            Perform1DTransform();
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

            var size = NativeArray<SingleComplex>.ElementSize;

            IntPtr srcPtr = Fftwf.Malloc((IntPtr)(length * size));
            IntPtr dstPtr = Fftwf.Malloc((IntPtr)(length * size));

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
