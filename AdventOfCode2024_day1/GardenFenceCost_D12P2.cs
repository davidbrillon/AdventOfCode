using System;
using System.Collections.Generic;
using System.Linq;

public class GardenFenceCost_D12P2
{
  public static int Solve(string[] rawGrid)
  {
    // Ensure no trailing whitespace on each line
    for (int i = 0; i < rawGrid.Length; i++)
    {
      rawGrid[i] = rawGrid[i].TrimEnd();
    }

    int rows = rawGrid.Length;
    if (rows == 0) return 0;
    int cols = rawGrid[0].Length;

    bool[,] visited = new bool[rows, cols];

    // Directions for BFS
    int[] dr = { -1, 1, 0, 0 };
    int[] dc = { 0, 0, -1, 1 };

    bool IsInGrid(int r, int c) => r >= 0 && r < rows && c >= 0 && c < cols;

    int totalPrice = 0;

    for (int r = 0; r < rows; r++)
    {
      for (int c = 0; c < cols; c++)
      {
        if (!visited[r, c])
        {
          char plantType = rawGrid[r][c];

          // BFS to find the entire region
          Queue<(int, int)> queue = new Queue<(int, int)>();
          queue.Enqueue((r, c));
          visited[r, c] = true;

          List<(int, int)> regionCells = new List<(int, int)>();

          while (queue.Count > 0)
          {
            var (cr, cc) = queue.Dequeue();
            regionCells.Add((cr, cc));

            for (int i = 0; i < 4; i++)
            {
              int nr = cr + dr[i];
              int nc = cc + dc[i];
              if (IsInGrid(nr, nc) && !visited[nr, nc] && rawGrid[nr][nc] == plantType)
              {
                visited[nr, nc] = true;
                queue.Enqueue((nr, nc));
              }
            }
          }

          // Calculate area
          int area = regionCells.Count;

          // Collect boundary edges
          var horizontalEdges = new Dictionary<int, List<(int startC, int endC)>>();
          var verticalEdges = new Dictionary<int, List<(int startR, int endR)>>();

          void AddHorizontalEdge(int hr, int hc1, int hc2)
          {
            if (!horizontalEdges.ContainsKey(hr))
              horizontalEdges[hr] = new List<(int, int)>();
            horizontalEdges[hr].Add((Math.Min(hc1, hc2), Math.Max(hc1, hc2)));
          }

          void AddVerticalEdge(int hc, int hr1, int hr2)
          {
            if (!verticalEdges.ContainsKey(hc))
              verticalEdges[hc] = new List<(int, int)>();
            verticalEdges[hc].Add((Math.Min(hr1, hr2), Math.Max(hr1, hr2)));
          }

          // Identify boundary edges
          foreach (var (cr, cc) in regionCells)
          {
            // Top edge
            if (cr == 0 || rawGrid[cr - 1][cc] != plantType)
            {
              AddHorizontalEdge(cr, cc, cc + 1);
            }
            // Bottom edge
            if (cr == rows - 1 || rawGrid[cr + 1][cc] != plantType)
            {
              AddHorizontalEdge(cr + 1, cc, cc + 1);
            }
            // Left edge
            if (cc == 0 || rawGrid[cr][cc - 1] != plantType)
            {
              AddVerticalEdge(cc, cr, cr + 1);
            }
            // Right edge
            if (cc == cols - 1 || rawGrid[cr][cc + 1] != plantType)
            {
              AddVerticalEdge(cc + 1, cr, cr + 1);
            }
          }

          int sideCount = 0;

          // Merge horizontal edges
          foreach (var kvp in horizontalEdges)
          {
            var rowEdges = kvp.Value;
            rowEdges.Sort((a, b) => a.startC.CompareTo(b.startC));
            int currentStart = rowEdges[0].startC;
            int currentEnd = rowEdges[0].endC;

            for (int i = 1; i < rowEdges.Count; i++)
            {
              var (s, e) = rowEdges[i];
              if (s == currentEnd)
              {
                // continuous horizontal segment
                currentEnd = e;
              }
              else
              {
                sideCount++;
                currentStart = s;
                currentEnd = e;
              }
            }
            // Finish the last segment
            sideCount++;
          }

          // Merge vertical edges
          foreach (var kvp in verticalEdges)
          {
            var colEdges = kvp.Value;
            colEdges.Sort((a, b) => a.startR.CompareTo(b.startR));
            int currentStart = colEdges[0].startR;
            int currentEnd = colEdges[0].endR;

            for (int i = 1; i < colEdges.Count; i++)
            {
              var (s, e) = colEdges[i];
              if (s == currentEnd)
              {
                // continuous vertical segment
                currentEnd = e;
              }
              else
              {
                sideCount++;
                currentStart = s;
                currentEnd = e;
              }
            }
            // Finish the last segment
            sideCount++;
          }

          int price = area * sideCount;
          totalPrice += price;

          // Print the log for this region
          Console.WriteLine($"A region of {plantType} plants with price {area} * {sideCount} = {price}.");
        }
      }
    }

    return totalPrice;
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

    string filePath = GetInputPath("input_day12.txt");
    string[] largeMap1 = File.ReadAllLines(filePath);

    int result = Solve(largeMap1);
    Console.WriteLine("Total price: " + result);
  }

  static string GetInputPath(string fileName)
  {
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
    return path;
  }
}
