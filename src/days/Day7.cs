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
  List<D7Hand> Hands = new();
  List<D7JackerHand> HandsP2 = new();

  public readonly string Part1Result;
  public readonly string Part2Result;

  public Day7Result(StreamReader input)
  {
    foreach (string line in input.GetLines())
    {
      Hands.Add(new D7Hand(line));
      HandsP2.Add(new D7JackerHand(line));
    }

    var HandsOrdered = Hands.Order();
    Part1Result = Hands.Order().Select((x, i) => (i + 1) * x.Wager).Sum().ToString();

    var HandsP2Ordered = HandsP2.Order();
    Part2Result = HandsP2Ordered.Select((x, i) => (i + 1) * x.Wager).Sum().ToString();
  }
}

public class D7Hand : IComparable<D7Hand>
{
  public readonly string Cards;
  public readonly string Rank;
  public readonly int Wager;

  public D7Hand(string line)
  {
    string[] split = line.Split(" ");
    Cards = split[0];
    Wager = int.Parse(split[1]);
    Rank = string.Join("", Cards
      .GroupBy(x => x)
      .Select(x => x.Count())
      .OrderDescending()
      .Select(x => x.ToString()));
  }

  public int CompareTo(D7Hand other)
  {
    int rankComp = CompareRank(other);
    if (rankComp != 0) return rankComp;
    return CompareCards(other);
  }

  public int CompareRank(D7Hand other)
  {
    return Rank.CompareTo(other.Rank);
  }

  static Dictionary<char, int> Values = new()
  {
    ['A'] = 14,
    ['K'] = 13,
    ['Q'] = 12,
    ['J'] = 11,
    ['T'] = 10,
    ['9'] = 9,
    ['8'] = 8,
    ['7'] = 7,
    ['6'] = 6,
    ['5'] = 5,
    ['4'] = 4,
    ['3'] = 3,
    ['2'] = 2
  };

  public int CompareCards(D7Hand other)
  {
    return Cards
      .Zip(other.Cards)
      .Select(x => (Values[x.First] - Values[x.Second]))
      .Where(x => x != 0)
      .FirstOrDefault(0);
  }
}

public class D7JackerHand : IComparable<D7JackerHand>
{
  public readonly string Cards;
  public readonly string Rank;
  public readonly int Wager;

  public D7JackerHand(string line)
  {
    string[] split = line.Split(" ");
    Cards = split[0];
    Wager = int.Parse(split[1]);

    string cardsWithoutJacks = Cards.Replace("J", "");
    int countJacks = 5 - cardsWithoutJacks.Length;

    int[] rankNumbers = cardsWithoutJacks
      .GroupBy(x => x)
      .Select(x => x.Count())
      .OrderDescending()
      .ToArray();

    // Handle an all-Jack hand
    if (rankNumbers.Length == 0)
    {
      rankNumbers = new[] { 0 };
    }

    rankNumbers[0] += countJacks;

    Rank = string.Join("", rankNumbers);
  }

  public int CompareTo(D7JackerHand other)
  {
    int rankComp = CompareRank(other);
    if (rankComp != 0) return rankComp;
    return CompareCards(other);
  }

  public int CompareRank(D7JackerHand other)
  {
    return Rank.CompareTo(other.Rank);
  }

  static Dictionary<char, int> Values = new()
  {
    ['A'] = 14,
    ['K'] = 13,
    ['Q'] = 12,
    ['T'] = 10,
    ['9'] = 9,
    ['8'] = 8,
    ['7'] = 7,
    ['6'] = 6,
    ['5'] = 5,
    ['4'] = 4,
    ['3'] = 3,
    ['2'] = 2,
    ['J'] = 1
  };

  public int CompareCards(D7JackerHand other)
  {
    return Cards
      .Zip(other.Cards)
      .Select(x => (Values[x.First] - Values[x.Second]))
      .Where(x => x != 0)
      .FirstOrDefault(0);
  }
}
