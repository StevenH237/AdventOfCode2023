public class AdventMain
{
  static void Main(string[] args)
  {
    var which = "";

    if (args.Any()) which = args.First();
    else
    {
      Console.Write("Which program should be run? (\"All\", or a number [optionally followed by a or b]): ");
      which = Console.ReadLine()?.ToLowerInvariant();
    }

    if (which == null)
    {
      Console.WriteLine($"No value was specified. Exiting...");
    }
    else if (which == "all")
    {
      foreach (string i in Enumerable.Range(1, 25).Select(x => x.ToString()))
      {
        Console.WriteLine($"Day {i}: {Programs[i]()}");
      }
    }
    else if (Programs.ContainsKey(which))
    {
      Console.WriteLine($"{which}: {Programs[which]()}");
    }
    else
    {
      Console.WriteLine($"The specified day does not exist: {which}");
    }
  }

  static Dictionary<string, Func<string>> Programs = new()
  {
    ["1"] = () => $"[PART 1] {Day1Main.Part1()} / [PART 2] {Day1Main.Part2()}",
    ["1a"] = Day1Main.Part1,
    ["1b"] = Day1Main.Part2,
    ["2"] = () => $"[PART 1] {Day2Main.Part1()} / [PART 2] {Day2Main.Part2()}",
    ["2a"] = Day2Main.Part1,
    ["2b"] = Day2Main.Part2,
    ["3"] = () => $"[PART 1] {Day3Main.Part1()} / [PART 2] {Day3Main.Part2()}",
    ["3a"] = Day3Main.Part1,
    ["3b"] = Day3Main.Part2,
    ["4"] = () => $"[PART 1] {Day4Main.Part1()} / [PART 2] {Day4Main.Part2()}",
    ["4a"] = Day4Main.Part1,
    ["4b"] = Day4Main.Part2,
    ["5"] = () => $"[PART 1] {Day5Main.Part1()} / [PART 2] {Day5Main.Part2()}",
    ["5a"] = Day5Main.Part1,
    ["5b"] = Day5Main.Part2,
    ["6"] = () => $"[PART 1] {Day6Main.Part1()} / [PART 2] {Day6Main.Part2()}",
    ["6a"] = Day6Main.Part1,
    ["6b"] = Day6Main.Part2,
    ["7"] = () => $"[PART 1] {Day7Main.Part1()} / [PART 2] {Day7Main.Part2()}",
    ["7a"] = Day7Main.Part1,
    ["7b"] = Day7Main.Part2,
    ["8"] = () => $"[PART 1] {Day8Main.Part1()} / [PART 2] {Day8Main.Part2()}",
    ["8a"] = Day8Main.Part1,
    ["8b"] = Day8Main.Part2,
    ["9"] = () => $"[PART 1] {Day9Main.Part1()} / [PART 2] {Day9Main.Part2()}",
    ["9a"] = Day9Main.Part1,
    ["9b"] = Day9Main.Part2,
    ["10"] = () => $"[PART 1] {Day10Main.Part1()} / [PART 2] {Day10Main.Part2()}",
    ["10a"] = Day10Main.Part1,
    ["10b"] = Day10Main.Part2,
    ["11"] = () => $"[PART 1] {Day11Main.Part1()} / [PART 2] {Day11Main.Part2()}",
    ["11a"] = Day11Main.Part1,
    ["11b"] = Day11Main.Part2,
    ["12"] = () => $"[PART 1] {Day12Main.Part1()} / [PART 2] {Day12Main.Part2()}",
    ["12a"] = Day12Main.Part1,
    ["12b"] = Day12Main.Part2,
    ["13"] = () => $"[PART 1] {Day13Main.Part1()} / [PART 2] {Day13Main.Part2()}",
    ["13a"] = Day13Main.Part1,
    ["13b"] = Day13Main.Part2,
    ["14"] = () => $"[PART 1] {Day14Main.Part1()} / [PART 2] {Day14Main.Part2()}",
    ["14a"] = Day14Main.Part1,
    ["14b"] = Day14Main.Part2,
    ["15"] = () => $"[PART 1] {Day15Main.Part1()} / [PART 2] {Day15Main.Part2()}",
    ["15a"] = Day15Main.Part1,
    ["15b"] = Day15Main.Part2,
    ["16"] = () => $"[PART 1] {Day16Main.Part1()} / [PART 2] {Day16Main.Part2()}",
    ["16a"] = Day16Main.Part1,
    ["16b"] = Day16Main.Part2,
    ["17"] = () => $"[PART 1] {Day17Main.Part1()} / [PART 2] {Day17Main.Part2()}",
    ["17a"] = Day17Main.Part1,
    ["17b"] = Day17Main.Part2,
    ["18"] = () => $"[PART 1] {Day18Main.Part1()} / [PART 2] {Day18Main.Part2()}",
    ["18a"] = Day18Main.Part1,
    ["18b"] = Day18Main.Part2,
    ["19"] = () => $"[PART 1] {Day19Main.Part1()} / [PART 2] {Day19Main.Part2()}",
    ["19a"] = Day19Main.Part1,
    ["19b"] = Day19Main.Part2,
    ["20"] = () => $"[PART 1] {Day20Main.Part1()} / [PART 2] {Day20Main.Part2()}",
    ["20a"] = Day20Main.Part1,
    ["20b"] = Day20Main.Part2,
    ["21"] = () => $"[PART 1] {Day21Main.Part1()} / [PART 2] {Day21Main.Part2()}",
    ["21a"] = Day21Main.Part1,
    ["21b"] = Day21Main.Part2,
    ["22"] = () => $"[PART 1] {Day22Main.Part1()} / [PART 2] {Day22Main.Part2()}",
    ["22a"] = Day22Main.Part1,
    ["22b"] = Day22Main.Part2,
    ["23"] = () => $"[PART 1] {Day23Main.Part1()} / [PART 2] {Day23Main.Part2()}",
    ["23a"] = Day23Main.Part1,
    ["23b"] = Day23Main.Part2,
    ["24"] = () => $"[PART 1] {Day24Main.Part1()} / [PART 2] {Day24Main.Part2()}",
    ["24a"] = Day24Main.Part1,
    ["24b"] = Day24Main.Part2,
    ["25"] = () => $"[PART 1] {Day25Main.Part1()} / [PART 2] {Day25Main.Part2()}",
    ["25a"] = Day25Main.Part1,
    ["25b"] = Day25Main.Part2
  };
}