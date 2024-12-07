﻿// See https://aka.ms/new-console-template for more information

// Day 1 - part 1
// Read and parse the file into two arrays
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;


var data = File.ReadAllLines(GetInputPath("input.txt"))
               .Select(line => line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(int.Parse).ToArray())
               .ToArray();

// Extract the left and right lists
var left = data.Select(pair => pair[0]).OrderBy(x => x).ToArray();
var right = data.Select(pair => pair[1]).OrderBy(x => x).ToArray();

// Calculate the total distance and output the result
int totalDistance = left.Zip(right, (l, r) => Math.Abs(l - r)).Sum();
Console.WriteLine($"Total Distance: {totalDistance}");

// Day 1 - part 2
int simScore = 0;
for (var i = 0; i < left.Length; i++)
{
  int counter = 0;
  for (var j = 0; j < right.Length; j++)
  {
    if (left[i] == right[j])
    {
      counter++;
    }
  }

  simScore += left[i] * counter;
}

int simScore2 = 0;
Span<int> leftSpan = left.AsSpan();
Span<int> rightSpan = right.AsSpan();

for (var i = 0; i < leftSpan.Length; i++)
{
  int counter = 0;
  for (var j = 0; j < rightSpan.Length; j++)
  {
    if (rightSpan[j] == leftSpan[i])
    {
      counter++;
    }
  }
  simScore2 += leftSpan[i] * counter;
}

Console.WriteLine($"Similarity Score Distance: {simScore}");

//day2();
//day3_part1();
//day3_part2();
//day4_part1();
//day4_part2();
//day5_part1();
//day5_part1_opt();
day5_part2_opt();

static string GetInputPath(string fileName)
{
  var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
  return path;
}

static void day2()
{
  // Part 1: Answer is 379
  // Part 2: Answer is 430
  // Read the input data from the file
  string filePath = GetInputPath("input_day2.txt");
  //string filePath = GetInputPath("test_day2.txt");
  string[] reports = File.ReadAllLines(filePath);

  int safeCount = 0;

  foreach (string report in reports)
  {
    // Split the levels into an array of integers
    int[] levels = report.Split(' ').Select(int.Parse).ToArray();

    if (IsSafeReport(levels))
    {
      safeCount++;
    } 
    else
    {
      // PART 2
      for (int i = 0; i < levels.Length; i++)
      {
        // get an array with all the value except the one represented by the index
        int[] newLevels = levels.Where((source, index) => index != i).ToArray();
        if (IsSafeReport(newLevels))
        {
          safeCount++;
          break;
        }
      }
    }
  }

  Console.WriteLine($"Number of safe reports: {safeCount}"); 
}

static bool IsSafeReport(int[] levels)
{
  int isIncreasing = 0;
  int isDecreasing = 0;

  for (int i = 1; i < levels.Length; i++)
  {
    int diff = (levels[i] - levels[i - 1]);

    if (Math.Abs(diff) < 1 || Math.Abs(diff) > 3)
    {
      return false; // Adjacent levels differ by less than 1 or more than 3
    }

    if (diff < 0)
    {
      isIncreasing++;
    }
    else if (diff > 0)
    {
      isDecreasing++;
    }
  }

  // A report is safe if it is either strictly increasing or strictly decreasing
  bool isNotSafe = (isIncreasing > 0) && (isDecreasing > 0);
  return !isNotSafe;
}

// Day 3
static void day3_part1()
{
  // Answer Part 1 = 179571322

  //string input = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
  string filePath = GetInputPath("input_day3.txt");
  string input = File.ReadAllText(filePath);
    
  // Define a regex pattern to match valid mul(X,Y) instructions
  string pattern = @"mul\((\d+),(\d+)\)";
  Regex regex = new Regex(pattern);

  int sum = 0;

  // Use regex to find matches and directly extract groups for numbers
  foreach (Match match in regex.Matches(input))
  {
    int x = int.Parse(match.Groups[1].Value);
    int y = int.Parse(match.Groups[2].Value);
    sum += x * y;
  }

  Console.WriteLine($"The total sum is: {sum}");
}

