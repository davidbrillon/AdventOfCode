class DiskCompactionPart1
{
  static List<object> ParseDiskMap(string diskMap)
  {
    var blocks = new List<object>();
    int fileId = 0; // Track file IDs

    for (int i = 0; i < diskMap.Length; i += 2)
    {
      int fileSize = int.Parse(diskMap[i].ToString());
      int freeSize = (i + 1 < diskMap.Length) ? int.Parse(diskMap[i + 1].ToString()) : 0;

      // Add file blocks with the current file ID
      for (int j = 0; j < fileSize; j++)
      {
        blocks.Add(fileId);
      }

      // Increment file ID for the next file
      if (fileSize > 0) fileId++;

      // Add free space blocks
      for (int j = 0; j < freeSize; j++)
      {
        blocks.Add('.');
      }
    }
    return blocks;
  }

  static List<object> CompactBlocks(List<object> blocks)
  {
    for (int i = blocks.Count - 1; i >= 0; i--) // Start from the rightmost block
    {
      if (blocks[i] is int) // Found a file block
      {
        for (int j = 0; j < i; j++) // Find the leftmost free space
        {
          if (blocks[j] is char && blocks[j].Equals('.'))
          {
            // Move the file block to the leftmost free space
            blocks[j] = blocks[i];
            blocks[i] = '.';
            break;
          }
        }
      }
    }
    return blocks;
  }

  static long CalculateChecksum(List<object> blocks)
  {
    long checksum = 0; // Use long to support large integers
    for (int position = 0; position < blocks.Count; position++)
    {
      if (blocks[position] is int fileId) // Only consider file blocks
      {
        checksum += (long)position * fileId;
      }
    }
    return checksum;
  }

  public static long Main(string diskMap)
  {
    // Step 1: Parse the disk map
    var blocks = ParseDiskMap(diskMap);

    // Step 2: Compact the blocks
    var compactedBlocks = CompactBlocks(blocks);

    // Step 3: Calculate the checksum
    return CalculateChecksum(compactedBlocks);
  }

  public static void Main()
  {
    // Answer = 6200294120911
    // Example Input
    string diskMap = "2333133121414131402";

    string filePath = GetInputPath("input_day9.txt");
    diskMap = File.ReadAllText(filePath);

    // Run the solution
    long checksum = Main(diskMap);
    Console.WriteLine("Checksum: " + checksum);
  }

  static string GetInputPath(string fileName)
  {
    var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", fileName);
    return path;
  }
}

