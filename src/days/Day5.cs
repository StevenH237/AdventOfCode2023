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

    return results[fname].Part1();
  }

  public static string Part2(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day5Result(input);
    }

    return results[fname].Part2();
  }
}

public class Day5Result
{
  List<long> Seeds;
  List<(long Start, long End)> SeedRanges;
  List<D5Map> Maps;

  public Day5Result(StreamReader input)
  {
    // Start with the seed catcher
    string line = input.ReadLine();

    Seeds = new(line.Split(" ").Skip(1).Select(long.Parse));
    SeedRanges = new(Seeds.Chunk(2).Select(range => (range[0], range[0] + range[1])));
    Maps = new();

    // Read the blank line 2
    input.ReadLine();

    while (!input.EndOfStream)
    {
      Maps.Add(CreateMap(input));
    }
  }

  public string Part1()
  {
    // Now iterate the inputs
    return Seeds
      .Select(seed => Maps
        .Aggregate(seed, (index, map) => map[index]))
      .Min()
      .ToString();
  }

  public string Part2()
  {
    // Hoo boy, part 2 looks fun :D I might actually make my first
    // write-up on this based on the logic I worked out in a discord channel.

    D5Chart activeChart = new();
    SeedRanges.ForEach(activeChart.AddRange);

    // For each of the maps...
    foreach (D5Map map in Maps)
    {
      D5Chart newChart = new();

      // ... We'll start the next chart, and start off with a list of
      // active ranges.
      foreach (var range in activeChart.Ranges())
      {
        // First of all, we should get all applicable map rules for any
        // value in the range.
        var rules = map.KeysInEffect(range.Start, range.End);
        List<(long Start, long End, long Offset)> ranges = new();
        long lastStart = range.Start;

        foreach (var rule in rules.Skip(1))
        {
          if (lastStart != -1)
          {
            // We'll add the offsets here too because why not
            newChart.AddRangeEnd(lastStart + map.OffsetAt(lastStart), rule + map.OffsetAt(lastStart));
          }
          lastStart = rule;
        }

        newChart.AddRangeEnd(lastStart + map.OffsetAt(lastStart), range.End + map.OffsetAt(lastStart));
      }

      activeChart = newChart;
    }

    // And last...
    return activeChart.LowestNumber().ToString();
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

  public long OffsetAt(long index)
  {
    return backingMap.FloorEntry(index).Value;
  }

  public long[] KeysInEffect(long start, long end) => KeysInEffectIE(start, end).ToArray();

  IEnumerable<long> KeysInEffectIE(long start, long end)
  {
    for (long lastKey = backingMap.FloorKey(start); lastKey < end; /*nil*/)
    {
      yield return lastKey;
      if (!backingMap.TryGetHigherKey(lastKey, out lastKey)) break;
    }
  }
}

public class D5Chart
{
  AVLTreeDictionary<long, bool> backingMap;

  public D5Chart()
  {
    backingMap = new();
  }

  public void AddRangeEnd(long start, long end)
  {
    backingMap[start] = true;
    if (backingMap.FloorKey(end) == start)
      backingMap[end] = false;
    else if (backingMap.FloorKey(end) != end && !backingMap.FloorEntry(end).Value)
      backingMap[end] = false;

    while (backingMap.LowerKey(end) != start)
      backingMap.Remove(backingMap.LowerKey(end));
  }

  public void AddRangeCount(long start, long count) => AddRangeEnd(start, start + count);

  public void AddRange((long Start, long End) range) => AddRangeEnd(range.Start, range.End);

  public IEnumerable<(long Start, long End)> Ranges()
  {
    bool lastState = false;
    long lastStart = 0;
    foreach (var entry in backingMap)
    {
      if (entry.Value != lastState)
      {
        lastState = entry.Value;
        if (entry.Value)
          lastStart = entry.Key;
        else
          yield return (lastStart, entry.Key);
      }
    }
  }

  public long LowestNumber()
  {
    foreach (var entry in backingMap)
    {
      if (entry.Value) return entry.Key;
    }

    return -1;
  }
}