// See https://aka.ms/new-console-template for more information

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
//day5_part2_opt();
//day6_part1();
//day7_part1();
//day7_part2();
//day8_part1();
//day8_part2();

DiskCompactionPart2.Main();

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
  // Part 1 answer: 4656

  string[] gridInput_ = [
    "....#.....",
    ".........#",
    "..........",
    "..#.......",
    ".......#..",
    "..........",
    ".#..^.....",
    "........#.",
    "#.........",
    "......#..."
  ];

  string filePath = GetInputPath("input_day6.txt");
  string[] gridInput = File.ReadAllLines(filePath);



  //var count = SimulateGuard(gridInput);
  var count = FindLoopObstructionPositions(gridInput);

}

static int SimulateGuard(string[] gridInput)
{
  int rows = gridInput.Length;
  int cols = gridInput[0].Length;
  char[,] grid = new char[rows, cols];
  (int x, int y) guardPosition = (0, 0); // (x: column, y: row)
  char direction = '^';

  // Initialize grid and find guard's starting position
  for (int y = 0; y < rows; y++) // y: row index
  {
    for (int x = 0; x < cols; x++) // x: column index
    {
      grid[y, x] = gridInput[y][x];
      if ("^v<>".Contains(grid[y, x]))
      {
        guardPosition = (x, y); // Set start position
        direction = grid[y, x];
        grid[y, x] = '.'; // Replace with empty space
      }
    }
  }

  // Directions: Up, Right, Down, Left (dy, dx)
  (int dx, int dy)[] directions = { (0, -1), (1, 0), (0, 1), (-1, 0) }; 
  int directionIndex = "^>v<".IndexOf(direction);

  HashSet<(int, int)> visited = new HashSet<(int, int)> { guardPosition };

  while (true)
  {
    // Calculate next position
    (int nx, int ny) = (
        guardPosition.x + directions[directionIndex].dx,
        guardPosition.y + directions[directionIndex].dy
    );

    // Check if next position is out of bounds or has an obstacle

    if (nx < 0 || ny < 0 || nx >= cols || ny >= rows)
    {
      break;  
    }
    else if (grid[ny, nx] == '#')
    {
      // Turn right if the guard cannot move forward
      directionIndex = (directionIndex + 1) % 4;
    }
    else
    {
      // Move forward if possible
      guardPosition = (nx, ny);
      visited.Add(guardPosition);

      // Stop if the guard moves out of bounds
      if (guardPosition.x < 0 || guardPosition.y < 0 || guardPosition.x >= cols || guardPosition.y >= rows)
        break;
    }

    // Break condition to ensure no infinite loop
    if (visited.Count >= rows * cols) // Safety condition
      break;
  }

  return visited.Count;
}

//PART2
// Answer 1575


static int FindLoopObstructionPositions(string[] gridInput)
{
  int rows = gridInput.Length;
  int cols = gridInput[0].Length;
  char[,] grid = new char[rows, cols];
  (int x, int y) guardStart = (0, 0); // Guard's starting position
  char direction = '^';

  // Initialize grid and find guard's starting position
  for (int y = 0; y < rows; y++)
  {
    for (int x = 0; x < cols; x++)
    {
      grid[y, x] = gridInput[y][x];
      if ("^v<>".Contains(grid[y, x]))
      {
        guardStart = (x, y);
        direction = grid[y, x];
        grid[y, x] = '.'; // Replace with empty space
      }
    }
  }

  // Get the initial path of the guard
  HashSet<(int x, int y)> guardPath = GetGuardPath(grid, guardStart, direction);

  // Test each position on the guard's path for validity
  int validPositions = 0;

  foreach (var pos in guardPath)
  {
    if (pos != guardStart)
    {
      // Temporarily add an obstruction
      grid[pos.y, pos.x] = '#';

      // Check if this causes a loop
      if (SimulateGuardLoop(grid, guardStart, direction))
      {
        validPositions++;
      }

      // Remove the obstruction
      grid[pos.y, pos.x] = '.';
    }
  }

  return validPositions;
}

