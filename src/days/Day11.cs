using System.Collections.Generic;
using Nixill.Collections.Grid;

public class Day11
{
  Grid<char> OriginalSpace;
  Grid<char> ExpandedSpace;

  int TotalDistance = 0;

  public Day11(string fname, StreamReader input)
  {
    // Create the space
    OriginalSpace = new(input.GetLines().Cast<IEnumerable<char>>());
    ExpandedSpace = new(OriginalSpace);

    // Expand it
    for (int i = 0; i < ExpandedSpace.Height; i++)
    {
      IList<char> row = ExpandedSpace.GetRow(i);
      if (!row.Contains('#'))
      {
        ExpandedSpace.InsertRow(i++, '.');
      }
    }

    for (int i = 0; i < ExpandedSpace.Width; i++)
    {
      IList<char> col = ExpandedSpace.GetColumn(i);
      if (!col.Contains('#'))
      {
        ExpandedSpace.InsertColumn(i++, '.');
      }
    }

    // Now find all asteroids
    List<(int Row, int Column)> asteroids = new();

    for (int r = 0; r < ExpandedSpace.Height; r++)
    {
      for (int c = 0; c < ExpandedSpace.Width; c++)
      {
        if (ExpandedSpace[r, c] == '#')
          asteroids.Add((r, c));
      }
    }

    // Get their distances
    foreach ((int R1, int C1, int index) in asteroids.Select((x, i) => (x.Row, x.Column, i)))
    {
      foreach ((int R2, int C2) in asteroids.Skip(index + 1))
      {
        TotalDistance += Math.Abs(R2 - R1) + Math.Abs(C2 - C1);
      }
    }
  }

  // --------- Static stuff can sit at the bottom of the page --------- //
  static Dictionary<string, Day11> results = new();

  static Day11 Get(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
      results[fname] = new Day11(fname, input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input)
  {
    Day11 result = Get(fname, input);
    return result.TotalDistance.ToString();
  }
}