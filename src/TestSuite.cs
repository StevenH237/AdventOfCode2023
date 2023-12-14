// For use in a REPL environment
public static class Tests
{
  // Returns a StreamReader over the example*.txt of a particular day.
  // (Skips two lines as the actual example runner does.)
  public static StreamReader GetExampleReader(string day, string file)
  {
    StreamReader input = new(File.OpenRead($"data/day{day}/{file}.txt"));

    input.ReadLine();
    input.ReadLine();

    return input;
  }

  // Returns a StreamReader over the input.txt of a particular day.
  public static StreamReader GetInputReader(string day)
  {
    StreamReader input = new(File.OpenRead($"data/day{day}/input.txt"));
    return input;
  }
}