static HashSet<(int x, int y)> GetGuardPath(char[,] grid, (int x, int y) guardStart, char direction)
{
  int rows = grid.GetLength(0);
  int cols = grid.GetLength(1);

  // Directions: Up, Right, Down, Left (dy, dx)
  (int dx, int dy)[] directions = { (0, -1), (1, 0), (0, 1), (-1, 0) };
  int directionIndex = "^>v<".IndexOf(direction);

  (int x, int y) guardPosition = guardStart;
  HashSet<(int, int)> path = new HashSet<(int, int)> { guardPosition };

  while (true)
  {
    //path.Add(guardPosition);

    // Calculate next position
    (int nx, int ny) = (
        guardPosition.x + directions[directionIndex].dx,
        guardPosition.y + directions[directionIndex].dy
    );

    // Check if next position is out of bounds or has an obstacle
    if (nx < 0 || ny < 0 || nx >= cols || ny >= rows)
    {
      break;
    }
    else if (grid[ny, nx] == '#')
    {
      // Turn right if the guard cannot move forward
      directionIndex = (directionIndex + 1) % 4;
    }
    else
    {
      // Move forward
      guardPosition = (nx, ny);
      path.Add(guardPosition);

      // Stop if the guard moves out of bounds
      if (guardPosition.x < 0 || guardPosition.y < 0 || guardPosition.x >= cols || guardPosition.y >= rows)
        break;
    }

    // Stop if the guard revisits the starting position
    //if (guardPosition == guardStart && path.Count > 1)
    //  break;

    // Safety condition to avoid infinite loops
    if (path.Count > rows * cols)
      break;
  }

  return path;
}

static bool SimulateGuardLoop(char[,] grid, (int x, int y) guardStart, char direction)
{
  int rows = grid.GetLength(0);
  int cols = grid.GetLength(1);

  // Directions: Up, Right, Down, Left (dy, dx)
  (int dx, int dy)[] directions = { (0, -1), (1, 0), (0, 1), (-1, 0) };
  int directionIndex = "^>v<".IndexOf(direction);

  (int x, int y) guardPosition = guardStart;
  HashSet<(int, int, int)> visitedStates = new HashSet<(int, int, int)>(); // Track (x, y, directionIndex)

  while (true)
  {
    // Check if this state has been seen before
    if (visitedStates.Contains((guardPosition.x, guardPosition.y, directionIndex)))
    {
      // Guard is stuck in a loop
      return true;
    }

    visitedStates.Add((guardPosition.x, guardPosition.y, directionIndex));

    // Calculate next position
    (int nx, int ny) = (
        guardPosition.x + directions[directionIndex].dx,
        guardPosition.y + directions[directionIndex].dy
    );

    // Check if next position is out of bounds or has an obstacle
    if (nx < 0 || ny < 0 || nx >= cols || ny >= rows)
    {
      break;
    } else if (grid[ny, nx] == '#')
    {
      // Turn right if the guard cannot move forward
      directionIndex = (directionIndex + 1) % 4;
    }
    else
    {
      // Move forward
      guardPosition = (nx, ny);
    }

    // Break condition to ensure no infinite loop
    if (visitedStates.Count > rows * cols) // Safety condition
      break;
  }

  return false; // Guard does not get stuck in a loop
}

///DAY7
static void day7_part1()
{
  // Answer: 1289579105366
  // Sample input lines
  string[] inputLines_ = new string[]
  {
            "190: 10 19",
            "3267: 81 40 27",
            "83: 17 5",
            "156: 15 6",
            "7290: 6 8 6 15",
            "161011: 16 10 13",
            "192: 17 8 14",
            "21037: 9 7 18 13",
            "292: 11 6 16 20"
  };

  string filePath = GetInputPath("input_day7.txt");
  string[] inputLines = File.ReadAllLines(filePath);

  long totalCalibrationResult = 0;

  foreach (var line in inputLines)
  {
    // Split the line into test value and numbers
    var parts = line.Split(':');
    if (parts.Length != 2)
    {
      Console.WriteLine($"Invalid line format: {line}");
      continue;
    }

    // Parse the test value
    if (!long.TryParse(parts[0].Trim(), out long testValue))
    {
      Console.WriteLine($"Invalid test value in line: {line}");
      continue;
    }

    // Parse the numbers
    var numberStrings = parts[1].Trim().Split(' ');
    List<long> numbers = new List<long>();
    bool validNumbers = true;
    foreach (var numStr in numberStrings)
    {
      if (long.TryParse(numStr, out long num))
      {
        numbers.Add(num);
      }
      else
      {
        Console.WriteLine($"Invalid number '{numStr}' in line: {line}");
        validNumbers = false;
        break;
      }
    }

    if (!validNumbers || numbers.Count == 0)
    {
      continue;
    }

    // Generate all possible operator combinations
    int operatorSlots = numbers.Count - 1;
    int totalCombinations = (int)Math.Pow(2, operatorSlots);
    bool isValid = false;

    for (int i = 0; i < totalCombinations; i++)
    {
      // Generate operator sequence based on the bits of i
      List<char> operators = new List<char>();
      for (int bit = 0; bit < operatorSlots; bit++)
      {
        if ((i & (1 << bit)) != 0)
          operators.Add('*');
        else
          operators.Add('+');
      }

      // Evaluate the expression left-to-right
      long result = numbers[0];
      for (int j = 0; j < operators.Count; j++)
      {
        if (operators[j] == '+')
        {
          result += numbers[j + 1];
        }
        else if (operators[j] == '*')
        {
          result *= numbers[j + 1];
        }
      }

      // Check if the result matches the test value
      if (result == testValue)
      {
        isValid = true;
        break; // No need to check other combinations
      }
    }

    if (isValid)
    {
      totalCalibrationResult += testValue;
      Console.WriteLine($"Valid: {line}");
    }
    else
    {
      Console.WriteLine($"Invalid: {line}");
    }
  }

  Console.WriteLine($"\nTotal Calibration Result: {totalCalibrationResult}");
}


