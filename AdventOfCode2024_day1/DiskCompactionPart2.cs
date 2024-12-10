using System;
using System.Collections.Generic;
using System.IO;

class DiskCompactionPart2
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

      // Increment file ID for the next file (only if a file was added)
      if (fileSize > 0) fileId++;

      // Add free space blocks
      for (int j = 0; j < freeSize; j++)
      {
        blocks.Add('.');
      }
    }
    return blocks;
  }

  // New method to find and move entire files
  static List<object> CompactBlocks(List<object> blocks)
  {
    // Identify files and their ranges
    // fileId -> (startIndex, length)
    // We assume files are contiguous.
    var fileRanges = new Dictionary<int, (int start, int length)>();

    // We'll scan for contiguous runs of integers and treat each distinct integer run as a file.
    // Since files were assigned IDs in sequence during parsing, we can trust that each file
    // appears contiguously in the initial state.
    int currentId = -1;
    int currentStart = -1;
    int currentLength = 0;
    for (int i = 0; i < blocks.Count; i++)
    {
      if (blocks[i] is int fileId)
      {
        if (fileId != currentId)
        {
          // If we were tracking a previous file run, record it
          if (currentId != -1)
          {
            fileRanges[currentId] = (currentStart, currentLength);
          }
          // Start a new file run
          currentId = fileId;
          currentStart = i;
          currentLength = 1;
        }
        else
        {
          currentLength++;
        }
      }
      else
      {
        // If we reach a non-file block and we were tracking a file, end that run
        if (currentId != -1)
        {
          fileRanges[currentId] = (currentStart, currentLength);
          currentId = -1;
          currentLength = 0;
        }
      }
    }

    // If ended with a file run not recorded yet
    if (currentId != -1 && currentLength > 0)
    {
      fileRanges[currentId] = (currentStart, currentLength);
    }

    // Get all file IDs in descending order
    var fileIds = new List<int>(fileRanges.Keys);
    fileIds.Sort((a, b) => b.CompareTo(a)); // descending

    // For each file in descending order, attempt to move it
    foreach (var fid in fileIds)
    {
      var (startIndex, length) = fileRanges[fid];

      // Find a run of '.' that can fit this file to the left of startIndex
      // The run must end before startIndex (i.e. runStart + length <= startIndex)
      // We'll find all '.' runs and pick the leftmost that can fit the file.
      (int runStart, int runLength)? chosenRun = null;

      int dotStart = -1;
      int dotLength = 0;

      for (int i = 0; i < startIndex; i++)
      {
        if (blocks[i] is char c && c == '.')
        {
          if (dotStart == -1)
          {
            dotStart = i;
            dotLength = 1;
          }
          else
          {
            dotLength++;
          }
        }
        else
        {
          // If we ended a '.' run, check if it fits the file
          if (dotStart != -1)
          {
            if (dotLength >= length)
            {
              // This run can fit the file. Check if it's the leftmost so far
              // Since we scan left to right, the first we find that fits is the leftmost.
              chosenRun = (dotStart, dotLength);
              break;
            }
            // Reset dot run
            dotStart = -1;
            dotLength = 0;
          }
        }
      }

      // Check at the end if we ended with a '.' run
      if (chosenRun == null && dotStart != -1 && dotLength >= length)
      {
        chosenRun = (dotStart, dotLength);
      }

      // If no suitable run found, do not move the file
      if (chosenRun == null) continue;

      // Move the file:
      var (rStart, rLen) = chosenRun.Value;

      // Overwrite the '.' run portion (just the first 'length' blocks of the run) with the file blocks
      for (int i = 0; i < length; i++)
      {
        blocks[rStart + i] = fid;
      }

      // Replace the old file location with '.'
      for (int i = 0; i < length; i++)
      {
        blocks[startIndex + i] = '.';
      }

      // Update the file's new location in the fileRanges dictionary (not strictly necessary for final checksum)
      fileRanges[fid] = (rStart, length);
    }

    return blocks;
  }

  static long CalculateChecksum(List<object> blocks)
  {
    long checksum = 0;
    for (int position = 0; position < blocks.Count; position++)
    {
      if (blocks[position] is int fileId)
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

    // Step 2: Compact the blocks with the new whole-file moving approach
    var compactedBlocks = CompactBlocks(blocks);

    // Step 3: Calculate the checksum
    return CalculateChecksum(compactedBlocks);
  }

  public static void Main()
  {
    //Answer: 6227018762750
    // Example given in the puzzle would result in a checksum of 2858 for that scenario.
    // We'll run with the given input file now.

    string filePath = GetInputPath("input_day9.txt");
    string diskMap = File.ReadAllText(filePath);
    //diskMap = "2333133121414131402";

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
