using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GardenFenceCost_D12P2
{
  // Directions for BFS (Up, Down, Left, Right)
  private static readonly int[] dr = { -1, 1, 0, 0 };
  private static readonly int[] dc = { 0, 0, -1, 1 };

  public static int Solve(string[] rawGrid)
  {
    // Remove trailing whitespace from each line
    for (int i = 0; i < rawGrid.Length; i++)
    {
      rawGrid[i] = rawGrid[i].TrimEnd();
    }

    int rows = rawGrid.Length;
    if (rows == 0) return 0; // Empty grid, no price
    int cols = rawGrid[0].Length;

    bool[,] visited = new bool[rows, cols];
    int totalPrice = 0;

    // Iterate over each cell in the grid
    for (int r = 0; r < rows; r++)
    {
      for (int c = 0; c < cols; c++)
      {
        if (!visited[r, c])
        {
          // Find all cells of the connected region starting from (r, c)
          var regionCells = GetRegionCells(rawGrid, visited, r, c);
          char plantType = rawGrid[r][c];

          // Calculate the area (simply the number of cells in the region)
          int area = regionCells.Count;

          // Compute the number of sides by identifying and merging boundary edges
          int sides = ComputeSides(rawGrid, regionCells);

          // Calculate the price for this region
          int price = area * sides;
          totalPrice += price;

          // Print the log for this region
          Console.WriteLine($"A region of {plantType} plants with price {area} * {sides} = {price}.");
        }
      }
    }

    return totalPrice;
  }

  /// <summary>
  /// Performs a BFS from the starting cell (startR, startC) to find all cells
  /// connected to it that have the same plant type. Marks visited cells.
  /// </summary>
  private static List<(int r, int c)> GetRegionCells(string[] grid, bool[,] visited, int startR, int startC)
  {
    int rows = grid.Length;
    int cols = grid[0].Length;
    char plantType = grid[startR][startC];

    var regionCells = new List<(int r, int c)>();
    var queue = new Queue<(int r, int c)>();

    visited[startR, startC] = true;
    queue.Enqueue((startR, startC));

    while (queue.Count > 0)
    {
      var (cr, cc) = queue.Dequeue();
      regionCells.Add((cr, cc));

      // Check the 4 neighbors
      for (int i = 0; i < 4; i++)
      {
        int nr = cr + dr[i];
        int nc = cc + dc[i];
        if (IsInGrid(rows, cols, nr, nc) && !visited[nr, nc] && grid[nr][nc] == plantType)
        {
          visited[nr, nc] = true;
          queue.Enqueue((nr, nc));
        }
      }
    }

    return regionCells;
  }

  /// <summary>
  /// Computes the number of sides for the given region. 
  /// A side is a continuous straight fence segment along the region boundary.
  /// </summary>
  private static int ComputeSides(string[] grid, List<(int r, int c)> regionCells)
  {
    int rows = grid.Length;
    int cols = grid[0].Length;

    // Dictionaries to hold horizontal and vertical edges of the boundary
    var horizontalEdges = new Dictionary<int, List<(int startC, int endC)>>();
    var verticalEdges = new Dictionary<int, List<(int startR, int endR)>>();

    // Local helper functions to add edges
    void AddHorizontalEdge(int row, int c1, int c2)
    {
      if (!horizontalEdges.ContainsKey(row))
        horizontalEdges[row] = new List<(int, int)>();
      horizontalEdges[row].Add((Math.Min(c1, c2), Math.Max(c1, c2)));
    }

    void AddVerticalEdge(int col, int r1, int r2)
    {
      if (!verticalEdges.ContainsKey(col))
        verticalEdges[col] = new List<(int, int)>();
      verticalEdges[col].Add((Math.Min(r1, r2), Math.Max(r1, r2)));
    }

    // Identify boundary edges around the region
    char plantType = grid[regionCells[0].r][regionCells[0].c];
    foreach (var (cr, cc) in regionCells)
    {
      // Top edge
      if (cr == 0 || grid[cr - 1][cc] != plantType)
        AddHorizontalEdge(cr, cc, cc + 1);

      // Bottom edge
      if (cr == rows - 1 || grid[cr + 1][cc] != plantType)
        AddHorizontalEdge(cr + 1, cc, cc + 1);

      // Left edge
      if (cc == 0 || grid[cr][cc - 1] != plantType)
        AddVerticalEdge(cc, cr, cr + 1);

      // Right edge
      if (cc == cols - 1 || grid[cr][cc + 1] != plantType)
        AddVerticalEdge(cc + 1, cr, cr + 1);
    }

    // Merge edges along each row and column to count the number of continuous sides
    int sideCount = 0;

    sideCount += MergeEdges(horizontalEdges, isHorizontal: true);
    sideCount += MergeEdges(verticalEdges, isHorizontal: false);

    return sideCount;
  }

  /// <summary>
  /// Merges continuous edges to form sides. For horizontal edges, we merge along rows.
  /// For vertical edges, we merge along columns.
  /// </summary>
  private static int MergeEdges(Dictionary<int, List<(int start, int end)>> edgesByLine, bool isHorizontal)
  {
    int count = 0;
    foreach (var kvp in edgesByLine)
    {
      var edges = kvp.Value;
      // Sort by the start coordinate
      edges.Sort((a, b) => a.start.CompareTo(b.start));

      int currentStart = edges[0].start;
      int currentEnd = edges[0].end;

      // Merge consecutive edges
      for (int i = 1; i < edges.Count; i++)
      {
        var (s, e) = edges[i];
        if (s == currentEnd)
        {
          // Extend the current segment
          currentEnd = e;
        }
        else
        {
          // Finish the current segment and start a new one
          count++;
          currentStart = s;
          currentEnd = e;
        }
      }

      // Finish the last segment
      count++;
    }

    return count;
  }

  /// <summary>
  /// Checks if the given coordinates (r, c) are within the grid bounds.
  /// </summary>
  private static bool IsInGrid(int rows, int cols, int r, int c)
  {
    return r >= 0 && r < rows && c >= 0 && c < cols;
  }

  public static void Main()
  {
    // Example usage:
    // Update this to load your actual data file or use the provided example
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

    string filePath = GetInputPath("input_day12.txt");
    string[] largeMap1 = File.ReadAllLines(filePath);

    int result = Solve(largeMap);
    Console.WriteLine("Total price: " + result);
  }

  static string GetInputPath(string fileName)
  {
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
    return path;
  }
}