static void day3_part2()
{
  // Answer Part 2 = 103811193

  // Specify the file path
  string filePath = GetInputPath("input_day3.txt");
  string input = File.ReadAllText(filePath);
  //input = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";

  //// Define regex patterns
  //string mulPattern = @"mul\((\d+),(\d+)\)";
  //string controlPattern = @"do\(\)|don't\(\)";
  //Regex combinedRegex = new Regex($"{controlPattern}|{mulPattern}");
  //var matches = combinedRegex.Matches(input);
  var matches = new Regex(@"do\(\)|don't\(\)|mul\((\d+),(\d+)\)").Matches(input);

  // Variables to track state and sum
  bool mulEnabled = true;
  int sum = 0;

  foreach (Match match in matches)
  {
    if (match.Value == "do()") mulEnabled = true;
    else if (match.Value == "don't()") mulEnabled = false;
    else if (mulEnabled && match.Groups.Count == 3)
      sum += int.Parse(match.Groups[1].Value) * int.Parse(match.Groups[2].Value);
  }

  // Output the result
  Console.WriteLine($"The total sum is: {sum}");
}

// Day 4
static void day4_part1()
{
  // PArt 1 answer: 2517

  //string[] grid = {
  //          "MMMSXXMASM",
  //          "MSAMXMSMSA",
  //          "AMXSXMAAMM",
  //          "MSAMASMSMX",
  //          "XMASAMXAMM",
  //          "XXAMMXXAMA",
  //          "SMSMSASXSS",
  //          "SAXAMASAAA",
  //          "MAMMMXMMMM",
  //          "MXMXAXMASX"
  //      };

  string filePath = GetInputPath("input_day4.txt");
  string[] grid = File.ReadAllLines(filePath);

  string word = "XMAS";
  int count = CountOccurrences(grid, word);
  Console.WriteLine($"The word '{word}' appears {count} times in the grid.");
}

static int CountOccurrences(string[] grid, string word)
{
  int rows = grid.Length;
  int cols = grid[0].Length;
  int wordLength = word.Length;
  int count = 0;

  // Directions: (row delta, col delta)
  int[,] directions = {
            { 0, 1 },   // Horizontal right
            { 0, -1 },  // Horizontal left
            { 1, 0 },   // Vertical down
            { -1, 0 },  // Vertical up
            { 1, 1 },   // Diagonal down-right
            { 1, -1 },  // Diagonal down-left
            { -1, 1 },  // Diagonal up-right
            { -1, -1 }  // Diagonal up-left
        };

  for (int row = 0; row < rows; row++)
  {
    for (int col = 0; col < cols; col++)
    {
      for (int d = 0; d < directions.GetLength(0); d++)
      {
        int dr = directions[d, 0];
        int dc = directions[d, 1];
        if (CheckWord(grid, word, row, col, dr, dc))
        {
          count++;
        }
      }
    }
  }

  return count;
}

static bool CheckWord(string[] grid, string word, int row, int col, int dr, int dc)
{
  int rows = grid.Length;
  int cols = grid[0].Length;
  int wordLength = word.Length;

  for (int i = 0; i < wordLength; i++)
  {
    int r = row + i * dr;
    int c = col + i * dc;

    // Check bounds
    if (r < 0 || r >= rows || c < 0 || c >= cols || grid[r][c] != word[i])
    {
      return false;
    }
  }

  return true;
}

