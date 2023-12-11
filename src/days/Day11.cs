using System.Collections.Generic;
using Nixill.Collections.Grid;

public class Day11
{
  Grid<char> Space;
  List<int> ExpansionColumns = new();
  List<int> ExpansionRows = new();
  List<(int Row, int Column)> Asteroids = new();

  public Day11(string fname, StreamReader input)
  {
    // Create the space
    Space = new(input.GetLines().Cast<IEnumerable<char>>());

    // Expand it
    for (int i = 0; i < Space.Height; i++)
    {
      IList<char> row = Space.GetRow(i);
      if (!row.Contains('#'))
      {
        Space.RemoveRowAt(i);
        Space.InsertRow(i, '-');
        ExpansionRows.Add(i);
      }
    }

    for (int i = 0; i < Space.Width; i++)
    {
      IList<char> col = Space.GetColumn(i);
      if (!col.Contains('#'))
      {
        // | will overwrite - rather than combining into +, but I think
        // that's okay. The lines are purely an (unused) visual anyway.
        Space.RemoveColumnAt(i);
        Space.InsertColumn(i, '|');
        ExpansionColumns.Add(i);
      }
    }

    // Now find all asteroids
    for (int r = 0; r < Space.Height; r++)
    {
      for (int c = 0; c < Space.Width; c++)
      {
        if (Space[r, c] == '#')
          Asteroids.Add((r, c));
      }
    }
  }

  // Get their distances
  public long GetDistances(int expnFactor)
  {
    long answer = 0;

    foreach ((int R1, int C1, int index) in Asteroids.Select((x, i) => (x.Row, x.Column, i)))
    {
      foreach ((int R2, int C2) in Asteroids.Skip(index + 1))
      {
        answer += GetDistance(R1, C1, R2, C2, expnFactor);
      }
    }

    return answer;
  }

  // Get a single distance
  public long GetDistance(int R1, int C1, int R2, int C2, int expnFactor)
  {
    if (R1 > R2) (R1, R2) = (R2, R1);
    if (C1 > C2) (C1, C2) = (C2, C1);

    var expnX = ExpansionColumns.Where(x => x > C1 && x < C2).Count();
    var expnY = ExpansionRows.Where(y => y > R1 && y < R2).Count();

    var distX = C2 - C1 + expnX * expnFactor;
    var distY = R2 - R1 + expnY * expnFactor;

    return distX + distY;
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
    => Get(fname, input).GetDistances(1).ToString();

  public static string Part2(string fname, StreamReader input)
    => Get(fname, input).GetDistances(999999).ToString();
}