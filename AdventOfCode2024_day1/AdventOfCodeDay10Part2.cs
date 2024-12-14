using System;
using System.Collections.Generic;

public class AdventOfCodeDay10Part2
{
  static string[] puzzleInput = {
        "89010123",
        "78121874",
        "87430965",
        "96549874",
        "45678903",
        "32019012",
        "01329801",
        "10456732"
    };

  // Directions: up, down, left, right
  private static readonly (int dx, int dy)[] Directions = new (int dx, int dy)[]
  {
        (0, 1), (0, -1), (1, 0), (-1, 0)
  };

  public static void Main()
  {
    var solver = new AdventOfCodeDay10Part2();
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

    int totalRating = 0;

    foreach (var trailhead in trailheads)
    {
      totalRating += CountDistinctTrails(grid, rows, cols, trailhead);
    }

    // Print the result
    Console.WriteLine($"Total Rating: {totalRating}");
  }

  private int CountDistinctTrails(int[,] grid, int rows, int cols, (int r, int c) start)
  {
    // Stack for DFS: (current_position, path_so_far)
    var stack = new Stack<((int r, int c) pos, HashSet<(int r, int c)> path)>();
    stack.Push((start, new HashSet<(int r, int c)> { start }));

    var distinctTrails = new HashSet<string>();

    while (stack.Count > 0)
    {
      var (current, path) = stack.Pop();
      int x = current.r, y = current.c;

      // If we've reached height 9, store the trail
      if (grid[x, y] == 9)
      {
        distinctTrails.Add(string.Join("->", path));
        continue;
      }

      // Explore neighbors
      foreach (var (dx, dy) in Directions)
      {
        int nx = x + dx, ny = y + dy;

        if (nx >= 0 && nx < rows && ny >= 0 && ny < cols && grid[nx, ny] == grid[x, y] + 1)
        {
          var newPath = new HashSet<(int r, int c)>(path) { (nx, ny) };
          stack.Push(((nx, ny), newPath));
        }
      }
    }

    return distinctTrails.Count;
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
