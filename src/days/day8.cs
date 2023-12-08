using System.Text.RegularExpressions;

public static class Day8
{
  static Regex nodeFormat = new(@"([A-Z]{3}) = \(([A-Z]{3}), ([A-Z]{3})\)";
  
  public static string Part1(string fname, StreamReader input)
  {
    Dictionary<string, D8Node> nodes;
    string steps = input.ReadLine();
    input.ReadLine();

    foreach (string line in input.GetLines())
    {
      Match mtc = nodeFormat.Match(line);
      string nodeID = mtc.Groups[1].Value;
      string nodeLN = mtc.Groups[2].Value;
      string nodeRN = mtc.Groups[3].Value;
      nodes[nodeID] = new D8Node() { Left = nodeLN, right = nodeRN };
    }
    
    int moves = 0;
    D8Node startNode = nodes["AAA"];
    
    foreach (char move in steps.RepeatInfinite())
    {
      string moveTo = null;
      if (move == 'L') moveTo = node.Left;
      else moveTo = node.Right;
      moves += 1;
      if (moveTo == "ZZZ") return moves.ToString();
      else node = nodes[MoveTo];
    }
    
    throw new InvalidDataException("How did you reach this error?");
  }
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
}