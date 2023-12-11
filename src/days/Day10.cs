using Nixill.Collections.Grid;

public class Day10
{
  Grid<D10Cell> maze = new();

  D10Cell startCell = default(D10Cell);

  int FarthestDistance = 0;
  int farthestManhattanDistance = 0;

  internal Day10(string fname, StreamReader input)
  {
    // We'll need to enumerate the input twice so...
    string[] lines = input.GetAllLines();

    // Create grid
    maze = new(lines
      .Select((string line, int row) => line
        .Select((char c, int col) => new D10Cell(c, row, col))));

    // Find start position
    foreach ((string line, int row) in lines.WithIndex())
    {
      foreach ((char c, int col) in line.WithIndex())
      {
        if (c == 'S')
        {
          startCell = maze[row, col];
          goto foundStart;
        }
      }
    }
  foundStart:;

    // Determine connections
    if (startCell.Column != 0 && maze[startCell.Row, startCell.Column - 1].Right) startCell.Left = true;
    if (startCell.Column != (maze.Width - 1) && maze[startCell.Row, startCell.Column + 1].Left) startCell.Right = true;
    if (startCell.Row != 0 && maze[startCell.Row - 1, startCell.Column].Down) startCell.Up = true;
    if (startCell.Row != (maze.Height - 1) && maze[startCell.Row + 1, startCell.Column].Up) startCell.Down = true;

    // And now start navigating the path
    D10Cell cell = startCell;
    char lastMove = 'U';

    List<D10Cell> path = new() { cell };
    int distance = 0;

    do
    {
      if (lastMove != 'D' && cell.Up)
      {
        cell = maze[cell.Row - 1, cell.Column];
        lastMove = 'U';
      }
      else if (lastMove != 'R' && cell.Left)
      {
        cell = maze[cell.Row, cell.Column - 1];
        lastMove = 'L';
      }
      else if (lastMove != 'L' && cell.Right)
      {
        cell = maze[cell.Row, cell.Column + 1];
        lastMove = 'R';
      }
      else
      {
        cell = maze[cell.Row + 1, cell.Column];
        lastMove = 'D';
      }

      path.Add(cell);
      cell.InMainLoop = true;
      if (cell != startCell)
      {
        cell.Distance = ++distance;
        cell.ManhattanDistance = (Math.Abs(cell.Row - startCell.Row) + Math.Abs(cell.Column - startCell.Column));
        farthestManhattanDistance = int.Max(farthestManhattanDistance, cell.ManhattanDistance);
      }
    }
    while (cell != startCell);

    // Make the two halves both have the shorter distance around the loop
    path.Reverse();
    for (int i = 0; i < path.Count / 2; i++)
    {
      path[i].Distance = i;
    }

    FarthestDistance = path.Count / 2;
  }

  public string Part2()
  {
    int answer = 0;

    foreach (IEnumerable<D10Cell> row in maze)
    {
      D10State state = new();

      foreach (D10Cell cell in row)
      {
        if (cell.InMainLoop)
        {
          state.TR = (state.TL != cell.Up);
          state.BR = (state.BL != cell.Down);
        }
        if (state.TR && state.TL && state.BR && state.BL) answer += 1;
        state.TL = state.TR;
        state.BL = state.BR;
      }

      if (state.TL || state.TR || state.BL || state.BR) throw new InvalidDataException("Something went wrong!");
    }

    return answer.ToString();
  }

  // --------- Static stuff can sit at the bottom of the page --------- //
  static Dictionary<string, Day10> results = new();

  static Day10 Get(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
      results[fname] = new Day10(fname, input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input)
  {
    Day10 result = Get(fname, input);
    return result.FarthestDistance.ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    Day10 result = Get(fname, input);
    return result.Part2();
  }
}

public class D10Cell
{
  public char Input { get; internal set; }
  public int Row { get; internal set; }
  public int Column { get; internal set; }

  public bool Up { get; internal set; }
  public bool Left { get; internal set; }
  public bool Right { get; internal set; }
  public bool Down { get; internal set; }

  public int Distance { get; internal set; }
  public int ManhattanDistance { get; internal set; }

  public bool InMainLoop { get; internal set; }

  public D10Cell(char c, int row, int col)
  {
    Input = c;
    Left = (c == '-' || c == 'J' || c == '7');
    Right = (c == '-' || c == 'L' || c == 'F');
    Up = (c == '|' || c == 'L' || c == 'J');
    Down = (c == '|' || c == '7' || c == 'F');
    InMainLoop = (c == 'S');

    Distance = 0;
    ManhattanDistance = 0;

    InMainLoop = false;

    Column = col;
    Row = row;
  }
}

internal struct D10State()
{
  internal bool TL = false;
  internal bool TR = false;
  internal bool BL = false;
  internal bool BR = false;
}