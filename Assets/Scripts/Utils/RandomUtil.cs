
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace MiniGames.Utils
{
    class RandomUtil
    {

        public static bool IsProbablityMet(float rate)
        {
            return UnityEngine.Random.Range(0f, 1f) < rate;
        }

        public static int GetIndexByProbablity(float[] rates)
        {
            float totalRate = 0;
            for (int i = 0; i < rates.Length; i++)
            {
                totalRate += rates[i];
            }

            float randomRate = UnityEngine.Random.Range(0f, totalRate);
            float tempRate = 0;
            for (int i = 0; i < rates.Length; i++)
            {
                if (randomRate >= tempRate && randomRate < tempRate + rates[i])
                {
                    return i;
                }
                tempRate += rates[i];
            }

            return -1;
        }


        /// <summary>
        /// 获取一个[min,max]随机的整数。
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetRandomInt(int min, int max)
        {
            return UnityEngine.Random.Range(min, max + 1);
        }


        /// <summary>
        /// 生成指定长度的随机索引列表。
        /// </summary>
        /// <param name="length">要生成的索引数量。</param>
        /// <param name="rangeMinInclusive">索引值的最小范围（包含）。</param>
        /// <param name="rangeMaxExclusive">索引值的最大范围（不包含）。</param>
        /// <param name="allowRepeat">是否允许重复的索引。默认值为false。</param>
        /// <returns>生成的随机索引列表。</returns>
        public static List<int> GetRandomIndexList(int length,int rangeMinInclusive, int rangeMaxExclusive,bool allowRepeat=false)
        {
            List<int> indexList = new List<int>();

            if (!allowRepeat)
            {
                // 当不允许重复时，确保范围内的可选数字足够
                if (rangeMaxExclusive - rangeMinInclusive < length)
                {
                    throw new ArgumentException("The range is too small for the requested length without repeats.");
                }

                HashSet<int> usedIndices = new HashSet<int>();

                while (indexList.Count < length)
                {
                    int randomIndex = UnityEngine.Random.Range(rangeMinInclusive, rangeMaxExclusive);
                    if (!usedIndices.Contains(randomIndex))
                    {
                        indexList.Add(randomIndex);
                        usedIndices.Add(randomIndex);
                    }
                }
            }
            else
            {
                // 允许重复的情况下，直接生成
                for (int i = 0; i < length; i++)
                {
                    int randomIndex = UnityEngine.Random.Range(rangeMinInclusive, rangeMaxExclusive);
                    indexList.Add(randomIndex);
                }
            }

            return indexList;
        }


        /// <summary>
        /// 随机打乱List中的顺序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(ref List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                // 生成一个随机索引，范围是 0 到 i（包含 i）
                int j = UnityEngine.Random.Range(0, i + 1);
                // 交换位置
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }


        private const string ALPHA_NUMERIC_CHARSET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        /// <summary>
        /// 返回字母和数组组成的随机字符串
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static string RandomAlphaNumericId(int length)
        {
            // 如果长度为 0 或负数，直接返回空字符串
            if (length <= 0) return string.Empty;

            // 存储生成的随机字符串
            StringBuilder result = new StringBuilder();

            // 生成随机字符并拼接到字符串中
            for (int i = 0; i < length; i++)
            {
                // 随机选择字符集中的一个字符并追加到结果
                result.Append(ALPHA_NUMERIC_CHARSET[UnityEngine.Random.Range(0,ALPHA_NUMERIC_CHARSET.Length)]);
            }

            // 返回生成的随机字母数字组合
            return result.ToString();
        }
    }
}