///////////////////////////
static void day4_part2()
{
  // PArt 2 answer: 1960

  //string[] gridInput = {
  //          "MMMSXXMASM",
  //          "MSAMXMSMSA",
  //          "AMXSXMAAMM",
  //          "MSAMASMSMX",
  //          "XMASAMXAMM",
  //          "XXAMMXXAMA",
  //          "SMSMSASXSS",
  //          "SAXAMASAAA",
  //          "MAMMMXMMMM",
  //          "MXMXAXMASX"
  //      };

  string filePath = GetInputPath("input_day4.txt");
  string[] gridInput = File.ReadAllLines(filePath);


  char[,] grid = ConvertToGrid(gridInput);
  int count = CountXmasPatterns(grid);
  Console.WriteLine($"Number of X-MAS patterns: {count}");
}

static char[,] ConvertToGrid(string[] input)
{
  int rows = input.Length;
  int cols = input[0].Length;
  char[,] grid = new char[rows, cols];

  for (int i = 0; i < rows; i++)
  {
    for (int j = 0; j < cols; j++)
    {
      grid[i, j] = input[i][j];
    }
  }

  return grid;
}

static int CountXmasPatterns(char[,] grid)
{
  int rows = grid.GetLength(0);
  int cols = grid.GetLength(1);
  int count = 0;

  for (int row = 1; row < rows - 1; row++)
  {
    for (int col = 1; col < cols - 1; col++)
    {
      if (IsXmasPattern(grid, row, col))
        count++;
    }
  }

  return count;
}

static bool IsXmasPattern(char[,] grid, int row, int col)
{
  // Check top-left to bottom-right diagonal
  string diagonal1 = $"{grid[row - 1, col - 1]}{grid[row, col]}{grid[row + 1, col + 1]}";
  // Check top-right to bottom-left diagonal
  string diagonal2 = $"{grid[row - 1, col + 1]}{grid[row, col]}{grid[row + 1, col - 1]}";

  return IsValidMas(diagonal1) && IsValidMas(diagonal2);
}

static bool IsValidMas(string diagonal)
{
  return diagonal == "MAS" || diagonal == "SAM";
}


// Day 5 Part 1. Ans 5248
// string filePath = GetInputPath("input_day5.txt");
// string[] input = File.ReadAllLines(filePath);
static void day5_part1()
{
  string[] input = {
    "47|53",
    "97|13",
    "97|61",
    "97|47",
    "75|29",
    "61|13",
    "75|53",
    "29|13",
    "97|29",
    "53|29",
    "61|53",
    "97|53",
    "61|29",
    "47|13",
    "75|47",
    "97|75",
    "47|61",
    "75|61",
    "47|29",
    "75|13",
    "53|13",
    "",
    "75,47,61,53,29",
    "97,61,53,29,13",
    "75,29,13",
    "75,97,47,61,53",
    "61,13,29",
    "97,13,75,29,47"
         };

  // create a list of tuple of int x and y
  List<(int x, int y)> rules = new List<(int x, int y)>();
  List<int[]> updates = new List<int[]>();

  // Parse the data
  for (int i = 0; i < input.Length; i++)
  {
    //Check if line contains | or ,
    if (input[i].Contains('|'))
    {
      string[] parts = input[i].Split('|');
      int x = int.Parse(parts[0]);
      int y = int.Parse(parts[1]);
      rules.Add((x, y));
      Console.WriteLine($"rule({x},{y})");
    }
    else if (input[i].Contains(','))
    {
      int[] parts = input[i].Split(',').Select(int.Parse).ToArray();
      updates.Add(parts);
      Console.WriteLine($"update({string.Join(",", parts)})");
    }
  }

  var sum = 0;
  foreach (var update in updates)
  {
    bool valid = true;
    foreach (var rule in rules)
    {
      //find the index of the rule.x in the update array
      int index1 = Array.IndexOf(update, rule.x);
      int index2 = Array.IndexOf(update, rule.y);

      if (index1 == -1 || index2 == -1)
        // ignore
        continue;

      if (index2 < index1)
      {
        valid = false;
        break;
      }
        
    }

    if (valid)
    {
      var pos = update.Length / 2;
      sum += update[pos];
    }
  }


  Console.WriteLine($"The total sum is: {sum}");
}


