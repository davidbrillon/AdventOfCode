using System;
using System.Collections.Generic;
using System.IO;

public class D13P1V1
{
  public static void Main()
  {
    // We'll read the input data from "input_day13.txt" file.
    // The data is formatted as groups of three lines (plus a blank line separator):
    //
    // Button A: X+46, Y+72
    // Button B: X+49, Y+18
    // Prize: X=5353, Y=4446
    //
    // Button A: X+82, Y+24
    // Button B: X+19, Y+40
    // Prize: X=3974, Y=4056
    //
    // And so on...

    // First, read all lines from the file
    string filePath = GetInputPath("input_day13.txt");
    string[] lines = File.ReadAllLines(filePath);

    // Parse the file into machine configurations.
    // We'll accumulate machines in a list. Each machine has:
    // (xA, yA, xB, yB, xP, yP)
    List<(int xA, int yA, int xB, int yB, int xP, int yP)> machines = new List<(int, int, int, int, int, int)>();

    // We'll iterate through the lines in chunks of 4:
    // Lines: 0: Button A, 1: Button B, 2: Prize, 3: (blank or next machine)
    // Note: The input might not have a blank line after the last machine. We'll handle that.
    for (int i = 0; i < lines.Length;)
    {
      // If we reach a blank line or an unexpected end, skip it
      if (string.IsNullOrWhiteSpace(lines[i]))
      {
        i++;
        continue;
      }

      // We expect a block of 3 lines describing a machine
      if (i + 2 >= lines.Length) break; // Not enough lines left

      string lineA = lines[i].Trim();
      string lineB = lines[i + 1].Trim();
      string lineP = lines[i + 2].Trim();

      // Parse Button A line: "Button A: X+94, Y+34"
      var (xA, yA) = ParseButtonLine(lineA);

      // Parse Button B line: "Button B: X+22, Y+67"
      var (xB, yB) = ParseButtonLine(lineB);

      // Parse Prize line: "Prize: X=8400, Y=5400"
      var (xP, yP) = ParsePrizeLine(lineP);

      machines.Add((xA, yA, xB, yB, xP, yP));

      i += 3;
      // Possibly skip the blank line if there is one
      if (i < lines.Length && string.IsNullOrWhiteSpace(lines[i]))
      {
        i++;
      }
    }

    // We are told to consider pressing buttons no more than 100 times each.
    int max_presses = 100;

    // Costs
    int cost_A = 3;
    int cost_B = 1;

    var results = new List<int>();

    foreach (var machine in machines)
    {
      int xA = machine.xA;
      int yA = machine.yA;
      int xB = machine.xB;
      int yB = machine.yB;
      int xP = machine.xP;
      int yP = machine.yP;

      int? min_cost = null;

      // Try all combinations of A and B presses from 0 to 100
      for (int a_count = 0; a_count <= max_presses; a_count++)
      {
        for (int b_count = 0; b_count <= max_presses; b_count++)
        {
          // Check if this combination reaches the prize
          if ((a_count * xA + b_count * xB == xP) &&
              (a_count * yA + b_count * yB == yP))
          {
            int current_cost = a_count * cost_A + b_count * cost_B;
            if (min_cost == null || current_cost < min_cost.Value)
            {
              min_cost = current_cost;
            }
          }
        }
      }

      // Store the minimal cost for this machine (if any)
      if (min_cost.HasValue)
      {
        results.Add(min_cost.Value);
      }
    }

    results.Sort();
    int max_prizes = results.Count;
    int min_total_cost = 0;
    foreach (var r in results)
    {
      min_total_cost += r;
    }

    Console.WriteLine("Max prizes that can be won: " + max_prizes);
    Console.WriteLine("Minimum total cost for all solvable prizes: " + min_total_cost);
  }

  private static (int x, int y) ParseButtonLine(string line)
  {
    // Format: "Button A: X+94, Y+34" or "Button B: X+22, Y+67"
    // We need to extract the integers after X and Y.
    // For example, after 'X' we may have '+94', after 'Y' '+34'.

    // Split by comma
    var parts = line.Split(',');
    // parts[0]: "Button A: X+94"
    // parts[1]: " Y+34"

    // Extract from parts[0]
    // Remove leading and trailing spaces
    var partX = parts[0].Trim();
    // partX might look like "Button A: X+94"
    // Let's find the substring after 'X'
    int xIndex = partX.IndexOf('X');
    string xStr = partX.Substring(xIndex + 1); // might give "+94"
    int xVal = int.Parse(xStr);

    // Extract from parts[1]
    var partY = parts[1].Trim();
    // partY might look like "Y+34"
    int yIndex = partY.IndexOf('Y');
    string yStr = partY.Substring(yIndex + 1); // might give "+34"
    int yVal = int.Parse(yStr);

    return (xVal, yVal);
  }

  private static (int x, int y) ParsePrizeLine(string line)
  {
    // Format: "Prize: X=8400, Y=5400"
    // Similar approach, split by comma
    var parts = line.Split(',');
    // parts[0]: "Prize: X=8400"
    // parts[1]: " Y=5400"

    var partX = parts[0].Trim();
    int xIndex = partX.IndexOf('X');
    // substring after 'X='
    string xStr = partX.Substring(xIndex + 2); // skip 'X='
    int xVal = int.Parse(xStr);

    var partY = parts[1].Trim();
    int yIndex = partY.IndexOf('Y');
    // substring after 'Y='
    string yStr = partY.Substring(yIndex + 2); // skip 'Y='
    int yVal = int.Parse(yStr);

    return (xVal, yVal);
  }

  public static string GetInputPath(string fileName)
  {
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
    return path;
  }
}
