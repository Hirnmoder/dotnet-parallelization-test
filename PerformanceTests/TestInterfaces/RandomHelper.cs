using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PerformanceTests
{
    public static class RandomHelper
    {
        public static byte NextByte(Random r) => NextNonZero<byte>(r, byte.MinValue, byte.MaxValue);
        public static short NextShort(Random r) => NextNonZero<short>(r, short.MinValue, short.MaxValue);
        public static int NextInt(Random r) => NextNonZero<int>(r, int.MinValue, int.MaxValue);
        public static long NextLong(Random r) => (long)NextNonZero<int>(r, int.MinValue, int.MaxValue) * (long)NextNonZero<int>(r, int.MinValue, int.MaxValue);
        public static double NextDouble(Random r) => (double)NextNonZero<double>(r, int.MinValue, int.MaxValue) / (double)int.MaxValue;
        public static float NextFloat(Random r) => (float)NextDouble(r);



        private static T NextNonZero<T>(Random r, int minValue, int maxValue) where T : struct
        {
            int value = 0;
            while ((value = r.Next(minValue, maxValue)) == 0) ;
            return (T)Convert.ChangeType(value, typeof(T));
        }

    }
}
