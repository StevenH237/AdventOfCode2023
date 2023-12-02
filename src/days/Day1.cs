using System.Text.RegularExpressions;
using Nixill.Utils;

public static class Day1
{
  static Regex firstDigitRegex = new(@"^\D*(\d)");
  static Regex lastDigitRegex = new(@"(\d)\D*$");

  static Regex digits = new(@"(on(?=e)|tw(?=o)|thre(?=e)|fou(?=r)|fiv(?=e)|si(?=x)|seve(?=n)|eigh(?=t)|nin(?=e)|\d)");

  static Dictionary<string, int> mappings = new()
  {
    ["on"] = 1,
    ["1"] = 1,
    ["tw"] = 2,
    ["2"] = 2,
    ["thre"] = 3,
    ["3"] = 3,
    ["fou"] = 4,
    ["4"] = 4,
    ["fiv"] = 5,
    ["5"] = 5,
    ["si"] = 6,
    ["6"] = 6,
    ["seve"] = 7,
    ["7"] = 7,
    ["eigh"] = 8,
    ["8"] = 8,
    ["nin"] = 9,
    ["9"] = 9,
    ["0"] = 0
  };

  public static string Part1(StreamReader input)
  {
    int sum = 0;

    foreach (string line in input.GetLines())
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

  public static string Part2(StreamReader input)
  {
    int sum = 0;

    foreach (string line in input.GetLines())
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