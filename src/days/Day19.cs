using System.Text.RegularExpressions;
using Nixill.Collections;
using Nixill.Utils;

public class Day19
{
  public Dictionary<string, List<D19Part>> Buckets = new();
  public Dictionary<string, List<D19PartRange>> Ranges = new();
  public Dictionary<string, D19Workflow> Workflows = new();
  public HashSet<string> PendingWorkflows1 = new();
  public HashSet<string> PendingWorkflows2 = new();

  public Day19(string fname, StreamReader input)
  {
    // First, add "A" and "R" buckets
    Buckets["A"] = new();
    Buckets["R"] = new();
    Ranges["A"] = new();
    Ranges["R"] = new();

    // First chunk is workflows
    foreach (string line in input.GetLinesOfChunk())
    {
      D19Workflow flow = new(line);
      Workflows[flow.ID] = flow;
      Buckets[flow.ID] = new();
      Ranges[flow.ID] = new();
    }

    // Second chunk is parts
    foreach (string line in input.GetLinesOfChunk())
    {
      D19Part part = new(line);
      Buckets["in"].Add(part);
    }

    PendingWorkflows1.Add("in");

    // And lastly, part 2 stuff
    D19PartRange range = new()
    {
      XMin = 1,
      XMax = 4000,
      MMin = 1,
      MMax = 4000,
      AMin = 1,
      AMax = 4000,
      SMin = 1,
      SMax = 4000
    };

    Ranges["in"].Add(range);
    PendingWorkflows2.Add("in");
  }

  public void ProcessParts()
  {
    while (PendingWorkflows1.Count > 0)
    {
      string flowID = PendingWorkflows1.First();
      D19Workflow flow = Workflows[flowID];
      List<D19Part> bucket = Buckets[flowID];

      PendingWorkflows1.Remove(flowID);

      while (bucket.Count > 0)
      {
        D19Part part = bucket.Pop();
        string movingTo = flow.SortPart(part);
        Buckets[movingTo].Add(part);
        if (movingTo != "A" && movingTo != "R") PendingWorkflows1.Add(movingTo);
      }
    }
  }

  public void ProcessRanges()
  {
    while (PendingWorkflows2.Count > 0)
    {
      string flowID = PendingWorkflows2.First();
      D19Workflow flow = Workflows[flowID];
      List<D19PartRange> rangeList = Ranges[flowID];

      PendingWorkflows2.Remove(flowID);

      while (rangeList.Count > 0)
      {
        D19PartRange range = rangeList.Pop();
        foreach ((string movingTo, D19PartRange movingRange) in flow.SplitRange(range))
        {
          Ranges[movingTo].Add(movingRange);
          if (movingTo != "A" && movingTo != "R") PendingWorkflows2.Add(movingTo);
        }
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

  public static string Part2(string fname, StreamReader input)
  {
    Day19 result = Get(fname, input);
    result.ProcessRanges();
    return result.Ranges["A"].Sum(r => r.Count).ToString();
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

    while (Rules.Any() && Rules.Last().MoveTo == FinalMove)
    {
      Rules.RemoveAt(Rules.Count - 1);
    }
  }

  public D19Workflow(string id, IEnumerable<D19Rule> rules, string finalMove)
  {
    ID = id;
    Rules = new(rules);
    FinalMove = finalMove;

    while (Rules.Any() && Rules.Last().MoveTo == FinalMove)
    {
      Rules.RemoveAt(Rules.Count - 1);
    }
  }

  public string SortPart(D19Part input)
  {
    foreach (D19Rule rule in Rules)
    {
      if (rule.MeetsCondition(input)) return rule.MoveTo;
    }

    return FinalMove;
  }

  public IEnumerable<(string Target, D19PartRange Range)> SplitRange(D19PartRange range)
  {
    foreach (D19Rule rule in Rules)
    {
      (D19PartRange? keep, D19PartRange? move) = rule.SplitRange(range);
      if (move.HasValue) yield return (rule.MoveTo, move.Value);
      if (!keep.HasValue) yield break;
      range = keep.Value;
    }

    yield return (FinalMove, range);
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

  public (D19PartRange? Keep, D19PartRange? Move) SplitRange(D19PartRange range)
  {
    D19PartRange intRange = range;
    D19PartRange? keepRange = range;
    D19PartRange? moveRange = range;
    (int Min, int Max) = range[TestedAttribute];

    if (CheckGreater)
    {
      if (Min > RequiredValue)
      {
        keepRange = null;
      }
      else if (Max <= RequiredValue)
      {
        moveRange = null;
      }
      else
      {
        // this feels a little dumb but whatever
        intRange[TestedAttribute, true] = RequiredValue;
        keepRange = intRange;

        intRange = range;
        intRange[TestedAttribute, false] = RequiredValue + 1;
        moveRange = intRange;
      }
    }
    else
    {
      if (Max < RequiredValue)
      {
        keepRange = null;
      }
      else if (Min >= RequiredValue)
      {
        moveRange = null;
      }
      else
      {
        intRange[TestedAttribute, false] = RequiredValue;
        keepRange = intRange;

        intRange = range;
        intRange[TestedAttribute, true] = RequiredValue - 1;
        moveRange = intRange;
      }
    }

    return (keepRange, moveRange);
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

public struct D19PartRange
{
  public int XMin;
  public int XMax;
  public int MMin;
  public int MMax;
  public int AMin;
  public int AMax;
  public int SMin;
  public int SMax;

  public (int Min, int Max) this[char attr]
  {
    get => attr switch
    {
      'X' or 'x' => (XMin, XMax),
      'M' or 'm' => (MMin, MMax),
      'A' or 'a' => (AMin, AMax),
      'S' or 's' => (SMin, SMax),
      _ => (0, 4000)
    };

    set
    {
      if (attr == 'X' || attr == 'x') (XMin, XMax) = value;
      if (attr == 'M' || attr == 'm') (MMin, MMax) = value;
      if (attr == 'A' || attr == 'a') (AMin, AMax) = value;
      if (attr == 'S' || attr == 's') (SMin, SMax) = value;
    }
  }

  public int this[char attr, bool max]
  {
    get => max ? this[attr].Max : this[attr].Min;
    set
    {
      if (max) this[attr] = (this[attr, false], value);
      else this[attr] = (value, this[attr, true]);
    }
  }

  static long CountAttr((int Min, int Max) range) => range.Max - range.Min + 1;

  public long Count => CountAttr(this['X']) * CountAttr(this['M']) * CountAttr(this['A']) * CountAttr(this['S']);

  public override string ToString() => $"{{x={XMin}..{XMax}, m={MMin}..{MMax}, a={AMin}..{AMax}, s={SMin}..{SMax}}}";
}