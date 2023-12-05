using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nixill.Collections;
using Nixill.Utils;

public static class Day4
{
  static Dictionary<string, Day4Result> results = new();

  public static string Part1(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day4Result(input);
    }

    return results[fname].Part1Result;
  }

  public static string Part2(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day4Result(input);
    }

    return results[fname].Part2Result;
  }
}

public class Day4Result
{
  Regex rgxCardNum = new(@"^Card +(\d+): (.+)$");

  public readonly string Part1Result;
  public readonly string Part2Result;

  List<D4Card> cards = new();

  public Day4Result(StreamReader input)
  {
    foreach (string line in input.GetLines())
    {
      Match mtcCardNum = rgxCardNum.Match(line);

      int id = int.Parse(mtcCardNum.Groups[1].Value);
      string cardText = mtcCardNum.Groups[2].Value;

      string[] cardSplit = cardText.Split(" | ");
      int[] winners = cardSplit[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
      int[] lots = cardSplit[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();

      cards.Add(new()
      {
        id = id,
        winners = winners,
        lots = lots
      });
    }

    int scoreSum = 0;

    foreach ((D4Card card, int i) in cards.Select((x, i) => (x, i)))
    {
      scoreSum += card.Score();

      foreach ((D4Card card2, int i2) in cards.Select((x, i) => (x, i)).Skip(i + 1).Take(card.WinCount))
      {
        card2.instances += card.instances;
      }
    }

    Part1Result = scoreSum.ToString();
    Part2Result = cards.Select(x => x.instances).Sum().ToString();
  }
}

public class D4Card
{
  public int id;
  public int[] winners;
  public int[] lots;
  public int instances = 1;

  public int Score()
  {
    var intersect = winners.Intersect(lots).ToArray();
    if (intersect.Count() == 0) return 0;
    return 1 << (intersect.Count() - 1);
  }

  public int WinCount => winners.Intersect(lots).Count();
}
