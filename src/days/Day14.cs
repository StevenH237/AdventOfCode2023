using Nixill.Collections.Grid;
using Nixill.Utils;

public class Day14
{
  public static string Part1(string fname, StreamReader input)
  {
    D14Field field = new(input.GetLines().Cast<IEnumerable<char>>());
    field.RollNorthRotateRight();
    field.RotateLeft();
    return field.NorthMass().ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    D14Field field = new(input.GetLines().Cast<IEnumerable<char>>());
    Dictionary<int, int> cache = new();

    int spins = 0;
    int cycleTime = -1;

    foreach (int i in Enumerable.Range(1, 1_000_000_000))
    {
      spins = i;
      field.SpinCycle();
      if (cache.ContainsKey(field.GetHashCode()))
      {
        cycleTime = spins - cache[field.GetHashCode()];
        break;
      }
      else
      {
        cache[field.GetHashCode()] = spins;
      }

      if (spins % 10_000 == 0)
      {
        Console.WriteLine($"No loop in {spins} cycles.");
      }
    }

    int billionthCycle = 1_000_000_000 % cycleTime;
    int ourPosition = spins % cycleTime;
    int spinsToAdd = billionthCycle - ourPosition;
    if (spinsToAdd < 0) spinsToAdd += cycleTime;

    foreach (int i in Enumerable.Range(1, spinsToAdd))
    {
      field.SpinCycle();
    }

    return field.NorthMass().ToString();
  }
}

public class D14Field
{
  Grid<char> Backing = new();

  public D14Field(IEnumerable<IEnumerable<char>> field)
  {
    Backing = new(field);
  }

  public void RollNorthRotateRight()
  {
    List<char[]> outField = new();

    foreach (IEnumerable<char> column in Backing.Columns)
    {
      char[] colArray = column.ToArray();
      int len = colArray.Length;

      char[] outArray = Enumerable.Repeat('.', len).ToArray();
      int nextBoulder = len - 1;

      foreach (char c in colArray)
      {
        len--;
        if (c == 'O')
        {
          outArray[nextBoulder] = 'O';
          nextBoulder--;
        }
        if (c == '#')
        {
          outArray[len] = '#';
          nextBoulder = len - 1;
        }
      }

      outField.Add(outArray);
    }

    Backing = new(outField);
  }

  public void RotateLeft()
  {
    Backing = new(Backing.Columns.Reverse());
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
        if (c == 'O')
        {
          mass += len;
        }
        len--;
      }
    }

    return mass;
  }

  public void SpinCycle()
  {
    RollNorthRotateRight();
    RollNorthRotateRight();
    RollNorthRotateRight();
    RollNorthRotateRight();
  }

  public void PrintField()
  {
    Backing.Select(x => x.FormString()).Do(x => Console.WriteLine(x));
  }

  public override string ToString()
  {
    return String.Join("\n", Backing.Select(x => x.FormString()));
  }

  public override int GetHashCode()
  {
    return ToString().GetHashCode();
  }
}