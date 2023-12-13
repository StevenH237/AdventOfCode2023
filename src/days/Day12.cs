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
  public int Length;
  public Regex Report;

  public D12Line(string line)
  {
    InputLine = line;
    string[] parts = InputLine.Split(" ");

    Pattern = parts[1].Split(",").Select(int.Parse).ToList();
    Length = parts[0].Length;

    string reportPattern = "^";

    foreach (char[] chunk in parts[0]
        .ChunkWhile((p, c) => (p == c), skipEmpty: true, prependFails: true)
        .Select(x => x.ToArray()))
    {
      char chunkChar = chunk[0] switch
      {
        '.' => 'w',
        '#' => 'D',
        '?' => '.',
        _ => '\0'
      };
      if (chunk.Length < 4) reportPattern += Enumerable.Repeat(chunkChar, chunk.Length).FormString();
      else reportPattern += $"{chunkChar}{{{chunk.Length}}}";
    }

    reportPattern += "$";

    Report = new Regex(reportPattern);
  }

  public int CountValidArrangements()
    => GetValidArrangements().Count();

  public IEnumerable<string> GetValidArrangements()
    => GetAllArrangements()
      .Where(arr => Report.IsMatch(arr));

  public IEnumerable<string> GetAllArrangements()
    => GetArrangements(Pattern, Length);

  public static IEnumerable<string> GetArrangements(IEnumerable<int> pattern, int length)
  {
    if (pattern.Count() == 0)
    {
      yield return new string('w', length);
      yield break;
    }

    int extraSpace = length - (pattern.Sum() + pattern.Count() - 1);
    if (extraSpace < 0) throw new InvalidDataException("Not enough space!");

    if (pattern.Count() == 1)
    {
      foreach (int i in Enumerable.Range(0, extraSpace + 1))
      {
        yield return
          new string('w', i)
          + new string('D', pattern.First())
          + new string('w', extraSpace - i);
      }

      yield break;
    }

    foreach (int i in Enumerable.Range(0, extraSpace + 1))
    {
      string prefix = new string('w', i) + new string('D', pattern.First()) + "w";

      foreach (string s in GetArrangements(pattern.Skip(1), length - (i + pattern.First() + 1)))
      {
        yield return prefix + s;
      }
    }
  }
}