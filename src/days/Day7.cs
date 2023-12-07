public static class Day7
{
  static Dictionary<string, Day7Result> results = new();

  public static string Part1(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day7Result(input);
    }

    return results[fname].Part1Result;
  }

  public static string Part2(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day7Result(input);
    }

    return results[fname].Part2Result;
  }
}

public class Day7Result
{
  public readonly string Part1Result;
  public readonly string Part2Result;

  public Day7Result(StreamReader input)
  {

  }
}

public class D7Hand
{
  readonly string Cards;
  readonly string Rank;

  public D7Hand(string cards)
  {
    Cards = cards;
  }
}
