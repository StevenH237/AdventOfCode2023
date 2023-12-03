using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nixill.Collections;
using Nixill.Utils;

public static class Day3
{
  static Dictionary<string, Day3Result> results = new();

  public static string Part1(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
    {
      results[fname] = new Day3Result(input);
    }

    return results[fname].Part1Result;
  }

  // public static string Part2(string fname, StreamReader input)
  // {
  //   if (!results.ContainsKey(fname))
  //   {
  //     results[fname] = new Day3Result(input);
  //   }

  //   return results[fname].Part2Result;
  // }
}

public class Day3Result
{
  static Regex RgNumber = new(@"\d+");
  static Regex RgSymbol = new(@"[^\d\.]");

  public readonly string Part1Result;
  public readonly string Part2Result;

  public Day3Result(StreamReader input)
  {
    List<D3Number> numbers = new();
    Dictionary<(int x, int y), D3Symbol> symbols = new();

    foreach ((string line, int y) in input.GetLines().Select((x, i) => (x, i)))
    {
      // Record every number separately
      foreach (Match match in RgNumber.Matches(line))
      {
        numbers.Add(new D3Number()
        {
          x = match.Index,
          l = match.Length,
          y = y,
          value = int.Parse(match.Value)
        });
      }

      // ... and every symbol
      foreach (Match match in RgSymbol.Matches(line))
      {
        int x = match.Index;
        symbols[(x, y)] = new D3Symbol()
        {
          x = x,
          y = y,
          symbol = match.Value[0]
        };
      }
    }

    int sum = 0;

    foreach (D3Number num in numbers)
    {
      // I'm gonna be slightly inefficient here, but this is a small
      // enough program that it shouldn't matter. I'll just search for all
      // up-to-15 positions that could be adjacent *or within* a number.
      // It also doesn't matter that some numbers could be on the edge of
      // the grid - in this case, we'll just look for symbols in, say, row
      // -1, none will exist, and the code will move on.
      foreach (int y in Enumerable.Range(num.y - 1, 3))
      {
        foreach (int x in Enumerable.Range(num.x - 1, num.l + 2))
        {
          if (symbols.ContainsKey((x, y)))
          {
            symbols[(x, y)].used = true;
            num.used = true;
            sum += num.value;
            goto nextNum;
          }
        }
      }

    nextNum:;
    }

    Part1Result = sum.ToString();
  }
}

internal class D3Number
{
  internal int x;
  internal int l;
  internal int y;
  internal int value;
  internal bool used = false;
}

internal class D3Symbol
{
  internal int x;
  internal int y;
  internal char symbol;
  internal bool used = false;
}