static void day5_part1_opt()
{
  //string[] input = {
  //    "47|53", "97|13", "97|61", "97|47", "75|29", "61|13", "75|53", "29|13", "97|29", "53|29",
  //    "61|53", "97|53", "61|29", "47|13", "75|47", "97|75", "47|61", "75|61", "47|29", "75|13",
  //    "53|13", "", "75,47,61,53,29", "97,61,53,29,13", "75,29,13", "75,97,47,61,53", "61,13,29",
  //    "97,13,75,29,47"
  //  };
   string filePath = GetInputPath("input_day5.txt");
   string[] input = File.ReadAllLines(filePath);

  var rules = input.TakeWhile(line => line.Contains('|'))
                   .Select(line => line.Split('|').Select(int.Parse).ToArray())
                   .Select(parts => (x: parts[0], y: parts[1]))
                   .ToList();

  var updates = input.SkipWhile(line => !line.Contains(','))
                     .Select(line => line.Split(',').Select(int.Parse).ToArray())
                     .ToList();

  var sum = updates.Where(update => rules.All(rule =>
  {
    int index1 = Array.IndexOf(update, rule.x);
    int index2 = Array.IndexOf(update, rule.y);
    return index1 == -1 || index2 == -1 || index1 < index2;
  }))
    .Sum(update => update[update.Length / 2]);

  Console.WriteLine($"The total sum is: {sum}");
}

static void day5_part2_opt()
{
  // Answer: 4507
  string[] input_ = {
      "47|53", "97|13", "97|61", "97|47", "75|29", "61|13", "75|53", "29|13", "97|29", "53|29",
      "61|53", "97|53", "61|29", "47|13", "75|47", "97|75", "47|61", "75|61", "47|29", "75|13",
      "53|13", "", "75,47,61,53,29", "97,61,53,29,13", "75,29,13", "75,97,47,61,53", "61,13,29",
      "97,13,75,29,47"
    };
  string filePath = GetInputPath("input_day5.txt");
  string[] input = File.ReadAllLines(filePath);

  var rules = input.TakeWhile(line => line.Contains('|'))
                   .Select(line => line.Split('|').Select(int.Parse).ToArray())
                   .Select(parts => (x: parts[0], y: parts[1]))
                   .ToList();

  var updates = input.SkipWhile(line => !line.Contains(','))
                     .Select(line => line.Split(',').Select(int.Parse).ToArray())
                     .ToList();

  var unorderedUpdates = updates.Where(update => !rules.All(rule =>
  {
    int index1 = Array.IndexOf(update, rule.x);
    int index2 = Array.IndexOf(update, rule.y);
    return index1 == -1 || index2 == -1 || index1 < index2;
  })).ToList();

  foreach (var update in unorderedUpdates)
  {
    bool isReordered;
    do
    {
      isReordered = false;

      foreach (var rule in rules)
      {
        int index1 = Array.IndexOf(update, rule.x);
        int index2 = Array.IndexOf(update, rule.y);

        // If both elements exist and are out of order, swap them
        if (index1 != -1 && index2 != -1 && index2 < index1)
        {
          (update[index2], update[index1]) = (update[index1], update[index2]);
          isReordered = true; // Mark as reordered to re-evaluate
          break; // Exit early to re-check from the beginning
        }
      }
    }
    while (isReordered); // Repeat until no more swaps are needed
  }

  var sum = unorderedUpdates.Sum(update => update[update.Length / 2]);
  
  Console.WriteLine($"The total sum is: {sum}");
}

static void day6_part1()
{
  // Part 1 answer: 1960

  string[] gridInput = [
    "....#.....",
    "....^....#",
    "..........",
    "..#.......",
    ".......#..",
    "..........",
    ".#........",
    "........#.",
    "#.........",
    "......#..."
  ];




  }
