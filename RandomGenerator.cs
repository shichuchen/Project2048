using System;
using System.Collections.Generic;
using System.Linq;

namespace Project2048
{
    public class RandomGenerator
    {
        private static Random random;
        private static void Renew()
        {
            random = new Random();
        }
        /// <summary>
        /// 获取一个大于等于0, 小于maxValue的随机整数
        /// </summary>
        /// <param name="maxValue"></param>
        /// <returns></returns>
        public static int Next(int maxValue)
        {
            return Next(0, maxValue);
        }
        public static int Next(int minValue, int maxValue)
        {
            Renew();
            return random.Next(minValue, maxValue);
        }
        /// <summary>
        /// 获取一个介于0到1之间的随机数
        /// </summary>
        /// <returns></returns>
        public static double NextDouble()
        {
            Renew();
            return random.NextDouble();
        }
        /// <summary>
        /// 返回每个值介于0到1的double类型数组
        /// </summary>
        /// <param name="count">数组大小</param>
        /// <returns></returns>
        public static double[] GetDoubles(int count)
        {
            Renew();
            double[] doubles = new double[count];
            for (int i = 0; i < count; ++i)
            {
                while (true)
                {
                    double tempDouble = random.NextDouble();
                    if (!doubles.Contains(tempDouble))
                    {
                        doubles[i] = tempDouble;
                        break;
                    }
                }
            }
            return doubles;
        }
        public static int[] GetInts(int count, int maxValue)
        {
            return GetInts(count, 0, maxValue);
        }
        public static int[] GetInts(int count, int minValue, int maxValue)
        {
            Renew();
            // 用List.Add, 防止由于数组初始化导致0无法取到
            List<int> ints = new List<int>(count);
            for (int i = 0; i < count; ++i)
            {
                ints.Add(random.Next(minValue, maxValue));
            }
            return ints.ToArray();
        }
        public static int[] GetDistinctInts(int count, int maxValue)
        {
            return GetDistinctInts(count, 0, maxValue);
        }
        /// <summary>
        /// 返回每个值介于minValue到maxValue的int数组
        /// 包括minValue但不包括maxValue
        /// </summary>
        /// <param name="count">数组大小</param>
        /// <param name="minValue">最小值, 可能产生</param>
        /// <param name="maxValue">最大值, 不可能产生</param>
        /// <returns></returns>
        public static int[] GetDistinctInts(int count, int minValue, int maxValue)
        {
            Renew();
            // 用List.Add, 防止由于数组初始化导致0无法取到
            List<int> ints = new List<int>(count);
            for (int i = 0; i < count; ++i)
            {
                while (true)
                {
                    int tempInt = random.Next(minValue, maxValue);
                    if (!ints.Contains(tempInt))
                    {
                        ints.Add(tempInt);
                        break;
                    }
                }
            }
            return ints.ToArray();
        }
    }
}
