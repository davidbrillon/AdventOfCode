using System;
using System.Collections.Generic;

class GardenFenceCost_D12P1
{
  static char[][] ParseMap(string[] inputMap)
  {
    char[][] grid = new char[inputMap.Length][];

    for (int i = 0; i < inputMap.Length; i++)
    {
      grid[i] = inputMap[i].ToCharArray();
    }

    return grid;
  }

  static (int, int) CalculateAreaAndPerimeter(char[][] grid, int startRow, int startCol, char plantType, bool[][] visited)
  {
    int rows = grid.Length;
    int cols = grid[0].Length;
    int[][] directions = new int[][] { new[] { -1, 0 }, new[] { 1, 0 }, new[] { 0, -1 }, new[] { 0, 1 } };
    Queue<(int, int)> queue = new Queue<(int, int)>();
    queue.Enqueue((startRow, startCol));
    visited[startRow][startCol] = true;

    int area = 0;
    int perimeter = 0;

    while (queue.Count > 0)
    {
      (int row, int col) = queue.Dequeue();
      area++;

      foreach (var dir in directions)
      {
        int nr = row + dir[0];
        int nc = col + dir[1];

        if (nr >= 0 && nr < rows && nc >= 0 && nc < cols)
        {
          if (grid[nr][nc] == plantType && !visited[nr][nc])
          {
            visited[nr][nc] = true;
            queue.Enqueue((nr, nc));
          }
          else if (grid[nr][nc] != plantType)
          {
            perimeter++;
          }
        }
        else
        {
          perimeter++;
        }
      }
    }

    return (area, perimeter);
  }

  static int TotalFencingPrice(string[] inputMap)
  {
    char[][] grid = ParseMap(inputMap);
    int rows = grid.Length;
    int cols = grid[0].Length;
    bool[][] visited = new bool[rows][];

    for (int i = 0; i < rows; i++)
    {
      visited[i] = new bool[cols];
    }

    int totalPrice = 0;

    for (int row = 0; row < rows; row++)
    {
      for (int col = 0; col < cols; col++)
      {
        if (!visited[row][col])
        {
          char plantType = grid[row][col];
          (int area, int perimeter) = CalculateAreaAndPerimeter(grid, row, col, plantType, visited);
          totalPrice += area * perimeter;
        }
      }
    }

    return totalPrice;
  }

  public static void Main()
  {
    // Part 1 Answer: 1494342

    string[] inputMap_ = new string[]
    {
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
    string[] inputMap = File.ReadAllLines(filePath);

    Console.WriteLine(TotalFencingPrice(inputMap));
  }

  static string GetInputPath(string fileName)
  {
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
    return path;
  }
}
