using System.Text.RegularExpressions;
using Nixill.Utils;

// PLEASE NOTE: My interpretation of the task changes working springs to
// w and damaged springs to D. This change is done at runtime, not by
// editing raw input, but this is why you see those characters instead of
// . and # everywhere.

public class Day12
{
  public static string Part1(string fname, StreamReader input)
    => input.GetLines()
      .Select(l => new D12Line(l))
      .Sum(l => l.CountValidArrangements())
      .ToString();

  public static string Part2(string fname, StreamReader input)
    => input.GetLines()
      .Select(l =>
      {
        string[] parts = l.Split(" ");
        string left = parts[0];
        string right = parts[1];
        return new D12Line($"{left}?{left}?{left}?{left}?{left} {right},{right},{right},{right},{right}");
      }).Sum(l => l.CountValidArrangements())
      .ToString();
}

public class D12Line
{
  public string InputLine;
  public List<int> Pattern;
  public string Report;

  public D12Line(string line)
  {
    InputLine = line;
    string[] parts = InputLine.Split(" ");

    Pattern = parts[1].Split(",").Select(int.Parse).ToList();

    Report = parts[0].Select(x => x switch
    {
      '.' => 'w',
      '#' => 'D',
      '?' => '_',
      _ => '\0'
    }).FormString();
  }

  public long CountValidArrangements()
    => Solve(Report, Pattern);

  // The below is a Python-to-C# translation of DragonGodGrapha's solving
  // function, which can be seen here:
  // https://github.com/DragonGodGrapha/AoCRedux/tree/main/2023/12.py
  //
  // That was itself a modification of code generated from mebeim's
  // walkthrough, which is visible here:
  // https://github.com/mebeim/aoc/tree/master/2023#day-12---hot-springs
  //
  // This code copying is why this solution lives on an alternate branch.
  // I still want to try to solve this myself one day, possibly learning
  // from the below or possibly having my own ideas again.
  //
  // Also, there isn't a built-in cache here, so I had to implement that
  // on my own. It worked nicely though.
  public static Dictionary<string, long> Cache = new();

  public static long Solve(string report, IEnumerable<int> pattern, int currentMatchLength = 0)
  {
    string key = null;
    if (currentMatchLength == 0)
    {
      key = report + " " + string.Join(",", pattern);
      if (Cache.ContainsKey(key)) return Cache[key];
    }
    if (pattern.Count() == 0) // No more pattern to match
    {
      if (report.Contains('D')) return 0; // Can't match, automatically fails
      else return 1; // Matches with all _ → w, only 1 way
    }

    if (currentMatchLength > pattern.First()) return 0; // Too many D in current length

    if (report == "") // Empty string; gone through all characters
    {
      if (pattern.Count() == 1 && currentMatchLength == pattern.First()) return 1; // On last group and length matches
      return 0; // Too many pattern left or last group isn't matched yet
    }

    char c = report[0]; // First character of report
    long total = 0; // Number of possible arrangements

    if (c != 'w') // If first character is D (or _), same group continues
      total += Solve(report[1..], pattern, currentMatchLength + 1);

    if (c != 'D') // If first character is w (or _), move to next group
      if (currentMatchLength == 0) // We haven't started next group, so don't remove it yet
        total += Solve(report[1..], pattern, 0);
      else if (currentMatchLength == pattern.First()) // We just finished the group, so move to next group if it matches
        total += Solve(report[1..], pattern.Skip(1), 0);

    if (key != null) Cache[key] = total;
    return total;
  }
}