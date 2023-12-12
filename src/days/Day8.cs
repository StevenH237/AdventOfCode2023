using System.Text.RegularExpressions;
using Nixill.Utils;

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

    return hauntLengths.AggregateFromFirst(NumberUtils.LCM).ToString();
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
}

public struct D8Node
{
  public string Left;
  public string Right;
}
