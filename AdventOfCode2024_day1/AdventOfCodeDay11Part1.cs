using System;
using System.Collections.Generic;
using System.Linq;

public class AdventOfCodeDay11Part1
{
  public static int CountStonesAfterBlinks(List<long> initialStones, int blinks)
  {
    Queue<long> queue = new Queue<long>(initialStones);

    for (int i = 0; i < blinks; i++)
    {
      Queue<long> newQueue = new Queue<long>();

      while (queue.Count > 0)
      {
        long number = queue.Dequeue();

        if (number == 0)
        {
          // Rule 1: If the number is 0, replace it with 1
          newQueue.Enqueue(1);
        }
        else if (number.ToString().Length % 2 == 0)
        {
          // Rule 2: If the number has an even number of digits, split it into two stones
          string numStr = number.ToString();
          int mid = numStr.Length / 2;
          long left = long.Parse(numStr.Substring(0, mid));
          long right = long.Parse(numStr.Substring(mid));

          newQueue.Enqueue(left);
          newQueue.Enqueue(right);
        }
        else
        {
          // Rule 3: If none of the above, multiply the number by 2024
          newQueue.Enqueue(number * 2024);
        }
      }

      queue = newQueue;
    }

    return queue.Count;
  }

  public static void Main()
  {
    // Part 1 25 => 218956
    // Part 2 75 => 259593838049805

    // Example input
    //List<int> initialStones = new List<int> { 125, 17 };

    // Puzzle input 3279 998884 1832781 517 8 18864 28 0 
    List<long> initialStones = new List<long> { 3279, 998884, 1832781, 517, 8, 18864, 28, 0 };

    // Part 1 = 25 
    //int blinks = 25;
    int blinks = 75;

    // Calculate the result
    int result = CountStonesAfterBlinks(initialStones, blinks);

    // Output the result
    Console.WriteLine(result);
  }
}
