using System.Text.RegularExpressions;

public static class Day8
{
  static Regex nodeFormat = new(@"([A-Z0-9]{3}) = \(([A-Z0-9]{3}), ([A-Z0-9]{3})\)");

  public static string Part1(string fname, StreamReader input)
  {
    Dictionary<string, D8Node> nodes = new();
    string steps = input.ReadLine();
    input.ReadLine();

    foreach (string line in input.GetLines())
    {
      Match mtc = nodeFormat.Match(line);
      string nodeID = mtc.Groups[1].Value;
      string nodeLN = mtc.Groups[2].Value;
      string nodeRN = mtc.Groups[3].Value;
      nodes[nodeID] = new D8Node() { Left = nodeLN, Right = nodeRN };
    }

    int moves = 0;
    D8Node node = nodes["AAA"];

    foreach (char move in steps.RepeatInfinite())
    {
      string moveTo = null;
      if (move == 'L') moveTo = node.Left;
      else moveTo = node.Right;
      moves += 1;
      if (moveTo == "ZZZ") return moves.ToString();
      else node = nodes[moveTo];
    }

    throw new InvalidDataException("How did you reach this error?");
  }

  public static string Part2(string fname, StreamReader input)
  {
    Dictionary<string, D8Node> nodes = new();
    string steps = input.ReadLine();
    input.ReadLine();

    List<string> currentNodes = new();

    foreach (string line in input.GetLines())
    {
      Match mtc = nodeFormat.Match(line);
      string nodeID = mtc.Groups[1].Value;
      string nodeLN = mtc.Groups[2].Value;
      string nodeRN = mtc.Groups[3].Value;
      nodes[nodeID] = new D8Node() { Left = nodeLN, Right = nodeRN };
      if (nodeID.EndsWith("A")) currentNodes.Add(nodeID);
    }

    int haunts = currentNodes.Count;

    // Okay so this is gonna be a complex one huh?
    // I got some help from DragonGodGrapha. This code makes a couple
    // assumptions that aren't necessarily true on *any*
    // eventually-solvable input, but are true of the example input and
    // his real input — hopefully mine too.
    List<long> hauntLengths = new();

    foreach (string haunt in currentNodes)
    {
      hauntLengths.Add(GetHauntSteps(steps, nodes[haunt], nodes));
    }

    return hauntLengths.AggregateFromFirst(LCM).ToString();
  }

  static int GetHauntSteps(string steps, D8Node node, Dictionary<string, D8Node> nodes)
  {
    int moves = 0;

    foreach (char move in steps.RepeatInfinite())
    {
      string moveTo = null;
      if (move == 'L') moveTo = node.Left;
      else moveTo = node.Right;
      moves += 1;
      if (moveTo.EndsWith("Z")) return moves;
      else node = nodes[moveTo];
    }

    throw new InvalidDataException("How did you reach this error?");
  }

  static long GCD(long a, long b)
  {
    // Turn negative numbers positive; since any negative integer can be expressed as -1 times a positive integer, the GCD of two negative numbers (or a negative and a positive) will be the same as GCD(|a|, |b|) anyway
    if (a < 0) a = -a;
    if (b < 0) b = -b;
    // Also, a should be the larger number.
    if (b > a) (a, b) = (b, a);
    while (b != 0)
      (a, b) = (b, a % b);
    return a;
  }

  static long LCM(long a, long b) => a / GCD(a, b) * b;
}

public struct D8Node
{
  public string Left;
  public string Right;
}

public static class MoreExtensions
{
  public static IEnumerable<T> Repeat<T>(this IEnumerable<T> seq, int count)
  {
    foreach (int i in Enumerable.Range(0, count))
    {
      foreach (T item in seq)
      {
        yield return item;
      }
    }
  }

  public static IEnumerable<T> RepeatInfinite<T>(this IEnumerable<T> seq)
  {
    while (true)
    {
      foreach (T item in seq)
      {
        yield return item;
      }
    }
  }

  public static T Pop<T>(this IList<T> list)
  {
    T val = list[0];
    list.RemoveAt(0);
    return val;
  }

  public static T AggregateFromFirst<T>(this IEnumerable<T> elems, Func<T, T, T> aggregation)
  {
    bool assigned = false;
    T aggregate = default(T);

    foreach (T item in elems)
    {
      if (!assigned)
      {
        aggregate = item;
        assigned = true;
      }
      else
      {
        aggregate = aggregation(aggregate, item);
      }
    }

    if (!assigned) throw new InvalidOperationException("Sequence contains no elements.");
    return aggregate;
  }
}