using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmlPro.Extensions;
using XmlPro.Helpers;

class Solution
{
    public int solution(int[] A)
    {
        // write your code in C# 6.0 with .NET 4.5 (Mono)
        HashSet<int> uniques = new HashSet<int>(A);
        for (int i = 1; i < 1000000; i++)
        {
            if (!uniques.Contains(i))
            {
                return i;
            }
        }

        return -1;
    }

    public int[] solution1(int N, int[] A)
    {
        // write your code in C# 6.0 with .NET 4.5 (Mono)
        int count = A.Length;
        Dictionary<int, int> increases = new Dictionary<int, int>();
        int lastMax = 0;
        for (int i = 0; i < count; i++)
        {
            int instruction = A[i];
            if (instruction == N+1)
            {
                if (increases.Count > 0)
                {
                    lastMax += Enumerable.Max(increases.Values);
                    increases.Clear();
                }
            }
            else if (increases.ContainsKey(instruction))
            {
                increases[instruction] += 1;
            }
            else
            {
                increases[instruction] = 1;
            }
        }

        int[] result = new int[N];
        for (int i = 0; i < N; i++)
        {
            result[i] = increases.ContainsKey(i + 1) ? increases[i + 1] + lastMax : lastMax;
        }
        return result;
    }

    static int next(string all, int size, int from = 0)
    {
        HashSet<char> set = new HashSet<char>();
        for (int i = from; i < size; i++)
        {
            if (!set.Add(all[i]))
            {
                return i;
            }
        }

        return size;
    }

    static List<int> split(string all, int start = 0)
    {
        int size = all.Length;
        List<int> splits = new List<int>();
        for (int i = 0; i < size; )
        {
            int next = Solution.next(all, size, i);
            splits.Add(next);
            i = next;
        }

        if (splits.Last() < size)
        {
            splits.Add(size);
        }

        return splits;
    }

    // public static int solution(int N)
    // {
    //     // write your code in C# 6.0 with .NET 4.5 (Mono)
    //     string binary = Convert.ToString(N, 2);
    //
    //     int startIndex = -1;
    //     int maxCount = 0;
    //     for (int i = 0; i < binary.Length; i++)
    //     {
    //         char cur = binary[i];
    //         if (cur == '0')
    //         {
    //             continue;
    //         }
    //         else if (startIndex < 0)
    //         {
    //             startIndex = i;
    //         }
    //         else
    //         {
    //             maxCount = Math.Max(maxCount, i - startIndex - 1);
    //             startIndex = i;
    //         }
    //     }
    //
    //     return maxCount;
    // }
    //
    // public int solution(int[] A)
    // {
    //     int size = A.Length;
    //     int[] indexes = A.Select((v, i) => i).ToArray();
    //     Array.Sort(A, indexes);
    //
    //     int count = 0;
    //     int last = indexes[0];
    //     int cur = -1;
    //     int max = 0;
    //     for (int i = 0; i < size; i++)
    //     {
    //         cur = indexes[i];
    //         if (cur >= max)
    //         {
    //             last = cur;
    //             max = max < cur ? cur : max;
    //             continue;
    //         }
    //         for (int j = i-1; j >= 0; j--)
    //         {
    //             last = indexes[j];
    //             if (cur < last)
    //             {
    //                 Console.WriteLine($"({cur}, {last})");
    //                 count++;
    //                 if (count > 1_000_000_000)
    //                 {
    //                     return -1;
    //                 }
    //             }
    //         }
    //     }
    //
    //     return count;
    // }



    // public int[] solution(int[] A, int K)
    // {
    //     int size =A.Length;
    //     int first = K % size;
    //     if (first == 0)
    //         return A;
    //
    //     int[] rotated = new int[size];
    //
    //     for (int i = 0; i < size; i++)
    //     {
    //         int index = (i + K) % size;
    //         rotated[i] = A[index];
    //     }
    //     
    //     return rotated;
    // }

    // public int solution(int N)
    // {
    //     // write your code in C# 6.0 with .NET 4.5 (Mono)
    //     int sqrt = (int)Math.Floor(Math.Sqrt(N));
    //
    //     HashSet<int> factors = new HashSet<int>();
    //     for (int i = 0; i < sqrt; i++)
    //     {
    //         if (N % (i+1) == 0)
    //         {
    //             factors.Add(i+1);
    //             factors.Add(N / (i + 1));
    //         }
    //     }
    //
    //     Console.WriteLine($"{string.Join(',', factors)}");
    //     return factors.Count;
    // }
}

[TestClass]
public class SolutionTest
{
    private Solution sol = new Solution();

    [TestMethod]
    public void Test4()
    {
        Assert.AreEqual(1, sol.solution(new int[]{-1, -3}));
        Assert.AreEqual(4, sol.solution(new int[]{1, 2, 3}));
    }

    public T[] randomArray<T>(int count, Func<T> random)
    {
        T[] array = Enumerable
            .Repeat(0, count)
            .Select(i => random())
            .ToArray();

        return array;
    }

    [TestMethod]
    public void Test3()
    {
        int[] A = {3, 4, 4, 6, 1, 4, 4};
        var result = new Solution().solution1(5, A);
        Console.WriteLine(string.Join(',', result));
    }

    [TestMethod]
    public void TestPorformance()
    {
        int N = 1000;
        int M = 100_000;
        int min = 1;        // min is inclusive!
        int max = N + 50;    // max is exclusive!
        Random random = new Random();
        Func<int> generator = () => Math.Min(N+1, random.Next(min, max));
        int[] A = randomArray(N, generator);
        int largest = Enumerable.Max(A);

        var result = new Solution().solution1(N, A);
        Console.WriteLine(string.Join(',', result));
        Console.WriteLine();
    }

    // [TestMethod]
    // public void Test1()
    // {
    //     Assert.AreEqual(5, Solution.solution(
    //         1041));
    //     Assert.AreEqual(2, Solution.solution(
    //         9));
    //     Assert.AreEqual(4, Solution.solution(
    //         529));
    //     Assert.AreEqual(1, Solution.solution(
    //         20));
    //
    // }
    //
    // [TestMethod]
    // public void Test2()
    // {
    //     var sol = new Solution();
    //     Assert.AreEqual(4, sol.solution(
    //         new int[]{-1, 6, 3, 4, 7, 4}));
    // }

    // [TestMethod]
    // public void Test2()
    // {
    //     var sol = new Solution();
    //     Assert.AreEqual(8, sol.solution(24));
    // }
}
