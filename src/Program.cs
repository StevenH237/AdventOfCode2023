using System.Reflection;

public static class Program
{
  static void Main(string[] args)
  {
    string which = "";

    if (args.Any()) which = args.First();
    else
    {
      Console.Write("Which day's program should be run? (Enter a number): ");
      which = Console.ReadLine().ToLowerInvariant();
    }

    Type dayType = Type.GetType($"Day{which}");

    if (dayType == null)
    {
      Console.WriteLine("This day hasn't been written yet!");
      return;
    }

    if (!Directory.Exists($"./data/day{which}"))
    {
      Console.WriteLine("This day's data directory doesn't exist!");
      return;
    }

    MethodInfo part1Method = dayType.GetMethod("Part1", new Type[] { typeof(StreamReader) });
    if (part1Method != null)
    {
      Console.WriteLine("Running part 1...");
      if (RunTests(which, 1, part1Method.CreateDelegate<Func<StreamReader, string>>()))
      {
        RunCode(which, 1, part1Method.CreateDelegate<Func<StreamReader, string>>());
      }
      Console.WriteLine();
    }

    MethodInfo part2Method = dayType.GetMethod("Part2", new Type[] { typeof(StreamReader) });
    if (part2Method != null)
    {
      Console.WriteLine("Running part 2...");
      if (RunTests(which, 2, part2Method.CreateDelegate<Func<StreamReader, string>>()))
      {
        RunCode(which, 2, part2Method.CreateDelegate<Func<StreamReader, string>>());
      }
      Console.WriteLine();
    }
  }

  static bool RunTests(string day, int part, Func<StreamReader, string> method)
  {
    string[] examples = Directory.GetFiles($"data/day{day}", "example*.txt");
    bool tested = false;

    foreach (string fname in examples)
    {
      using StreamReader input = new(File.OpenRead(fname));

      // The first two lines are the expected answers
      string p1Answer = input.ReadLine();
      string p2Answer = input.ReadLine();
      string answer = (part == 1) ? p1Answer : p2Answer;

      // A blank line means that this input file doesn't apply to this part
      if (answer == "") continue;

      // Now perform the test!
      tested = true;
      string given = method(input);

      // Output the results!
      Console.WriteLine($"Test file: {(new FileInfo(fname)).Name} / Expected output: {answer} / Actual output: {given}");

      if (given != answer) return false;
    }

    if (!tested) Console.WriteLine("No test cases were provided (if this is part 2, did you remember to add answers?)! Continuing anyway...");
    else Console.WriteLine("All tests passed!");

    return true;
  }

  static void RunCode(string day, int part, Func<StreamReader, string> method)
  {
    using StreamReader input = new(File.OpenRead($"data/day{day}/input.txt"));

    Console.WriteLine($"Result on puzzle input: {method(input)}");
  }

  // For convenience:
  public static IEnumerable<string> GetLines(this StreamReader input)
  {
    for (string line = input.ReadLine(); line != null && line != ""; line = input.ReadLine())
    {
      yield return line;
    }
  }

  public static string[] GetAllLines(this StreamReader input) => input.GetLines().ToArray();

  public static string GetEverything(this StreamReader input)
  {
    return input.ReadToEnd();
  }
}