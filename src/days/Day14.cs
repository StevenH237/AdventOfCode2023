using Nixill.Collections.Grid;

public class Day14
{
  D14Field Field;

  public Day14(string fname, StreamReader input)
  {
    Field = new(input.GetLines());
  }

  static Dictionary<string, Day14> results = new();

  static Day14 Get(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
      results[fname] = new Day14(fname, input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input) => Get(fname, input).Field.NorthMass().ToString();
}

public class D14Field
{
  Grid<char> Backing = new();

  public D14Field(IEnumerable<IEnumerable<char>> field)
  {
    Backing = new(field);
  }

  public int NorthMass()
  {
    int mass = 0;

    foreach (IEnumerable<char> column in Backing.Columns)
    {
      char[] colArray = column.ToArray();

      int len = colArray.Length;
      int nextBoulder = len;

      foreach (char c in colArray)
      {
        len--;
        if (c == 'O')
        {
          mass += nextBoulder;
          nextBoulder--;
        }
        else if (c == '#')
        {
          nextBoulder = len;
        }
      }
    }

    return mass;
  }
}