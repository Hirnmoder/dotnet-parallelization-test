using System;
using System.Numerics;

namespace PerformanceTests.Calculations
{
    public sealed class VectorizationTest : ITest
    {
        private const int MIN_EXP = 25;
        private const int MAX_EXP = 25;
        private const int EXP_STEP = 5;

        public string Name => "Vectorization Test";

        public void Run(AutoStopwatch parent)
        {
            this.Run<byte>(parent, RandomHelper.NextByte);
            this.Run<short>(parent, RandomHelper.NextShort);
            this.Run<int>(parent, RandomHelper.NextInt);
            this.Run<long>(parent, RandomHelper.NextLong);
            this.Run<double>(parent, RandomHelper.NextDouble);
            this.Run<float>(parent, RandomHelper.NextFloat);

            this.RunByte(parent);
            this.RunInt(parent);
            this.RunLong(parent);
            this.RunDouble(parent);
            this.RunFloat(parent);
        }

        private void Run<T>(AutoStopwatch parent, Func<Random, T> getRandomValue) where T : struct, IComparable<T>
        {
            for (int exp = MIN_EXP; exp <= MAX_EXP; exp += EXP_STEP)
            {
                using (var child = parent.CreateChild(prefix: $"{typeof(T).Name} ", postfix: $" (2^{exp})"))
                {
                    T[] randomData1, randomData2;
                    using (var awCompare = child.CreateChild(stepName: "Create Data"))
                    {
                        randomData1 = new T[1 << exp];
                        randomData2 = new T[1 << exp];
                        var random = new Random();
                        for (int i = 0; i < randomData1.Length; i++)
                        {
                            randomData1[i] = getRandomValue(random);
                            randomData2[i] = getRandomValue(random);
                        }
                    }

                    var v = this.RunGenericVectorized(child, randomData1, randomData2);
                    var c = this.RunGenericConventional(child, randomData1, randomData2);

                    using (var awCompare = child.CreateChild(stepName: "Compare Results"))
                    {
                        ArrayComparer.Compare(v.add, c.add);
                        ArrayComparer.Compare(v.sub, c.sub);
                        ArrayComparer.Compare(v.mul, c.mul);
                        ArrayComparer.Compare(v.div, c.div);
                    }
                }
            }
        }

        private (T[] add, T[] sub, T[] mul, T[] div) RunGenericVectorized<T>(AutoStopwatch aw, T[] input1, T[] input2) where T : struct
        {
            var (add, sub, mul, div) =
                (new T[input1.Length],
                 new T[input1.Length],
                 new T[input1.Length],
                 new T[input1.Length]);
            using (var child = aw.CreateChild(postfix: Vector.IsHardwareAccelerated ? "(On)" : "(Off)"))
            {
                for (int i = 0; i < input1.Length; i += Vector<T>.Count)
                {
                    var v1 = new Vector<T>(input1, i);
                    var v2 = new Vector<T>(input2, i);
                    (v1 + v2).CopyTo(add, i);
                    (v1 - v2).CopyTo(sub, i);
                    (v1 * v2).CopyTo(mul, i);
                    (v1 / v2).CopyTo(div, i);
                }
            }
            return (add, sub, mul, div);
        }

        private (T[] add, T[] sub, T[] mul, T[] div) RunGenericConventional<T>(AutoStopwatch aw, T[] input1, T[] input2) where T : struct
        {
            var (add, sub, mul, div) =
                (new T[input1.Length],
                 new T[input1.Length],
                 new T[input1.Length],
                 new T[input1.Length]);
            using (var child = aw.CreateChild())
            {
                for (int i = 0; i < input1.Length; i++)
                {
                    add[i] = (T)((dynamic)input1[i] + input2[i]);
                    sub[i] = (T)((dynamic)input1[i] - input2[i]);
                    mul[i] = (T)((dynamic)input1[i] * input2[i]);
                    div[i] = (T)((dynamic)input1[i] / input2[i]);
                }
            }
            return (add, sub, mul, div);
        }


        #region Byte
        private void RunByte(AutoStopwatch parent)
        {
            for (int exp = MIN_EXP; exp <= MAX_EXP; exp += EXP_STEP)
            {
                using (var child = parent.CreateChild(postfix: $" (2^{exp})"))
                {
                    byte[] randomData1, randomData2;
                    using (var awCompare = child.CreateChild(stepName: "Create Data"))
                    {
                        randomData1 = new byte[1 << exp];
                        randomData2 = new byte[1 << exp];
                        var random = new Random();
                        for (int i = 0; i < randomData1.Length; i++)
                        {
                            randomData1[i] = RandomHelper.NextByte(random);
                            randomData2[i] = RandomHelper.NextByte(random);
                        }
                    }

                    var v = this.RunByteVectorized(child, randomData1, randomData2);
                    var c = this.RunByteConventional(child, randomData1, randomData2);

                    using (var awCompare = child.CreateChild(stepName: "Compare Results"))
                    {
                        ArrayComparer.Compare(v.add, c.add);
                        ArrayComparer.Compare(v.sub, c.sub);
                        ArrayComparer.Compare(v.mul, c.mul);
                        ArrayComparer.Compare(v.div, c.div);
                    }
                }
            }
        }

