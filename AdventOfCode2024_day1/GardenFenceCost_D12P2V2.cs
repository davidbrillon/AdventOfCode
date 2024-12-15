using System;
using System.Collections.Generic;

public class GardenFenceCost_D12P2V2
{
  private static readonly int[] dr = { -1, 1, 0, 0 };
  private static readonly int[] dc = { 0, 0, -1, 1 };

  public static int Solve(string[] rawGrid)
  {
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
          var regionCells = GetRegionCells(rawGrid, visited, r, c, plantType);
          int area = regionCells.Count;

          bool[,] horizontalEdges = new bool[rows + 1, cols];
          bool[,] verticalEdges = new bool[rows, cols + 1];
          MarkBoundaryEdges(rawGrid, regionCells, horizontalEdges, verticalEdges);

          int sides = CountSidesByRightHandRule(rows, cols, horizontalEdges, verticalEdges);

          int price = area * sides;
          totalPrice += price;
          Console.WriteLine($"A region of {plantType} plants with price {area} * {sides} = {price}.");
        }
      }
    }

    return totalPrice;
  }

  private static List<(int, int)> GetRegionCells(string[] grid, bool[,] visited, int startR, int startC, char plantType)
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

  private static void MarkBoundaryEdges(string[] grid, List<(int, int)> regionCells, bool[,] horizontalEdges, bool[,] verticalEdges)
  {
    int rows = grid.Length;
    int cols = grid[0].Length;
    var regionSet = new HashSet<(int, int)>(regionCells);

    foreach (var (cr, cc) in regionCells)
    {
      // Top
      if (cr == 0 || !regionSet.Contains((cr - 1, cc)))
        horizontalEdges[cr, cc] = true;

      // Bottom
      if (cr == rows - 1 || !regionSet.Contains((cr + 1, cc)))
        horizontalEdges[cr + 1, cc] = true;

      // Left
      if (cc == 0 || !regionSet.Contains((cr, cc - 1)))
        verticalEdges[cr, cc] = true;

      // Right
      if (cc == cols - 1 || !regionSet.Contains((cr, cc + 1)))
        verticalEdges[cr, cc + 1] = true;
    }
  }

  // Directions: up=0, right=1, down=2, left=3 for convenience
  // We'll always try to turn right to remain on the polygon boundary in a clockwise manner.
  private static int CountSidesByRightHandRule(int rows, int cols, bool[,] horizontalEdges, bool[,] verticalEdges)
  {
    // Build a set of edges
    var edges = new HashSet<((int, int), (int, int))>();
    for (int r = 0; r <= rows; r++)
    {
      for (int c = 0; c < cols; c++)
      {
        if (horizontalEdges[r, c])
        {
          var A = (r, c);
          var B = (r, c + 1);
          edges.Add(NormalizeEdge(A, B));
        }
      }
    }

    for (int r = 0; r < rows; r++)
    {
      for (int c = 0; c <= cols; c++)
      {
        if (verticalEdges[r, c])
        {
          var A = (r, c);
          var B = (r + 1, c);
          edges.Add(NormalizeEdge(A, B));
        }
      }
    }

    int totalSides = 0;
    var usedEdges = new HashSet<((int, int), (int, int))>();

    while (true)
    {
      // Find an unused edge with the smallest row,col start to have a consistent start point
      ((int, int), (int, int)) startEdge = default;
      bool found = false;
      foreach (var e in edges)
      {
        if (!usedEdges.Contains(e))
        {
          startEdge = e;
          found = true;
          break;
        }
      }
      if (!found) break; // no unused edges left

      var loop = ExtractLoopClockwise(startEdge, edges, usedEdges);
      if (loop.Count > 0)
        totalSides += CountSidesInLoop(loop);
    }

    return totalSides;
  }

  private static ((int, int), (int, int)) NormalizeEdge((int, int) A, (int, int) B)
  {
    if (A.Item1 < B.Item1) return (A, B);
    if (A.Item1 > B.Item1) return (B, A);
    if (A.Item2 < B.Item2) return (A, B);
    return (B, A);
  }

  private static void MarkEdgeUsed(HashSet<((int, int), (int, int))> usedEdges, (int, int) X, (int, int) Y)
  {
    usedEdges.Add(NormalizeEdge(X, Y));
  }

  // Extract a loop by always trying to turn right.
  // Directions: up=0, right=1, down=2, left=3
  // Given an edge A->B, determine initial direction and then at each step pick the next edge that corresponds
  // to turning right if possible, else go straight, else turn left, etc. Always bias toward turning right.
  private static List<(int, int)> ExtractLoopClockwise(((int, int), (int, int)) startEdge, HashSet<((int, int), (int, int))> edges, HashSet<((int, int), (int, int))> usedEdges)
  {
    var (A, B) = startEdge;
    MarkEdgeUsed(usedEdges, A, B);

    var loop = new List<(int, int)>();
    loop.Add(A);
    loop.Add(B);

    int dir = GetDirection(A, B); // initial direction
    var current = B;
    var prev = A;

    int safety = 0;
    while (!(current.Item1 == A.Item1 && current.Item2 == A.Item2))
    {
      var next = PickNextEdge(current, prev, dir, edges, usedEdges);
      if (next.Item1 == -1 && next.Item2 == -1)
      {
        // Could not continue the loop properly
        return new List<(int, int)>();
      }

      MarkEdgeUsed(usedEdges, current, next);
      loop.Add(next);
      prev = current;
      // new direction
      dir = GetDirection(current, next);
      current = next;

      safety++;
      if (safety > 1000000) // emergency
        break;
    }

    // Check if closed properly
    if (!(current.Item1 == A.Item1 && current.Item2 == A.Item2))
    {
      // not closed
      return new List<(int, int)>();
    }

    return loop;
  }

  // Determine direction from A to B
  private static int GetDirection((int, int) A, (int, int) B)
  {
    if (A.Item1 == B.Item1)
    {
      return (B.Item2 > A.Item2) ? 1 : 3; // right : left
    }
    else
    {
      return (B.Item1 > A.Item1) ? 2 : 0; // down : up
    }
  }

  // Attempt to turn right from current direction:
  // Direction order: up=0, right=1, down=2, left=3
  // Turning right: (dir+1)%4, straight: dir, left: (dir+3)%4, back: (dir+2)%4
  // We'll try directions in order of preference: right turn, straight, left turn, back
  private static (int, int) PickNextEdge((int, int) current, (int, int) prev, int dir, HashSet<((int, int), (int, int))> edges, HashSet<((int, int), (int, int))> usedEdges)
  {
    // All possible directions from current corner:
    // up: (current.Item1-1, current.Item2)
    // right: (current.Item1, current.Item2+1)
    // down: (current.Item1+1, current.Item2)
    // left: (current.Item1, current.Item2-1)

    // Try directions in order: right turn: (dir+1)%4, straight: dir, left: (dir+3)%4, back: (dir+2)%4
    int[] testOrder = { (dir + 1) % 4, dir, (dir + 3) % 4, (dir + 2) % 4 };
    foreach (int ndir in testOrder)
    {
      var nextCorner = Move(current, ndir);
      if (IsEdge(current, nextCorner, edges, usedEdges) && !(nextCorner.Item1 == prev.Item1 && nextCorner.Item2 == prev.Item2))
      {
        return nextCorner;
      }
    }

    return (-1, -1); // no suitable next edge
  }

  private static (int, int) Move((int, int) corner, int d)
  {
    // d: 0=up,1=right,2=down,3=left
    if (d == 0) return (corner.Item1 - 1, corner.Item2);
    if (d == 1) return (corner.Item1, corner.Item2 + 1);
    if (d == 2) return (corner.Item1 + 1, corner.Item2);
    return (corner.Item1, corner.Item2 - 1);
  }

  private static bool IsEdge((int, int) A, (int, int) B, HashSet<((int, int), (int, int))> edges, HashSet<((int, int), (int, int))> usedEdges)
  {
    if (B.Item1 < 0 || B.Item2 < 0) return false;
    var E = NormalizeEdge(A, B);
    return edges.Contains(E) && !usedEdges.Contains(E);
  }

  private static int CountSidesInLoop(List<(int, int)> loop)
  {
    if (loop.Count < 2) return 0;
    int sideCount = 1;
    var prev = loop[0];
    var curr = loop[1];
    bool prevHorizontal = (curr.Item1 == prev.Item1);

    for (int i = 2; i < loop.Count; i++)
    {
      var next = loop[i];
      bool horizontal = (next.Item1 == curr.Item1);
      if (horizontal != prevHorizontal) sideCount++;
      prevHorizontal = horizontal;
      curr = next;
    }

    return sideCount;
  }

  public static void Main()
  {
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
    string[] largeMapFinal = File.ReadAllLines(filePath);

    int result = Solve(largeMapFinal);
    Console.WriteLine("Total price: " + result);
    // Expected: 893676
  }

  static string GetInputPath(string fileName)
  {
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
    return path;
  }
}
