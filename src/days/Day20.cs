using System.Text.RegularExpressions;
using Nixill.Collections;
using Nixill.Utils;

public class Day20
{
  D20Field Field;

  public Day20(string fname, StreamReader input)
  {
    Field = new(input.GetLines());
  }

  static Dictionary<string, Day20> results = new();

  static Day20 Get(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
      results[fname] = new Day20(fname, input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input)
  {
    D20Field field = new(input.GetLines());
    foreach (int i in Enumerable.Range(1, 1000)) field.PressTheButton().Count(); // Count forces full enum
    return (field.HighPulses * field.LowPulses).ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    HashSet<string> loops = new();
    Dictionary<string, int> loopSizes = new();

    D20Field field = new(input.GetLines());

    foreach (string target in field.OutputsToFinal) loops.Add(target);

    while (true)
    {
      foreach (D20Signal signal in field.PressTheButton())
      {
        if (signal.High && loops.Contains(signal.Source))
        {
          loops.Remove(signal.Source);
          loopSizes[signal.Source] = field.ButtonPresses;

          if (loops.Count == 0) return
            loopSizes.Values
              .Select(x => (long)x)
              .AggregateFromFirst((a, x) => a * x)
              .ToString();
        }
      }
    }

    throw new InvalidOperationException("How did you reach this error? 2");
  }
}

public class D20Field
{
  Dictionary<string, D20Module> Modules = new();
  internal bool ButtonProcessing = false;

  internal List<string> OutputsToFinal = new();

  internal int ButtonPresses = 0;
  internal long HighPulses = 0;
  internal long LowPulses = 0;

  static readonly Regex Parser = new(@"^([%&]?)([a-z]+) -> ((?:[a-z]+, )*[a-z]+)$");

  public D20Field(IEnumerable<string> input)
  {
    DictionaryGenerator<string, List<string>> inputs = new(new EmptyConstructorGenerator<string, List<string>>());

    foreach (string line in input)
    {
      Match mtc = Parser.Match(line);
      string id = mtc.Groups[2].Value;
      IEnumerable<string> targets = mtc.Groups[3].Value.Split(", ");
      D20Module module = mtc.Groups[1].Value switch
      {
        "" => new D20BroadcasterModule(targets),
        "%" => new D20FlipFlopModule(id, targets),
        "&" => new D20ConjunctionModule(id, targets),
        _ => throw new InvalidDataException("How did you reach this error? 1")
      };

      Modules[id] = module;
      foreach (string target in targets) inputs[target].Add(id);

      if (inputs.ContainsKey("rx"))
        OutputsToFinal = inputs[inputs["rx"].First()];
    }

    foreach (D20ConjunctionModule mod in Modules.Values.Where(x => x is D20ConjunctionModule))
    {
      foreach (string target in inputs[mod.ID])
      {
        mod.AddInput(target);
      }
    }
  }

  public IEnumerable<D20Signal> PressTheButton()
  {
    ButtonPresses += 1;

    // This is singlethreaded code, so this error will only be thrown if I
    // forget something in the code.
    if (ButtonProcessing) throw new InvalidOperationException("Cannot press button while processing a previous press!");
    ButtonProcessing = true;

    Queue<D20Signal> queue = new();
    queue.Enqueue(new("button", "broadcaster", false));

    while (queue.Count > 0)
    {
      D20Signal signal = queue.Dequeue();

      if (signal.High) HighPulses += 1;
      else LowPulses += 1;

      yield return signal;

      if (Modules.ContainsKey(signal.Target))
      {
        foreach (D20Signal result in Modules[signal.Target].ProcessInput(signal))
        {
          queue.Enqueue(result);
        }
      }
    }

    ButtonProcessing = false;
  }

  public override int GetHashCode() => string.Join('\n', Modules.Values
    .OrderBy(x => x.ID)
    .Select(x => $"{x.ID}={x.GetHashCode()}")).GetHashCode();
}

public abstract class D20Module
{
  public readonly string ID;
  protected List<string> Targets;

  public D20Module(string id, IEnumerable<string> targets)
  {
    ID = id;
    Targets = targets.ToList();
  }

  public abstract IEnumerable<D20Signal> ProcessInput(D20Signal received);

  public abstract override int GetHashCode();
}

public class D20BroadcasterModule : D20Module
{
  public D20BroadcasterModule(IEnumerable<string> targets)
    : base("broadcaster", targets) { }

  public override IEnumerable<D20Signal> ProcessInput(D20Signal received)
  {
    foreach (string target in Targets) yield return new(ID, target, received.High);
  }

  public override int GetHashCode() => "broadcaster".GetHashCode();
}

public class D20FlipFlopModule : D20Module
{
  bool State = false;

  public D20FlipFlopModule(string id, IEnumerable<string> targets)
    : base(id, targets) { }

  public override IEnumerable<D20Signal> ProcessInput(D20Signal received)
  {
    if (received.High) yield break;
    State = !State;

    foreach (string target in Targets) yield return new(ID, target, State);
  }

  public override int GetHashCode() => $"{ID}/{State}".GetHashCode();
}

public class D20ConjunctionModule : D20Module
{
  Dictionary<string, bool> States = new();

  public D20ConjunctionModule(string id, IEnumerable<string> targets)
    : base(id, targets) { }

  public void AddInput(string input)
  {
    States[input] = false;
  }

  public override IEnumerable<D20Signal> ProcessInput(D20Signal received)
  {
    States[received.Source] = received.High;

    bool output = (States.Values.Where(x => !x).Any());
    foreach (string target in Targets) yield return new(ID, target, output);
  }

  public override int GetHashCode() => $"{ID}/{string.Join("/", States.Select(x => $"{x.Key}={x.Value}"))}".GetHashCode();
}

public struct D20Signal
{
  public string Source;
  public string Target;
  public bool High;

  public D20Signal(string source, string target, bool high)
  {
    Source = source;
    Target = target;
    High = high;
  }
}