using System.Drawing;
using System.Numerics;
using System.Text.RegularExpressions;
using Nixill.Collections;
using Nixill.Utils;

public class Day18
{
  D18Trench Trench;
  public static readonly Regex inputLineMatcher = new(@"([URDL]) (\d+) \(#([0-9a-f]{2})([0-9a-f]{2})([0-9a-f]{2})\)");

  public Day18(string fname, StreamReader input)
  {
    Trench = new();
    foreach (string line in input.GetLines())
    {
      if (inputLineMatcher.TryMatch(line, out Match mtc))
      {
        Direction dir = mtc.Groups[1].Value switch
        {
          "U" => Direction.Up,
          "R" => Direction.Right,
          "D" => Direction.Down,
          "L" => Direction.Left,
          _ => throw new InvalidDataException("Bad direction.")
        };
        int digLen = int.Parse(mtc.Groups[2].Value);
        int red = NumberUtils.StringToInt(mtc.Groups[3].Value, 16);
        int green = NumberUtils.StringToInt(mtc.Groups[4].Value, 16);
        int blue = NumberUtils.StringToInt(mtc.Groups[5].Value, 16);
        Color clr = Color.FromArgb(red, green, blue);

        Trench.DigMany(dir, digLen, clr);
      }
    }

    Trench.SavePerimeter();
    Trench.FillIn();
  }

  static Dictionary<string, Day18> results = new();

  static Day18 Get(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
      results[fname] = new Day18(fname, input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input)
  {
    Day18 result = Get(fname, input);
    return result.Trench.Count.ToString();
  }
}

public class D18Trench
{
  DictionaryGenerator<Vector2, D18Cell> Dict;
  Vector2 Here;

  int NorthEdge = 0;
  int SouthEdge = 0;
  int WestEdge = 0;
  int EastEdge = 0;

  public D18Trench()
  {
    Dict = new(new EmptyConstructorGenerator<Vector2, D18Cell>());
    Here = Vector2.Zero;

    D18Cell startCell = new() { IsPerimeter = true };
    var walls = startCell.Walls;

    Dict[Here] = startCell;
  }

  public int Count => Dict.Count;

  void DigOne(Direction dir, Color color, bool first)
  {
    D18Cell cell = Dict[Here];

    var walls = cell.Walls;
    walls.Remove(dir);
    if (first)
    {
      if (walls.ContainsKey(dir.TurnLeft())) walls[dir.TurnLeft()] = color;
      if (walls.ContainsKey(dir.TurnRight())) walls[dir.TurnRight()] = color;
    }

    Here += dir.Normal;
    cell = Dict[Here];
    walls = cell.Walls;

    walls.Remove(-dir);
    if (walls.ContainsKey(dir.TurnLeft())) walls[dir.TurnLeft()] = color;
    if (walls.ContainsKey(dir.TurnRight())) walls[dir.TurnRight()] = color;
    cell.IsPerimeter = true;
  }

  public void DigMany(Direction dir, int count, Color color)
  {
    DigOne(dir, color, true);
    foreach (int i in Enumerable.Range(1, count - 1))
      DigOne(dir, color, false);

    WestEdge = Math.Min(WestEdge, (int)Here.X);
    EastEdge = Math.Max(EastEdge, (int)Here.X);
    NorthEdge = Math.Min(NorthEdge, (int)Here.Y);
    SouthEdge = Math.Max(SouthEdge, (int)Here.Y);
  }

  public void FillInOne(Vector2 pos)
  {
    D18Cell cell = Dict.Add(pos);
    Dict[pos + Direction.Up.Normal].Walls.Remove(Direction.Down);
    Dict[pos + Direction.Down.Normal].Walls.Remove(Direction.Up);
    Dict[pos + Direction.Left.Normal].Walls.Remove(Direction.Right);
    Dict[pos + Direction.Right.Normal].Walls.Remove(Direction.Left);
  }

  public void SavePerimeter()
  {
    foreach (D18Cell cell in Dict.Values)
    {
      cell.Perimeter = new(cell.Walls.Keys);
    }
  }

  public void FillIn()
  {
    Dict.StoreGeneratedValues = false;

    foreach (int y in Enumerable.Range(NorthEdge, SouthEdge - NorthEdge + 1))
    {
      // Code reuse! :D
      D10State state = new();

      foreach (int x in Enumerable.Range(WestEdge, EastEdge - WestEdge + 1))
      {
        Vector2 pos = new Vector2(x, y);
        D18Cell cell = Dict[pos];

        if (cell.IsPerimeter)
        {
          state.TR = (state.TL == cell.Perimeter.Contains(Direction.Up));
          state.BR = (state.BL == cell.Perimeter.Contains(Direction.Down));
        }
        if (state.TR && state.TL && state.BR && state.BL) FillInOne(pos);
        state.TL = state.TR;
        state.BL = state.BR;
      }

      if (state.TL || state.TR || state.BL || state.BR) throw new InvalidDataException("Something went wrong!");
    }

    Dict.StoreGeneratedValues = true;
  }

  public void Print()
  {
    foreach (int y in Enumerable.Range(NorthEdge, SouthEdge - NorthEdge + 1))
    {
      foreach (int x in Enumerable.Range(WestEdge, EastEdge - WestEdge + 1))
      {
        Vector2 pos = new(x, y);
        if (Dict.ContainsKey(pos))
        {
          var walls = Dict[pos].Walls;
          bool up = !walls.ContainsKey(Direction.Up);
          bool down = !walls.ContainsKey(Direction.Down);
          bool left = !walls.ContainsKey(Direction.Left);
          bool right = !walls.ContainsKey(Direction.Right);

          if (up)
            if (left)
              if (down)
                if (right) Console.Write("┼");
                else Console.Write("┤");
              else
                if (right) Console.Write("┴");
              else Console.Write("┘");
            else
              if (down)
              if (right) Console.Write("├");
              else Console.Write("│");
            else
                if (right) Console.Write("└");
            else Console.Write("╵");
          else
            if (left)
            if (down)
              if (right) Console.Write("┬");
              else Console.Write("┐");
            else
              if (right) Console.Write("─");
            else Console.Write("╴");
          else
              if (down)
            if (right) Console.Write("┌");
            else Console.Write("╷");
          else
                if (right) Console.Write("╶");
          else Console.Write(".");

        }
        else
        {
          Console.Write("X");
        }
      }
      Console.WriteLine();
    }
  }
}

public class D18Cell
{
  internal DictionaryGenerator<Direction, Color> Walls = new(false)
  {
    [Direction.Up] = default(Color),
    [Direction.Down] = default(Color),
    [Direction.Left] = default(Color),
    [Direction.Right] = default(Color)
  };

  internal HashSet<Direction> Perimeter = new();

  internal bool IsPerimeter = false;
}
