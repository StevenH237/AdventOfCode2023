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

  // public static string Part2(string fname, StreamReader input)
  //   => input.GetLines()
  //     .Select(l =>
  //     {
  //       string[] parts = l.Split(" ");
  //       string left = parts[0];
  //       string right = parts[1];
  //       return new D12Line($"{left}?{left}?{left}?{left}?{left} {right},{right},{right},{right},{right}");
  //     }).Sum(l => l.CountValidArrangements())
  //     .ToString();
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

  public int CountValidArrangements()
    => GetValidArrangements().Count();

  public IEnumerable<string> GetValidArrangements()
    => GetArrangements(Pattern, Report);

  public static IEnumerable<string> GetArrangements(IEnumerable<int> pattern, string report)
  {
    if (pattern.Count() == 0)
    {
      yield break;
    }

    int extraSpace = report.Length - (pattern.Sum() + pattern.Count() - 1);
    if (extraSpace < 0) yield break;

    int next = pattern.First();

    int p = 0;

    while (p <= extraSpace)
    {
      IEnumerable<char> pReport = report.Skip(p);
      IEnumerable<char> damaged = pReport.Take(next);
      IEnumerable<char> skipped = report.Take(p);

      if (skipped.Contains('D')) break;

      if (damaged.Contains('w'))
      {
        p += pReport.WithIndex().Where(x => x.Item == 'w').First().Index + 1;
        continue;
      }

      if (pattern.Count() == 1)
      {
        yield return new string('w', p)
          + new string('D', next)
          + new string('w', extraSpace - p);
      }
      else
      {
        char afterGroup = report[p + next];
        if (afterGroup == 'D')
        {
          p += 1;
          continue;
        }

        string reportLeft = report[(p + next + 1)..^0];
        string prefix = new string('w', p)
          + new string('D', next)
          + "w";

        foreach (string suffix in GetArrangements(pattern.Skip(1), reportLeft))
        {
          yield return prefix + suffix;
        }
      }

      p += 1;
    }
  }
}

// public static IEnumerable<string> GetArrangements(IEnumerable<int> pattern, string report)
// {
//   if (pattern.Count() == 0)
//   {
//     yield break;
//   }

//   int extraSpace = report.Length - (pattern.Sum() + pattern.Count() - 1);
//   if (extraSpace < 0) yield break;

//   Regex patternRegex = new(string.Join("[w_]+", pattern.Select(c => "[D_]" + (c >= 2 ? $"{{{c}}}" : ""))));

//   int p = 0;
//   while (p <= extraSpace && !report[0..p].Contains('D'))
//   {
//     Match mtc = patternRegex.Match(report, p);

//     if (mtc.Success)
//     {
//       p = mtc.Index;

//       if (pattern.Count() == 1)
//         yield return new string('w', p)
//           + new string('D', pattern.First())
//           + new string('w', extraSpace - p);
//       else if (extraSpace - p == 0)
//       {
//         yield return String.Join('w', pattern.Select(c => new string('D', c)));
//       }
//       else
//       {
//         string reportLeft = report[(p + pattern.First() + 1)..^0];
//         string prefix = new string('w', p)
//           + new string('D', pattern.First())
//           + "w";

//         foreach (string suffix in GetArrangements(pattern.Skip(1), reportLeft))
//         {
//           yield return prefix + suffix;
//         }
//       }
//     }
//     else break;

//     p += 1;
//   }
// }

// public static IEnumerable<string> GetArrangements(IEnumerable<int> pattern, int length)
// {
//   if (pattern.Count() == 0)
//   {
//     yield return new string('w', length);
//     yield break;
//   }

//   int extraSpace = length - (pattern.Sum() + pattern.Count() - 1);
//   if (extraSpace < 0) throw new InvalidDataException("Not enough space!");

//   if (pattern.Count() == 1)
//   {
//     foreach (int i in Enumerable.Range(0, extraSpace + 1))
//     {
//       yield return
//         new string('w', i)
//         + new string('D', pattern.First())
//         + new string('w', extraSpace - i);
//     }

//     yield break;
//   }

//   foreach (int i in Enumerable.Range(0, extraSpace + 1))
//   {
//     string prefix = new string('w', i) + new string('D', pattern.First()) + "w";

//     foreach (string s in GetArrangements(pattern.Skip(1), length - (i + pattern.First() + 1)))
//     {
//       yield return prefix + s;
//     }
//   }
// }