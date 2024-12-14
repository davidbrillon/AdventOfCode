using System;
using System.Collections.Generic;

public class AdventOfCodeDay11Part2
{
  public static long CountStonesAfterBlinks(List<long> initialStones, int blinks)
  {
    // Dictionary to store counts of stones
    Dictionary<long, long> stoneCounts = new Dictionary<long, long>();

    // Initialize the dictionary with initial stones
    foreach (long stone in initialStones)
    {
      if (stoneCounts.ContainsKey(stone))
        stoneCounts[stone]++;
      else
        stoneCounts[stone] = 1;
    }

    for (int i = 0; i < blinks; i++)
    {
      Dictionary<long, long> newStoneCounts = new Dictionary<long, long>();

      foreach (var kvp in stoneCounts)
      {
        long number = kvp.Key;
        long count = kvp.Value;

        if (number == 0) // Rule 1
        {
          if (!newStoneCounts.ContainsKey(1))
            newStoneCounts[1] = 0;
          newStoneCounts[1] += count;
        }
        else if (NumberOfDigits(number) % 2 == 0) // Rule 2
        {
          string numStr = number.ToString();
          int mid = numStr.Length / 2;
          long left = long.Parse(numStr.Substring(0, mid));
          long right = long.Parse(numStr.Substring(mid));

          if (!newStoneCounts.ContainsKey(left))
            newStoneCounts[left] = 0;
          if (!newStoneCounts.ContainsKey(right))
            newStoneCounts[right] = 0;

          newStoneCounts[left] += count;
          newStoneCounts[right] += count;
        }
        else // Rule 3
        {
          long newNumber = number * 2024;
          if (!newStoneCounts.ContainsKey(newNumber))
            newStoneCounts[newNumber] = 0;

          newStoneCounts[newNumber] += count;
        }
      }

      // Replace the current stone counts with the new ones
      stoneCounts = newStoneCounts;
    }

    // Calculate total number of stones
    long totalStones = 0;
    foreach (var count in stoneCounts.Values)
    {
      totalStones += count;
    }

    return totalStones;
  }

  private static int NumberOfDigits(long number)
  {
    int digits = 0;
    while (number > 0)
    {
      digits++;
      number /= 10;
    }
    return digits;
  }

  public static void Main()
  {
    // Example input
    //List<long> initialStones = new List<long> { 125, 17 };

    // Puzzle input 3279 998884 1832781 517 8 18864 28 0 
    List<long> initialStones = new List<long> { 3279, 998884, 1832781, 517, 8, 18864, 28, 0 };

    int blinks = 75;

    // Calculate the result
    long result = CountStonesAfterBlinks(initialStones, blinks);

    // Output the result
    Console.WriteLine(result);
  }
}