static void day7_part2()
{
  // Answer: 92148721834692  
  // Sample input lines
  string[] inputLines_ = new string[]
  {
            "190: 10 19",
            "3267: 81 40 27",
            "83: 17 5",
            "156: 15 6",
            "7290: 6 8 6 15",
            "161011: 16 10 13",
            "192: 17 8 14",
            "21037: 9 7 18 13",
            "292: 11 6 16 20"
  };
  string filePath = GetInputPath("input_day7.txt");
  string[] inputLines = File.ReadAllLines(filePath);

  long totalCalibrationResult = 0;

  foreach (var line in inputLines)
  {
    // Split the line into test value and numbers
    var parts = line.Split(':');
    if (parts.Length != 2)
    {
      Console.WriteLine($"Invalid line format: {line}");
      continue;
    }

    // Parse the test value
    if (!long.TryParse(parts[0].Trim(), out long testValue))
    {
      Console.WriteLine($"Invalid test value in line: {line}");
      continue;
    }

    // Parse the numbers
    var numberStrings = parts[1].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    List<long> numbers = new List<long>();
    bool validNumbers = true;
    foreach (var numStr in numberStrings)
    {
      if (long.TryParse(numStr, out long num))
      {
        numbers.Add(num);
      }
      else
      {
        Console.WriteLine($"Invalid number '{numStr}' in line: {line}");
        validNumbers = false;
        break;
      }
    }

    if (!validNumbers || numbers.Count == 0)
    {
      continue;
    }

    // Generate all possible operator combinations
    int operatorSlots = numbers.Count - 1;
    long totalCombinations = (long)Math.Pow(3, operatorSlots);
    bool isValid = false;

    for (long i = 0; i < totalCombinations; i++)
    {
      // Generate operator sequence based on the base-3 representation of i
      List<Operator> operators = new List<Operator>();
      long temp = i;
      for (int slot = 0; slot < operatorSlots; slot++)
      {
        int op = (int)(temp % 3);
        operators.Add((Operator)op);
        temp /= 3;
      }

      // Evaluate the expression left-to-right
      long result = numbers[0];
      for (int j = 0; j < operators.Count; j++)
      {
        switch (operators[j])
        {
          case Operator.Add:
            result += numbers[j + 1];
            break;
          case Operator.Multiply:
            result *= numbers[j + 1];
            break;
          case Operator.Concatenate:
            result = ConcatenateNumbers(result, numbers[j + 1]);
            break;
        }

        // Early termination if result exceeds testValue and operators do not include concatenation
        // This can optimize for some cases but is optional
      }

      // Check if the result matches the test value
      if (result == testValue)
      {
        isValid = true;
        break; // No need to check other combinations
      }
    }

    if (isValid)
    {
      totalCalibrationResult += testValue;
      Console.WriteLine($"Valid: {line}");
    }
    else
    {
      Console.WriteLine($"Invalid: {line}");
    }
  }

  Console.WriteLine($"\nTotal Calibration Result: {totalCalibrationResult}");

}

/// <summary>
/// Concatenates two numbers by combining their digits.
/// For example, ConcatenateNumbers(12, 345) returns 12345.
/// </summary>
/// <param name="left">The left number.</param>
/// <param name="right">The right number.</param>
/// <returns>The concatenated number.</returns>
static long ConcatenateNumbers(long left, long right)
{
  if (right == 0)
    return left * 10;

  long multiplier = 1;
  long temp = right;
  while (temp > 0)
  {
    multiplier *= 10;
    temp /= 10;
  }
  return left * multiplier + right;
}