        private (byte[] add, byte[] sub, byte[] mul, byte[] div) RunByteVectorized(AutoStopwatch aw, byte[] input1, byte[] input2)
        {
            var (add, sub, mul, div) =
                   (new byte[input1.Length],
                    new byte[input1.Length],
                    new byte[input1.Length],
                    new byte[input1.Length]);
            using (var child = aw.CreateChild(postfix: Vector.IsHardwareAccelerated ? "(On)" : "(Off)"))
            {
                for (int i = 0; i < input1.Length; i += Vector<byte>.Count)
                {
                    var v1 = new Vector<byte>(input1, i);
                    var v2 = new Vector<byte>(input2, i);
                    (v1 + v2).CopyTo(add, i);
                    (v1 - v2).CopyTo(sub, i);
                    (v1 * v2).CopyTo(mul, i);
                    (v1 / v2).CopyTo(div, i);
                }
            }
            return (add, sub, mul, div);
        }

        private (byte[] add, byte[] sub, byte[] mul, byte[] div) RunByteConventional(AutoStopwatch aw, byte[] input1, byte[] input2)
        {
            var (add, sub, mul, div) =
                (new byte[input1.Length],
                 new byte[input1.Length],
                 new byte[input1.Length],
                 new byte[input1.Length]);
            using (var child = aw.CreateChild())
            {
                for (int i = 0; i < input1.Length; i++)
                {
                    add[i] = (byte)(input1[i] + input2[i]);
                    sub[i] = (byte)(input1[i] - input2[i]);
                    mul[i] = (byte)(input1[i] * input2[i]);
                    div[i] = (byte)(input1[i] / input2[i]);
                }
            }
            return (add, sub, mul, div);
        }

        #endregion

        #region Int
        private void RunInt(AutoStopwatch parent)
        {
            for (int exp = MIN_EXP; exp <= MAX_EXP; exp += EXP_STEP)
            {
                using (var child = parent.CreateChild(postfix: $" (2^{exp})"))
                {
                    int[] randomData1, randomData2;
                    using (var awCompare = child.CreateChild(stepName: "Create Data"))
                    {
                        randomData1 = new int[1 << exp];
                        randomData2 = new int[1 << exp];
                        var random = new Random();
                        for (int i = 0; i < randomData1.Length; i++)
                        {
                            randomData1[i] = RandomHelper.NextInt(random);
                            randomData2[i] = RandomHelper.NextInt(random);
                        }
                    }

                    var v = this.RunIntVectorized(child, randomData1, randomData2);
                    var c = this.RunIntConventional(child, randomData1, randomData2);

                    using (var awCompare = child.CreateChild(stepName: "Compare Results"))
                    {
                        ArrayComparer.Compare(v.add, c.add);
                        ArrayComparer.Compare(v.sub, c.sub);
                        ArrayComparer.Compare(v.mul, c.mul);
                        ArrayComparer.Compare(v.div, c.div);
                    }
                }
            }
        }

        private (int[] add, int[] sub, int[] mul, int[] div) RunIntVectorized(AutoStopwatch aw, int[] input1, int[] input2)
        {
            var (add, sub, mul, div) =
                   (new int[input1.Length],
                    new int[input1.Length],
                    new int[input1.Length],
                    new int[input1.Length]);
            using (var child = aw.CreateChild(postfix: Vector.IsHardwareAccelerated ? "(On)" : "(Off)"))
            {
                for (int i = 0; i < input1.Length; i += Vector<int>.Count)
                {
                    var v1 = new Vector<int>(input1, i);
                    var v2 = new Vector<int>(input2, i);
                    (v1 + v2).CopyTo(add, i);
                    (v1 - v2).CopyTo(sub, i);
                    (v1 * v2).CopyTo(mul, i);
                    (v1 / v2).CopyTo(div, i);
                }
            }
            return (add, sub, mul, div);
        }

