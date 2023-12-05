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
  List<long> Seeds1;
  List<(long Start, long Count)> Seeds2;
  List<D5Map> Maps;

  public readonly string Part1Result;
  public readonly string Part2Result;

  public Day5Result(StreamReader input)
  {
    // Start with the seed catcher
    string line = input.ReadLine();

    Seeds1 = new(line.Split(" ").Skip(1).Select(long.Parse));
    Seeds2 = new(Seeds1.Chunk(2).Select(x => (x[0], x[1])));
    Maps = new();

    // Read the blank line 2
    input.ReadLine();

    while (!input.EndOfStream)
    {
      Maps.Add(CreateMap(input));
    }

    // Now iterate the inputs
    long lowLocation = Seeds1
      .Select(seed => Maps
        .Aggregate(seed, (index, map) => map[index]))
      .Min();

    long lowLocation2 = long.MaxValue;
    foreach (var seedRange in Seeds2)
    {
      Console.WriteLine($"Processing range {seedRange.Start} × {seedRange.Count}...");

      long test = LongRange(seedRange.Start, seedRange.Count)
        .Select(seed => Maps
          .Aggregate(seed, (index, map) => map[index])
        ).Min();

      if (test < lowLocation2) lowLocation2 = test;
    }

    Part1Result = lowLocation.ToString();
    Part2Result = lowLocation2.ToString();
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

  IEnumerable<long> LongRange(long start, long count)
  {
    for (long i = start; i < (start + count); i++)
    {
      yield return i;
    }
  }
}

public class D5Map
{
  AVLTreeDictionary<long, long> backingMap;

  public D5Map()
  {
    backingMap = new()
    {
      [0] = 0
    };
  }

  public void AddRange(long destStart, long sourceStart, long size)
  {
    backingMap[sourceStart] = destStart - sourceStart;
    long sourceEnd = sourceStart + size;
    if (backingMap.FloorKey(sourceEnd) == sourceStart)
      backingMap[sourceEnd] = 0;
    else if (backingMap.FloorKey(sourceEnd) != sourceEnd)
      backingMap[sourceEnd] = backingMap.FloorEntry(sourceEnd).Value;
    while (backingMap.LowerKey(sourceEnd) != sourceStart)
      backingMap.Remove(backingMap.LowerKey(sourceEnd));
  }

  public void AddLine(string line)
  {
    long[] pars = line.Split(" ").Select(long.Parse).ToArray();
    AddRange(pars[0], pars[1], pars[2]);
  }

  public long this[long index]
  {
    get
    {
      long offset = backingMap.FloorEntry(index).Value;
      return index + offset;
    }
  }
}