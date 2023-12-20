using System.Text.RegularExpressions;
using Nixill.Collections;
using Nixill.Utils;

public class Day19
{
  Dictionary<string, List<D19Part>> Buckets = new();
  Dictionary<string, D19Workflow> Workflows = new();
  HashSet<string> PendingWorkflows = new();

  public Day19(string fname, StreamReader input)
  {
    // First, add "A" and "R" buckets
    Buckets["A"] = new();
    Buckets["R"] = new();

    // First chunk is workflows
    foreach (string line in input.GetLinesOfChunk())
    {
      D19Workflow flow = new(line);
      Workflows[flow.ID] = flow;
      Buckets[flow.ID] = new();
    }

    // Second chunk is parts
    foreach (string line in input.GetLinesOfChunk())
    {
      D19Part part = new(line);
      Buckets["in"].Add(part);
    }

    PendingWorkflows.Add("in");
  }

  public void ProcessParts()
  {
    while (PendingWorkflows.Count > 0)
    {
      string flowID = PendingWorkflows.First();
      D19Workflow flow = Workflows[flowID];
      List<D19Part> bucket = Buckets[flowID];

      PendingWorkflows.Remove(flowID);

      while (bucket.Count > 0)
      {
        D19Part part = bucket.Pop();
        string movingTo = flow.SortPart(part);
        Buckets[movingTo].Add(part);
        if (movingTo != "A" && movingTo != "R") PendingWorkflows.Add(movingTo);
      }
    }
  }

  static Dictionary<string, Day19> results = new();

  static Day19 Get(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
      results[fname] = new Day19(fname, input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input)
  {
    Day19 result = Get(fname, input);
    result.ProcessParts();
    return result.Buckets["A"].Sum(p => p.Sum).ToString();
  }
}

public class D19Workflow
{
  public readonly string ID;
  List<D19Rule> Rules;
  public readonly string FinalMove;

  public IEnumerable<D19Rule> GetRules() => Rules.AsReadOnly();

  static readonly Regex workflowParser = new(@"^([a-z]+)\{((?:[xmas][\<\>]\d+:(?:[a-z]+|A|R),)+)([a-z]+|A|R)\}");

  public D19Workflow(string input)
  {
    Match mtc = workflowParser.Match(input);
    ID = mtc.Groups[1].Value;
    Rules = mtc.Groups[2].Value.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(s => new D19Rule(s)).ToList();
    FinalMove = mtc.Groups[3].Value;
  }

  public D19Workflow(string id, IEnumerable<D19Rule> rules, string finalMove)
  {
    ID = id;
    Rules = new(rules);
    FinalMove = finalMove;
  }

  public string SortPart(D19Part input)
  {
    foreach (D19Rule rule in Rules)
    {
      if (rule.MeetsCondition(input)) return rule.MoveTo;
    }

    return FinalMove;
  }
}

public struct D19Rule
{
  public char TestedAttribute;
  // To be clear, this means "check that the attribute's value is greater
  // than the required value", and false means less.
  public bool CheckGreater;
  public int RequiredValue;
  public string MoveTo;

  static readonly Regex ruleParser = new(@"^([xmas])([\<\>])(\d+):([a-z]+|A|R)$");

  public D19Rule(string input)
  {
    Match mtc = ruleParser.Match(input);
    TestedAttribute = mtc.Groups[1].Value[0];
    CheckGreater = (mtc.Groups[2].Value) == ">";
    RequiredValue = int.Parse(mtc.Groups[3].Value);
    MoveTo = mtc.Groups[4].Value;
  }

  public D19Rule(char test, bool greater, int value, string move)
  {
    TestedAttribute = test;
    CheckGreater = greater;
    RequiredValue = value;
    MoveTo = move;
  }

  public bool MeetsCondition(D19Part test)
  {
    if (CheckGreater) return test[TestedAttribute] > RequiredValue;
    else return test[TestedAttribute] < RequiredValue;
  }
}

public struct D19Part
{
  public int X;
  public int M;
  public int A;
  public int S;

  public int Sum => X + M + A + S;

  public int this[char test] => test switch
  {
    'x' or 'X' => X,
    'm' or 'M' => M,
    'a' or 'A' => A,
    's' or 'S' => S,
    _ => 0
  };

  static readonly Regex partParser = new(@"^\{x=(\d+),m=(\d+),a=(\d+),s=(\d+)\}$");

  public D19Part(string input)
  {
    if (partParser.TryMatch(input, out Match mtc))
    {
      X = int.Parse(mtc.Groups[1].Value);
      M = int.Parse(mtc.Groups[2].Value);
      A = int.Parse(mtc.Groups[3].Value);
      S = int.Parse(mtc.Groups[4].Value);
    }
  }

  public D19Part(int x, int m, int a, int s)
  {
    X = x;
    M = m;
    A = a;
    S = s;
  }
}