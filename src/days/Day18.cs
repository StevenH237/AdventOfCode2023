using System.Text.RegularExpressions;
using Nixill.Collections;
using Nixill.Utils;

public static class Day18
{
  static IEnumerable<(int ΔX, int ΔY, long L)> Part1Parser(IEnumerable<string> input)
  {
    Regex rgxMatcher = new(@"^([DRUL]) (\d+) \(#[0-9a-f]{6}\)$");
    foreach (string line in input)
    {
      if (rgxMatcher.TryMatch(line, out Match mtc))
      {
        (int ΔX, int ΔY) = mtc.Groups[1].Value switch
        {
          "D" => (0, 1),
          "U" => (0, -1),
          "L" => (-1, 0),
          "R" => (1, 0),
          _ => (0, 0)
        };

        int length = int.Parse(mtc.Groups[2].Value);

        yield return (ΔX, ΔY, length);
      }
    }
  }

  static IEnumerable<(int ΔX, int ΔY, long L)> Part2Parser(IEnumerable<string> input)
  {
    Regex rgxMatcher = new(@"^[DRUL] \d+ \(#([0-9a-f]{5})([0-3])\)$");
    foreach (string line in input)
    {
      if (rgxMatcher.TryMatch(line, out Match mtc))
      {
        int length = NumberUtils.StringToInt(mtc.Groups[1].Value, 16);
        (int ΔX, int ΔY) = mtc.Groups[2].Value switch
        {
          "0" => (1, 0),
          "1" => (0, 1),
          "2" => (-1, 0),
          "3" => (0, -1),
          _ => (0, 0)
        };

        yield return (ΔX, ΔY, length);
      }
    }
  }

  public static string Part1(string fname, StreamReader input)
  {
    D18Field field = new(Part1Parser, input.GetAllLines());
    return field.GetArea().ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    D18Field field = new(Part2Parser, input.GetAllLines());
    return field.GetArea().ToString();
  }
}

public delegate IEnumerable<(int ΔX, int ΔY, long Length)> InputParser(IEnumerable<string> inputs);

public class D18Field
{
  AVLTreeDictionary<long, AVLTreeSet<long>> Coordinates = new();
  DictionaryGenerator<long, AVLTreeSet<long>> GenCoords;
  long Perimeter = 0;

  public D18Field(InputParser parser, IEnumerable<string> input)
  {
    GenCoords = new(Coordinates, new EmptyConstructorGenerator<long, AVLTreeSet<long>>());

    long x = 0;
    long y = 0;

    foreach ((int Δx, int Δy, long length) in parser(input))
    {
      x += Δx * length;
      y += Δy * length;

      GenCoords[x].Add(y);
      Perimeter += length;
    }
  }

  public long GetArea()
  {
    long area = 0;
    long currentArea = 0;
    AVLTreeSet<long> currentBounds = new();

    long lastX = Coordinates.LowestKey();

    foreach ((long x, AVLTreeSet<long> lineX) in Coordinates)
    {
      area += currentArea * (x - lastX);

      foreach (long y in lineX)
      {
        if (currentBounds.Contains(y)) currentBounds.Remove(y);
        else currentBounds.Add(y);
      }

      currentArea = 0;
      foreach (long[] twoY in currentBounds.Chunk(2))
      {
        currentArea += twoY[1] - twoY[0];
      }

      lastX = x;
    }

    return area + Perimeter / 2 + 1;
  }
}