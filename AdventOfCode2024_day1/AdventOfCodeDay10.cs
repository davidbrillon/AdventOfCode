using System;
using System.Collections.Generic;

public class AdventOfCodeDay10
{
  static string[] puzzleInput = [
    "89010123",
    "78121874",
    "87430965",
    "96549874",
    "45678903",
    "32019012",
    "01329801",
    "10456732"];

  // Directions: up, down, left, right
  private static readonly (int dx, int dy)[] Directions = new (int dx, int dy)[]
  {
        (0,1), (0,-1), (1,0), (-1,0)
  };

  public static void Main()
  {
    var solver = new AdventOfCodeDay10();
    solver.Run();
  }

  public void Run()
  {
    string filePath = GetInputPath("input_day10.txt");
    string[] puzzleInput = File.ReadAllLines(filePath);


    // Parse input into a 2D grid
    var grid = ParseInput(puzzleInput, out int rows, out int cols);

    // Identify all trailheads (height == 0)
    List<(int r, int c)> trailheads = new List<(int r, int c)>();
    for (int r = 0; r < rows; r++)
    {
      for (int c = 0; c < cols; c++)
      {
        if (grid[r, c] == 0)
        {
          trailheads.Add((r, c));
        }
      }
    }

    // Memoization dictionary: for each cell, store set of reachable "9" coordinates as strings
    Dictionary<(int r, int c), HashSet<string>> memo = new Dictionary<(int r, int c), HashSet<string>>();

    int sumOfScores = 0;
    foreach (var start in trailheads)
    {
      var reachableNines = DFS(start.r, start.c, grid, rows, cols, memo);
      sumOfScores += reachableNines.Count;
    }

    // Print the result
    Console.WriteLine(sumOfScores);
  }

  private HashSet<string> DFS(int r, int c, int[,] grid, int rows, int cols, Dictionary<(int r, int c), HashSet<string>> memo)
  {
    if (memo.ContainsKey((r, c)))
    {
      return memo[(r, c)];
    }

    int currentHeight = grid[r, c];
    var result = new HashSet<string>();

    // If current cell is height '9', record this cell
    if (currentHeight == 9)
    {
      result.Add($"{r},{c}");
      memo[(r, c)] = result;
      return result;
    }

    // Try all valid moves that go from height h to h+1
    foreach (var (dx, dy) in Directions)
    {
      int nr = r + dx;
      int nc = c + dy;
      if (nr < 0 || nr >= rows || nc < 0 || nc >= cols)
        continue;

      if (grid[nr, nc] == currentHeight + 1)
      {
        foreach (var ninePos in DFS(nr, nc, grid, rows, cols, memo))
          result.Add(ninePos);
      }
    }

    memo[(r, c)] = result;
    return result;
  }

  private int[,] ParseInput(string[] inputLines, out int rows, out int cols)
  {
    rows = inputLines.Length;
    cols = inputLines[0].Length;
    int[,] grid = new int[rows, cols];

    for (int r = 0; r < rows; r++)
    {
      var line = inputLines[r].Trim();
      for (int c = 0; c < cols; c++)
      {
        grid[r, c] = line[c] - '0';
      }
    }
    return grid;
  }

  static string GetInputPath(string fileName)
  {
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
    return path;
  }
}
