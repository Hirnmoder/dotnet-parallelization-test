using System;

namespace PerformanceTests
{
    public static class ArrayComparer
    {
        public static void Compare<T>(T[] a, T[] b) where T : IComparable<T>
        {
            if (a.Length != b.Length)
                throw new Exception();
            for (int i = 0; i < a.Length; i++)
                if (a[i].CompareTo(b[i]) != 0)
                    throw new Exception();
        }

    }
}
