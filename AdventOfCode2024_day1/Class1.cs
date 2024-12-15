using System;
using System.Collections.Generic;

public class GardenFenceCost_D12P2V2
{
  // Directions for BFS/DFS
  private static readonly int[] dr = { -1, 1, 0, 0 };
  private static readonly int[] dc = { 0, 0, -1, 1 };

  public static int Solve(string[] rawGrid)
  {
    // Ensure no trailing whitespace
    for (int i = 0; i < rawGrid.Length; i++)
      rawGrid[i] = rawGrid[i].TrimEnd();

    int rows = rawGrid.Length;
    if (rows == 0) return 0;
    int cols = rawGrid[0].Length;

    bool[,] visited = new bool[rows, cols];

    int totalPrice = 0;

    for (int r = 0; r < rows; r++)
    {
      for (int c = 0; c < cols; c++)
      {
        if (!visited[r, c])
        {
          char plantType = rawGrid[r][c];

          // Get all cells in this region
          var regionCells = GetRegionCells(rawGrid, visited, r, c, plantType);
          int area = regionCells.Count;

          // Identify boundary edges
          // We'll have a grid of (rows+1) x cols for horizontal edges and rows x (cols+1) for vertical edges
          bool[,] horizBoundary = new bool[rows + 1, cols];  // horizBoundary[r,c] = boundary edge between (r,c) and (r,c+1) horizontally
          bool[,] vertBoundary = new bool[rows, cols + 1];   // vertBoundary[r,c] = boundary edge between (r,c) and (r+1,c) vertically

          var regionSet = new HashSet<(int, int)>(regionCells);
          foreach (var (cr, cc) in regionCells)
          {
            // Top edge
            if (cr == 0 || rawGrid[cr - 1][cc] != plantType)
              horizBoundary[cr, cc] = true;

            // Bottom edge
            if (cr == rows - 1 || rawGrid[cr + 1][cc] != plantType)
              horizBoundary[cr + 1, cc] = true;

            // Left edge
            if (cc == 0 || rawGrid[cr][cc - 1] != plantType)
              vertBoundary[cr, cc] = true;

            // Right edge
            if (cc == cols - 1 || rawGrid[cr][cc + 1] != plantType)
              vertBoundary[cr, cc + 1] = true;
          }

          // Now count the number of continuous runs in horizontal and vertical edges
          int sideCount = CountContinuousRunsHorizontal(horizBoundary) + CountContinuousRunsVertical(vertBoundary);

          int price = area * sideCount;
          totalPrice += price;

          Console.WriteLine($"A region of {plantType} plants with price {area} * {sideCount} = {price}.");
        }
      }
    }

    return totalPrice;
  }

  private static List<(int r, int c)> GetRegionCells(string[] grid, bool[,] visited, int startR, int startC, char plantType)
  {
    int rows = grid.Length;
    int cols = grid[0].Length;
    var region = new List<(int, int)>();
    var queue = new Queue<(int, int)>();

    visited[startR, startC] = true;
    queue.Enqueue((startR, startC));

    while (queue.Count > 0)
    {
      var (cr, cc) = queue.Dequeue();
      region.Add((cr, cc));
      for (int i = 0; i < 4; i++)
      {
        int nr = cr + dr[i];
        int nc = cc + dc[i];
        if (nr >= 0 && nr < rows && nc >= 0 && nc < cols && !visited[nr, nc] && grid[nr][nc] == plantType)
        {
          visited[nr, nc] = true;
          queue.Enqueue((nr, nc));
        }
      }
    }

    return region;
  }

  // Count how many continuous horizontal runs we have across all rows
  private static int CountContinuousRunsHorizontal(bool[,] horizBoundary)
  {
    int rows = horizBoundary.GetLength(0);
    int cols = horizBoundary.GetLength(1);
    int count = 0;

    for (int r = 0; r < rows; r++)
    {
      // Each row in horizBoundary represents potential horizontal edges between (r,c) and (r,c+1)
      // Actually each true in horizBoundary[r,c] is a single vertical line segment (since it separates two corners horizontally).
      // We want to merge consecutive trues.
      bool inRun = false;
      for (int c = 0; c < cols; c++)
      {
        if (horizBoundary[r, c])
        {
          if (!inRun)
          {
            // Start a new run
            inRun = true;
          }
          // If already in run, continue...
        }
        else
        {
          // No boundary edge at this position
          if (inRun)
          {
            // We reached the end of a run
            count++;
            inRun = false;
          }
        }
      }
      // If the row ended while still in a run
      if (inRun) count++;
    }

    return count;
  }

  // Count how many continuous vertical runs we have across all columns
  private static int CountContinuousRunsVertical(bool[,] vertBoundary)
  {
    int rows = vertBoundary.GetLength(0);
    int cols = vertBoundary.GetLength(1);
    int count = 0;

    for (int c = 0; c < cols; c++)
    {
      bool inRun = false;
      for (int r = 0; r < rows; r++)
      {
        if (vertBoundary[r, c])
        {
          if (!inRun)
          {
            // start a new run
            inRun = true;
          }
          // continue the run
        }
        else
        {
          // no boundary here
          if (inRun)
          {
            // end the run
            count++;
            inRun = false;
          }
        }
      }
      if (inRun) count++;
    }

    return count;
  }

  public static void Main()
  {
    // Test with the provided example
    string[] largeMap = {
            "RRRRIICCFF",
            "RRRRIICCCF",
            "VVRRRCCFFF",
            "VVRCCCJFFF",
            "VVVVCJJCFE",
            "VVIVCCJJEE",
            "VVIIICJJEE",
            "MIIIIIJJEE",
            "MIIISIJEEE",
            "MMMISSJEEE"
        };

    string[] largeMap2 = {
            "OOOOO",
            "OXOXO",
            "OOOOO",
            "OXOXO",
            "OOOOO"
        };

    string[] largeMap3 = {
            "EEEEE",
            "EXXXX",
            "EEEEE",
            "EXXXX",
            "EEEEE"
        };

    string[] largeMap4 = {
            "AAAAAA",
            "AAABBA",
            "AAABBA",
            "ABBAAA",
            "ABBAAA",
            "AAAAAA"
        };

    string filePath = GetInputPath("input_day12.txt");
    string[] largeMap1 = File.ReadAllLines(filePath);

    int result = Solve(largeMap4);
    Console.WriteLine("Total price: " + result);
    // Expected: 1206
  }
  static string GetInputPath(string fileName)
  {
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
    return path;
  }

}
