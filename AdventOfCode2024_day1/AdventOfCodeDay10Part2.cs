using System;
using System.Collections.Generic;

public class AdventOfCodeDay10Part2
{
  static string[] puzzleInput = [
      "89010123",
        "78121874",
        "87430965",
        "96549874",
        "45678903",
        "32019012",
        "01329801",
        "10456732"
  ];

  // Directions: up, down, left, right
  private static readonly (int dx, int dy)[] Directions = new (int dx, int dy)[]
  {
        (0, 1), (0, -1), (1, 0), (-1, 0)
  };

  public static void Main()
  {
    var solver = new AdventOfCodeDay10();
    solver.Run();
  }

  public void Run()
  {
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

    // Memoization dictionary for trail ratings
    Dictionary<(int r, int c), int> memo = new Dictionary<(int r, int c), int>();

    int sumOfRatings = 0;
    foreach (var start in trailheads)
    {
      int trailRating = CountDistinctTrails(start.r, start.c, grid, rows, cols, memo, new HashSet<(int, int)>());
      sumOfRatings += trailRating;
    }

    // Print the result
    Console.WriteLine(sumOfRatings);
  }

  private int CountDistinctTrails(int r, int c, int[,] grid, int rows, int cols,
      Dictionary<(int r, int c), int> memo, HashSet<(int, int)> visited)
  {
    if (memo.ContainsKey((r, c)))
    {
      return memo[(r, c)];
    }

    int currentHeight = grid[r, c];

    // If current cell is height '9', this is a valid trail endpoint
    if (currentHeight == 9)
    {
      return 1;
    }

    int totalTrails = 0;
    visited.Add((r, c));

    // Try all valid moves that go from height h to h+1
    foreach (var (dx, dy) in Directions)
    {
      int nr = r + dx;
      int nc = c + dy;
      if (nr < 0 || nr >= rows || nc < 0 || nc >= cols || visited.Contains((nr, nc)))
        continue;

      if (grid[nr, nc] == currentHeight + 1)
      {
        totalTrails += CountDistinctTrails(nr, nc, grid, rows, cols, memo, new HashSet<(int, int)>(visited));
      }
    }

    visited.Remove((r, c));
    memo[(r, c)] = totalTrails;
    return totalTrails;
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
}