static void day8_part1()
{
  // Answer = 293
  // Example input as a string array
  string[] inputLines_ = new string[]
  {
            "............",
            "........0...",
            ".....0......",
            ".......0....",
            "....0.......",
            "......A.....",
            "............",
            "............",
            "........A...",
            ".........A..",
            "............",
            "............"
  };
  string filePath = GetInputPath("input_day8.txt");
  string[] inputLines = File.ReadAllLines(filePath);


  int height = inputLines.Length;
  int width = inputLines[0].Length;

  // Dictionary mapping frequencies to list of antenna positions
  Dictionary<char, List<(int x, int y)>> frequencyAntennas = new Dictionary<char, List<(int x, int y)>>();

  for (int y = 0; y < height; y++)
  {
    string row = inputLines[y];
    for (int x = 0; x < width; x++)
    {
      char c = row[x];
      if (char.IsLetterOrDigit(c))
      {
        if (!frequencyAntennas.ContainsKey(c))
        {
          frequencyAntennas[c] = new List<(int x, int y)>();
        }
        frequencyAntennas[c].Add((x, y));
      }
    }
  }

  // HashSet to store unique antinode positions
  HashSet<(int x, int y)> antinodePositions = new HashSet<(int x, int y)>();

  foreach (var kvp in frequencyAntennas)
  {
    List<(int x, int y)> antennas = kvp.Value;

    int n = antennas.Count;
    for (int i = 0; i < n; i++)
    {
      var A = antennas[i];
      for (int j = i + 1; j < n; j++)
      {
        var B = antennas[j];

        // Compute the two antinode positions for the pair (A, B)

        // First antinode P1
        int P1_x = 2 * A.x - B.x;
        int P1_y = 2 * A.y - B.y;

        if (IsValid(P1_x, P1_y, width, height))
        {
          antinodePositions.Add((P1_x, P1_y));
        }

        // Second antinode P2
        int P2_x = 2 * B.x - A.x;
        int P2_y = 2 * B.y - A.y;

        if (IsValid(P2_x, P2_y, width, height))
        {
          antinodePositions.Add((P2_x, P2_y));
        }
      }
    }
  }

  Console.WriteLine(antinodePositions.Count);
}

// Helper method to check if a position is valid within the bounds of the map
static bool IsValid(int x, int y, int width, int height)
{
  return x >= 0 && x < width && y >= 0 && y < height;
}

/// PART 2

static void day8_part2()
{
  // Answer is 934

  string[] inputLines_ = new string[]
  {
        "............",
        "........0...",
        ".....0......",
        ".......0....",
        "....0.......",
        "......A.....",
        "............",
        "............",
        "........A...",
        ".........A..",
        "............",
        "............"
  };

  string filePath = GetInputPath("input_day8.txt");
  string[] inputLines = File.ReadAllLines(filePath);

  int height = inputLines.Length;
  int width = inputLines[0].Length;

  // Dictionary mapping frequencies to list of antenna positions
  Dictionary<char, List<(int x, int y)>> frequencyAntennas = new Dictionary<char, List<(int x, int y)>>();

  for (int y = 0; y < height; y++)
  {
    string row = inputLines[y];
    for (int x = 0; x < width; x++)
    {
      char c = row[x];
      if (char.IsLetterOrDigit(c))
      {
        if (!frequencyAntennas.ContainsKey(c))
        {
          frequencyAntennas[c] = new List<(int x, int y)>();
        }
        frequencyAntennas[c].Add((x, y));
      }
    }
  }

  // HashSet to store unique antinode positions
  HashSet<(int x, int y)> antinodePositions = new HashSet<(int x, int y)>();

  foreach (var kvp in frequencyAntennas)
  {
    char frequency = kvp.Key;
    List<(int x, int y)> antennas = kvp.Value;

    if (antennas.Count == 1)
    {
      // Single antenna of a unique frequency creates an antinode only at its position
      antinodePositions.Add(antennas[0]);
      continue;
    }

    // For each pair of antennas, calculate all positions in line
    for (int i = 0; i < antennas.Count; i++)
    {
      var A = antennas[i];
      antinodePositions.Add(A); // Add the antenna's position

      for (int j = i + 1; j < antennas.Count; j++)
      {
        var B = antennas[j];

        // Calculate the step size using GCD
        int dx = B.x - A.x;
        int dy = B.y - A.y;
        int gcd = GCD(Math.Abs(dx), Math.Abs(dy));
        dx /= gcd;
        dy /= gcd;

        // Traverse the line in both directions
        AddAntinodesOnLine(A, dx, dy, width, height, antinodePositions);
        AddAntinodesOnLine(B, -dx, -dy, width, height, antinodePositions);
      }
    }
  }

  Console.WriteLine(antinodePositions.Count);
}

// Helper method to add antinodes along a line in a specific direction
static void AddAntinodesOnLine((int x, int y) start, int dx, int dy, int width, int height, HashSet<(int x, int y)> antinodePositions)
{
  int x = start.x + dx;
  int y = start.y + dy;

  while (IsValid2(x, y, width, height))
  {
    antinodePositions.Add((x, y));
    x += dx;
    y += dy;
  }
}

// Helper method to check if a position is valid within the bounds of the map
static bool IsValid2(int x, int y, int width, int height)
{
  return x >= 0 && x < width && y >= 0 && y < height;
}

// Helper method to compute the GCD of two integers
static int GCD(int a, int b)
{
  while (b != 0)
  {
    int temp = b;
    b = a % b;
    a = temp;
  }
  return a;
}