        private (int[] add, int[] sub, int[] mul, int[] div) RunIntConventional(AutoStopwatch aw, int[] input1, int[] input2)
        {
            var (add, sub, mul, div) =
                (new int[input1.Length],
                 new int[input1.Length],
                 new int[input1.Length],
                 new int[input1.Length]);
            using (var child = aw.CreateChild())
            {
                for (int i = 0; i < input1.Length; i++)
                {
                    add[i] = (int)(input1[i] + input2[i]);
                    sub[i] = (int)(input1[i] - input2[i]);
                    mul[i] = (int)(input1[i] * input2[i]);
                    div[i] = (int)(input1[i] / input2[i]);
                }
            }
            return (add, sub, mul, div);
        }

        #endregion

        #region Long
        private void RunLong(AutoStopwatch parent)
        {
            for (int exp = MIN_EXP; exp <= MAX_EXP; exp += EXP_STEP)
            {
                using (var child = parent.CreateChild(postfix: $" (2^{exp})"))
                {
                    long[] randomData1, randomData2;
                    using (var awCompare = child.CreateChild(stepName: "Create Data"))
                    {
                        randomData1 = new long[1 << exp];
                        randomData2 = new long[1 << exp];
                        var random = new Random();
                        for (int i = 0; i < randomData1.Length; i++)
                        {
                            randomData1[i] = RandomHelper.NextLong(random);
                            randomData2[i] = RandomHelper.NextLong(random);
                        }
                    }

                    var v = this.RunLongVectorized(child, randomData1, randomData2);
                    var c = this.RunLongConventional(child, randomData1, randomData2);

                    using (var awCompare = child.CreateChild(stepName: "Compare Results"))
                    {
                        ArrayComparer.Compare(v.add, c.add);
                        ArrayComparer.Compare(v.sub, c.sub);
                        ArrayComparer.Compare(v.mul, c.mul);
                        ArrayComparer.Compare(v.div, c.div);
                    }
                }
            }
        }

        private (long[] add, long[] sub, long[] mul, long[] div) RunLongVectorized(AutoStopwatch aw, long[] input1, long[] input2)
        {
            var (add, sub, mul, div) =
                   (new long[input1.Length],
                    new long[input1.Length],
                    new long[input1.Length],
                    new long[input1.Length]);
            using (var child = aw.CreateChild(postfix: Vector.IsHardwareAccelerated ? "(On)" : "(Off)"))
            {
                for (int i = 0; i < input1.Length; i += Vector<long>.Count)
                {
                    var v1 = new Vector<long>(input1, i);
                    var v2 = new Vector<long>(input2, i);
                    (v1 + v2).CopyTo(add, i);
                    (v1 - v2).CopyTo(sub, i);
                    (v1 * v2).CopyTo(mul, i);
                    (v1 / v2).CopyTo(div, i);
                }
            }
            return (add, sub, mul, div);
        }

        private (long[] add, long[] sub, long[] mul, long[] div) RunLongConventional(AutoStopwatch aw, long[] input1, long[] input2)
        {
            var (add, sub, mul, div) =
                (new long[input1.Length],
                 new long[input1.Length],
                 new long[input1.Length],
                 new long[input1.Length]);
            using (var child = aw.CreateChild())
            {
                for (int i = 0; i < input1.Length; i++)
                {
                    add[i] = (long)(input1[i] + input2[i]);
                    sub[i] = (long)(input1[i] - input2[i]);
                    mul[i] = (long)(input1[i] * input2[i]);
                    div[i] = (long)(input1[i] / input2[i]);
                }
            }
            return (add, sub, mul, div);
        }

        #endregion

        #region Double
        private void RunDouble(AutoStopwatch parent)
        {
            for (int exp = MIN_EXP; exp <= MAX_EXP; exp += EXP_STEP)
            {
                using (var child = parent.CreateChild(postfix: $" (2^{exp})"))
                {
                    double[] randomData1, randomData2;
                    using (var awCompare = child.CreateChild(stepName: "Create Data"))
                    {
                        randomData1 = new double[1 << exp];
                        randomData2 = new double[1 << exp];
                        var random = new Random();
                        for (int i = 0; i < randomData1.Length; i++)
                        {
                            randomData1[i] = RandomHelper.NextDouble(random);
                            randomData2[i] = RandomHelper.NextDouble(random);
                        }
                    }

                    var v = this.RunDoubleVectorized(child, randomData1, randomData2);
                    var c = this.RunDoubleConventional(child, randomData1, randomData2);

                    using (var awCompare = child.CreateChild(stepName: "Compare Results"))
                    {
                        ArrayComparer.Compare(v.add, c.add);
                        ArrayComparer.Compare(v.sub, c.sub);
                        ArrayComparer.Compare(v.mul, c.mul);
                        ArrayComparer.Compare(v.div, c.div);
                    }
                }
            }
        }

