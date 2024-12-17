using System;

public class D13P2V1
{
  public static void Main()
  {
    // Given machine configurations (A: xA, yA; B: xB, yB; Prize: xP, yP)
    var machines = new (int xA, int yA, int xB, int yB, int xP, int yP)[]
    {
            (94, 34, 22, 67, 8400, 5400),
            (26, 66, 67, 21, 12748, 12176),
            (17, 86, 84, 37, 7870, 6450),
            (69, 23, 27, 71, 18641, 10279)
    };

    // We are told to consider pressing buttons no more than 100 times each.
    int max_presses = 100;

    // Costs
    int cost_A = 3;
    int cost_B = 1;

    var results = new System.Collections.Generic.List<int>();

    foreach (var machine in machines)
    {
      int xA = machine.xA;
      int yA = machine.yA;
      int xB = machine.xB;
      int yB = machine.yB;
      int xP = machine.xP;
      int yP = machine.yP;

      int? min_cost = null;

      // Try all combinations of A and B presses from 0 to 100
      for (int a_count = 0; a_count <= max_presses; a_count++)
      {
        for (int b_count = 0; b_count <= max_presses; b_count++)
        {
          // Check if this combination reaches the prize
          if ((a_count * xA + b_count * xB == xP) &&
              (a_count * yA + b_count * yB == yP))
          {
            int current_cost = a_count * cost_A + b_count * cost_B;
            if (min_cost == null || current_cost < min_cost.Value)
            {
              min_cost = current_cost;
            }
          }
        }
      }

      // Store the minimal cost for this machine (if any)
      if (min_cost.HasValue)
      {
        results.Add(min_cost.Value);
      }
    }

    results.Sort();
    int max_prizes = results.Count;
    int min_total_cost = 0;
    foreach (var r in results)
    {
      min_total_cost += r;
    }

    Console.WriteLine("Max prizes that can be won: " + max_prizes);
    Console.WriteLine("Minimum total cost for all solvable prizes: " + min_total_cost);
  }
}
