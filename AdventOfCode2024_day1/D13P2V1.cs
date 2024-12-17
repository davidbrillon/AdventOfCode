using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics; // For handling very large integers if needed

public class D13P2V1
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
    // After reading and parsing, we must add 10000000000000 to both the X and Y of every prize.
    // According to the problem, after this huge offset is added, only the second and fourth
    // claw machines from the example can be solved.
    //
    // We'll solve the given systems of linear Diophantine equations exactly. To handle large
    // numbers, we'll use BigInteger. The cost calculation remains straightforward once we find (a,b).
    
    string filePath = D13P1V1.GetInputPath("input_day13.txt");
    string[] lines = File.ReadAllLines(filePath);
    
    // Parse the file into machine configurations.
    // We'll accumulate machines in a list. Each machine has:
    // (xA, yA, xB, yB, xP, yP)
    List<(BigInteger xA, BigInteger yA, BigInteger xB, BigInteger yB, BigInteger xP, BigInteger yP)> machines
        = new List<(BigInteger, BigInteger, BigInteger, BigInteger, BigInteger, BigInteger)>();

    for (int i = 0; i < lines.Length;)
    {
      // Skip blank lines
      if (string.IsNullOrWhiteSpace(lines[i]))
      {
        i++;
        continue;
      }

      // We expect a block of 3 lines describing a machine
      if (i + 2 >= lines.Length) break; // not enough lines left

      string lineA = lines[i].Trim();
      string lineB = lines[i + 1].Trim();
      string lineP = lines[i + 2].Trim();

      var (xA, yA) = ParseButtonLine(lineA);
      var (xB, yB) = ParseButtonLine(lineB);
      var (xP, yP) = ParsePrizeLine(lineP);

      // Add the large offset to prize coordinates
      BigInteger offset = 10000000000000;
      xP += offset;
      yP += offset;

      machines.Add((xA, yA, xB, yB, xP, yP));

      i += 3;
      if (i < lines.Length && string.IsNullOrWhiteSpace(lines[i]))
      {
        i++;
      }
    }

    // Costs
    BigInteger cost_A = 3;
    BigInteger cost_B = 1;

    var results = new List<BigInteger>();

    // Solve each machine using a linear Diophantine approach:
    // We have two equations:
    // xA*a + xB*b = xP
    // yA*a + yB*b = yP
    //
    // This is a system of linear equations. For a solution to exist, the system must be consistent.
    // If it exists, there's a unique (a,b) solution since it's a 2x2 system with a nonzero determinant.
    // Once we find (a,b), we check if they're non-negative. If yes, compute cost = 3a + b and record it.

    foreach (var machine in machines)
    {
      BigInteger xA = machine.xA;
      BigInteger yA = machine.yA;
      BigInteger xB = machine.xB;
      BigInteger yB = machine.yB;
      BigInteger xP = machine.xP;
      BigInteger yP = machine.yP;

      // Solve:
      // |xA xB||a| = |xP|
      // |yA yB||b|   |yP|

      // Determinant:
      BigInteger det = xA * yB - xB * yA;
      if (det == 0)
      {
        // No unique solution
        continue;
      }

      // Using Cramer's rule:
      // a = (xP*yB - xB*yP)/det
      // b = (xA*yP - yA*xP)/det
      BigInteger a_num = (xP * yB - xB * yP);
      BigInteger b_num = (xA * yP - yA * xP);

      // a and b must be integers, det divides a_num and b_num for solution to exist
      if (a_num % det != 0 || b_num % det != 0)
      {
        // No integer solution
        continue;
      }

      BigInteger a = a_num / det;
      BigInteger b = b_num / det;

      // We need non-negative solution since negative presses don't make sense
      if (a < 0 || b < 0)
      {
        continue;
      }

      // Compute cost
      BigInteger current_cost = a * cost_A + b * cost_B;
      results.Add(current_cost);
    }

    results.Sort();
    int max_prizes = results.Count;
    BigInteger min_total_cost = 0;
    foreach (var r in results)
    {
      min_total_cost += r;
    }

    Console.WriteLine("Max prizes that can be won: " + max_prizes);
    Console.WriteLine("Minimum total cost for all solvable prizes: " + min_total_cost);
  }

  private static (BigInteger x, BigInteger y) ParseButtonLine(string line)
  {
    // Format: "Button A: X+94, Y+34" or "Button B: X+22, Y+67"
    var parts = line.Split(',');
    var partX = parts[0].Trim();
    // partX might contain something like "Button A: X+94"
    int xIndex = partX.IndexOf('X');
    string xStr = partX.Substring(xIndex + 1); // could be +94
    BigInteger xVal = BigInteger.Parse(xStr);

    var partY = parts[1].Trim();
    int yIndex = partY.IndexOf('Y');
    string yStr = partY.Substring(yIndex + 1);
    BigInteger yVal = BigInteger.Parse(yStr);

    return (xVal, yVal);
  }

  private static (BigInteger x, BigInteger y) ParsePrizeLine(string line)
  {
    // Format: "Prize: X=8400, Y=5400"
    var parts = line.Split(',');
    var partX = parts[0].Trim();
    int xIndex = partX.IndexOf('X');
    string xStr = partX.Substring(xIndex + 2); // skip "X="
    BigInteger xVal = BigInteger.Parse(xStr);

    var partY = parts[1].Trim();
    int yIndex = partY.IndexOf('Y');
    string yStr = partY.Substring(yIndex + 2); // skip "Y="
    BigInteger yVal = BigInteger.Parse(yStr);

    return (xVal, yVal);
  }
}