        private (double[] add, double[] sub, double[] mul, double[] div) RunDoubleVectorized(AutoStopwatch aw, double[] input1, double[] input2)
        {
            var (add, sub, mul, div) =
                   (new double[input1.Length],
                    new double[input1.Length],
                    new double[input1.Length],
                    new double[input1.Length]);
            using (var child = aw.CreateChild(postfix: Vector.IsHardwareAccelerated ? "(On)" : "(Off)"))
            {
                for (int i = 0; i < input1.Length; i += Vector<double>.Count)
                {
                    var v1 = new Vector<double>(input1, i);
                    var v2 = new Vector<double>(input2, i);
                    (v1 + v2).CopyTo(add, i);
                    (v1 - v2).CopyTo(sub, i);
                    (v1 * v2).CopyTo(mul, i);
                    (v1 / v2).CopyTo(div, i);
                }
            }
            return (add, sub, mul, div);
        }

        private (double[] add, double[] sub, double[] mul, double[] div) RunDoubleConventional(AutoStopwatch aw, double[] input1, double[] input2)
        {
            var (add, sub, mul, div) =
                (new double[input1.Length],
                 new double[input1.Length],
                 new double[input1.Length],
                 new double[input1.Length]);
            using (var child = aw.CreateChild())
            {
                for (int i = 0; i < input1.Length; i++)
                {
                    add[i] = (double)(input1[i] + input2[i]);
                    sub[i] = (double)(input1[i] - input2[i]);
                    mul[i] = (double)(input1[i] * input2[i]);
                    div[i] = (double)(input1[i] / input2[i]);
                }
            }
            return (add, sub, mul, div);
        }

        #endregion

        #region Float
        private void RunFloat(AutoStopwatch parent)
        {
            for (int exp = MIN_EXP; exp <= MAX_EXP; exp += EXP_STEP)
            {
                using (var child = parent.CreateChild(postfix: $" (2^{exp})"))
                {
                    float[] randomData1, randomData2;
                    using (var awCompare = child.CreateChild(stepName: "Create Data"))
                    {
                        randomData1 = new float[1 << exp];
                        randomData2 = new float[1 << exp];
                        var random = new Random();
                        for (int i = 0; i < randomData1.Length; i++)
                        {
                            randomData1[i] = RandomHelper.NextFloat(random);
                            randomData2[i] = RandomHelper.NextFloat(random);
                        }
                    }

                    var v = this.RunFloatVectorized(child, randomData1, randomData2);
                    var c = this.RunFloatConventional(child, randomData1, randomData2);

                    using (var awCompare = child.CreateChild(stepName: "Compare Results"))
                    {
                        ArrayComparer.Compare(v.add, c.add);
                        ArrayComparer.Compare(v.sub, c.sub);
                        ArrayComparer.Compare(v.mul, c.mul);
                        ArrayComparer.Compare(v.div, c.div);
                    }
                }
            }
        }

        private (float[] add, float[] sub, float[] mul, float[] div) RunFloatVectorized(AutoStopwatch aw, float[] input1, float[] input2)
        {
            var (add, sub, mul, div) =
                   (new float[input1.Length],
                    new float[input1.Length],
                    new float[input1.Length],
                    new float[input1.Length]);
            using (var child = aw.CreateChild(postfix: Vector.IsHardwareAccelerated ? "(On)" : "(Off)"))
            {
                for (int i = 0; i < input1.Length; i += Vector<float>.Count)
                {
                    var v1 = new Vector<float>(input1, i);
                    var v2 = new Vector<float>(input2, i);
                    (v1 + v2).CopyTo(add, i);
                    (v1 - v2).CopyTo(sub, i);
                    (v1 * v2).CopyTo(mul, i);
                    (v1 / v2).CopyTo(div, i);
                }
            }
            return (add, sub, mul, div);
        }

        private (float[] add, float[] sub, float[] mul, float[] div) RunFloatConventional(AutoStopwatch aw, float[] input1, float[] input2)
        {
            var (add, sub, mul, div) =
                (new float[input1.Length],
                 new float[input1.Length],
                 new float[input1.Length],
                 new float[input1.Length]);
            using (var child = aw.CreateChild())
            {
                for (int i = 0; i < input1.Length; i++)
                {
                    add[i] = (float)(input1[i] + input2[i]);
                    sub[i] = (float)(input1[i] - input2[i]);
                    mul[i] = (float)(input1[i] * input2[i]);
                    div[i] = (float)(input1[i] / input2[i]);
                }
            }
            return (add, sub, mul, div);
        }

        #endregion
    }
}
