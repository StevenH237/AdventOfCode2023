using System.Text.RegularExpressions;
using Nixill.Utils;

public static class Day1Main
{
  static Regex firstDigitRegex = new(@"^\D*(\d)");
  static Regex lastDigitRegex = new(@"(\d)\D*$");

  static Regex digits = new(@"(one|two|three|four|five|six|seven|eight|nine|\d)");

  static Dictionary<string, int> mappings = new() {
    ["one"] = 1, ["1"] = 1,
    ["two"] = 2, ["2"] = 2,
    ["three"] = 3, ["3"] = 3,
    ["four"] = 4, ["4"] = 4,
    ["five"] = 5, ["5"] = 5,
    ["six"] = 6, ["6"] = 6,
    ["seven"] = 7, ["7"] = 7,
    ["eight"] = 8, ["8"] = 8,
    ["nine"] = 9, ["9"] = 9,
    ["0"] = 0
  };

  const string Filename = "data/day1" +
    // "example2" +
    ".txt";

  public static string Part1()
  {
    var file = File.ReadAllLines(Filename);
    int sum = 0;

    foreach (string line in file)
    {
      Match mtc;
      int number = 0;

      if (firstDigitRegex.TryMatch(line, out mtc))
      {
        number = int.Parse(mtc.Groups[1].Value) * 10;
      }

      if (lastDigitRegex.TryMatch(line, out mtc))
      {
        number += int.Parse(mtc.Groups[1].Value);
      }

      sum += number;
    }

    return sum.ToString();
  }

  public static string Part2()
  {
    var file = File.ReadAllLines(Filename);
    int sum = 0;

    foreach (string line in file)
    {
      if (digits.TryMatch(line, out Match mtc))
      {
        int number = mappings[mtc.Captures.First().Value] * 10;

        for (Match nmtc = mtc.NextMatch(); nmtc.Success; nmtc = mtc.NextMatch())
        {
          mtc = nmtc;
        }

        number += mappings[mtc.Captures.First().Value];

        sum += number;
      }
    }

    return sum.ToString();
  }
}