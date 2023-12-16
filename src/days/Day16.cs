using Nixill.Collections.Grid;

public class Day16
{
  D16Field Field;

  public Day16(string fname, StreamReader input)
  {
    Field = new(input.GetAllLines());
  }

  static Dictionary<string, Day16> results = new();

  static Day16 Get(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
      results[fname] = new Day16(fname, input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input)
  {
    Day16 result = Get(fname, input);
    result.Field.Simulate();
    return result.Field.GetVisitedTiles().ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    Day16 result = Get(fname, input);
    D16Field field = result.Field;

    int bestAnswer = 0;

    int w = field.Width;
    int h = field.Height;

    foreach (int c in Enumerable.Range(1, w - 2))
    {
      // Beam from top down
      field.Reset();
      field.AddBeam(c, 0, 0, 1);
      field.Simulate();
      int answer = field.GetVisitedTiles();
      if (answer > bestAnswer) bestAnswer = answer;

      // Beam from bottom up
      field.Reset();
      field.AddBeam(c, h - 1, 0, -1);
      field.Simulate();
      answer = field.GetVisitedTiles();
      if (answer > bestAnswer) bestAnswer = answer;
    }

    foreach (int r in Enumerable.Range(1, h - 2))
    {
      // Beam from top down
      field.Reset();
      field.AddBeam(0, r, 1, 0);
      field.Simulate();
      int answer = field.GetVisitedTiles();
      if (answer > bestAnswer) bestAnswer = answer;

      // Beam from bottom up
      field.Reset();
      field.AddBeam(w - 1, r, -1, 0);
      field.Simulate();
      answer = field.GetVisitedTiles();
      if (answer > bestAnswer) bestAnswer = answer;
    }

    return bestAnswer.ToString();
  }
}

public class D16Field
{
  internal Grid<char> Backing;
  internal Grid<bool> HorizLines;
  internal Grid<bool> VertLines;

  internal List<D16Beam> Beams = new();

  public int Width => Backing.Width;
  public int Height => Backing.Height;

  public D16Field(IEnumerable<IEnumerable<char>> input)
  {
    Backing = new(input);
    Backing.AddColumn('#');
    Backing.AddRow('#');
    Backing.InsertColumn(0, '#');
    Backing.InsertRow(0, '#');

    HorizLines = new(Backing.Width, Backing.Height);
    VertLines = new(Backing.Width, Backing.Height);

    Beams = new();
    Beams.Add(new(this, 0, 1, 1, 0));
  }

  public void Simulate()
  {
    while (Beams.Count > 0)
    {
      D16Beam beam = Beams.Pop();
      beam.Travel();
    }
  }

  public void Reset()
  {
    HorizLines = new(Backing.Width, Backing.Height);
    VertLines = new(Backing.Width, Backing.Height);
    Beams = new();
  }

  public void AddBeam(int x, int y, int Δx, int Δy)
  {
    Beams.Add(new(this, x, y, Δx, Δy));
  }

  public int GetVisitedTiles()
  {
    int total = 0;

    foreach (int r in Enumerable.Range(1, Backing.Height - 2))
    {
      foreach (int c in Enumerable.Range(1, Backing.Width - 2))
      {
        if (VertLines[r, c] || HorizLines[r, c])
        {
          total += 1;
        }
      }
    }

    return total;
  }
}

public class D16Beam(D16Field Parent, int X, int Y, int ΔX, int ΔY)
{
  public void Travel()
  {
    bool dead = false;

    while (!dead)
    {
      X += ΔX;
      Y += ΔY;

      switch (Parent.Backing[Y, X])
      {
        case '#': return; // Return quick just to avoid marking the tile as seen
        case '-':
          if (ΔX == 0)
          {
            Parent.Beams.Add(new D16Beam(Parent, X, Y, -1, 0));
            Parent.Beams.Add(new D16Beam(Parent, X, Y, 1, 0));
            dead = true;
          }
          break;
        case '|':
          if (ΔX != 0)
          {
            Parent.Beams.Add(new D16Beam(Parent, X, Y, 0, 1));
            Parent.Beams.Add(new D16Beam(Parent, X, Y, 0, -1));
            dead = true;
          }
          break;
        case '/':
          Parent.Beams.Add(new D16Beam(Parent, X, Y, -ΔY, -ΔX));
          dead = true;
          break;
        case '\\':
          Parent.Beams.Add(new D16Beam(Parent, X, Y, ΔY, ΔX));
          dead = true;
          break;
      }

      if (ΔX == 0)
        if (Parent.HorizLines[Y, X]) return;
        else Parent.HorizLines[Y, X] = true;
      else
        if (Parent.VertLines[Y, X]) return;
      else Parent.VertLines[Y, X] = true;
    }
  }
}
