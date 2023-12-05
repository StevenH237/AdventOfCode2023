using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nixill.Collections;
using Nixill.Utils;

public static class Day5
{
  static Dictionary<string, Day5Result> results = new();

  public static string Part1(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day5Result(input);
    }

    return results[fname].Part1Result;
  }

  public static string Part2(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day5Result(input);
    }

    return results[fname].Part2Result;
  }
}

public class Day5Result
{
  List<int> Seeds;
  List<D5Map> Maps;

  public readonly string Part1Result;
  public readonly string Part2Result;

  public Day5Result(StreamReader input)
  {
    // Start with the seed catcher
    string line = input.ReadLine();

    Seeds = new(line.Split(" ").Skip(1).Select(int.Parse));
    Maps = new();

    // Read the blank line 2
    input.ReadLine();

    while (!input.EndOfStream)
    {
      Maps.Add(CreateMap(input));
    }

    // Now iterate the inputs
    int lowLocation = Seeds.Select(seed => Maps.Aggregate(seed, (index, map) => map[index])).Min();

    Part1Result = lowLocation.ToString();
  }

  D5Map CreateMap(StreamReader input)
  {
    D5Map map = new();

    // Discard first line
    input.ReadLine();

    // Now start mapping
    while (!input.EndOfStream)
    {
      string line = input.ReadLine();
      if (line == "") break;
      map.AddLine(line);
    }

    return map;
  }
}

public class D5Map
{
  AVLTreeDictionary<int, int> backingMap;

  public D5Map()
  {
    backingMap = new() {
      [0] = 0
    };
  }

  public void AddRange(int sourceStart, int destStart, int size)
  {
    backingMap[sourceStart] = destStart - sourceStart;
    int sourceEnd = sourceStart + size;
    if (backingMap.FloorKey(sourceEnd) == sourceStart)
      backingMap[sourceEnd] = 0;
    else if (backingMap.FloorKey(sourceEnd) != sourceEnd)
      backingMap[sourceEnd] = backingMap.FloorEntry(sourceEnd).Value;
    while (backingMap.LowerKey(sourceEnd) != sourceStart)
      backingMap.Remove(backingMap.LowerKey(sourceEnd));
  }

  public void AddLine(string line)
  {
    int[] pars = line.Split(" ").Select(int.Parse).ToArray();
    AddRange(pars[0], pars[1], pars[2]);
  }

  public int this[int index] {
    get {
      int offset = backingMap.FloorEntry(index).Value;
      return index + offset;
    }
  }
}