using Nixill.Collections.Grid;
using Nixill.Utils;

public static class Day13
{
  public static string Part1(string fname, StreamReader input)
  {
    var chunks = input.GetLines().ChunkWhile(x => x != "");

    int total = 0;

    foreach (IEnumerable<IEnumerable<char>> section in chunks)
    {
      Grid<char> grid = new(section);

      total += FindLine(grid);
    }

    return total.ToString();
  }

  public static int FindLine(Grid<char> field, bool isTransposed = false)
  {
    int foldRow = -1;

    // Iterate all rows from top to bottom
    foreach (int testFold in Enumerable.Range(1, field.Height - 1))
    {
      foreach (int row in Enumerable.Range(testFold, int.Min(testFold, field.Height - testFold)))
      {
        int mirror = 2 * testFold - (row + 1);
        foreach (int col in Enumerable.Range(0, field.Width))
        {
          if (field[row, col] != field[mirror, col]) goto nextFold;
        }
      }

      foldRow = testFold;
      goto foundRow;

    nextFold:;
    }

    if (!isTransposed) return FindLine(new Grid<char>(field.Columns), true);
    else return -1;

    foundRow:;
    if (isTransposed) return foldRow;
    else return 100 * foldRow;
  }

  public static string Part2(string fname, StreamReader input)
  {
    var chunks = input.GetLines().ChunkWhile(x => x != "");

    int total = 0;

    foreach (IEnumerable<IEnumerable<char>> section in chunks)
    {
      Grid<char> grid = new(section);

      total += FindSmudgedLine(grid);
    }

    return total.ToString();
  }

  public static int FindSmudgedLine(Grid<char> field, bool isTransposed = false)
  {
    int foldRow = -1;

    // Iterate all rows from top to bottom
    foreach (int testFold in Enumerable.Range(1, field.Height - 1))
    {
      bool smudgeCorrected = false;

      foreach (int row in Enumerable.Range(testFold, int.Min(testFold, field.Height - testFold)))
      {
        int mirror = 2 * testFold - (row + 1);
        foreach (int col in Enumerable.Range(0, field.Width))
        {
          if (field[row, col] != field[mirror, col])
          {
            if (smudgeCorrected) goto nextFold;
            else smudgeCorrected = true;
          }
        }
      }

      if (!smudgeCorrected) continue;

      foldRow = testFold;
      goto foundRow;

    nextFold:;
    }

    if (!isTransposed) return FindSmudgedLine(new Grid<char>(field.Columns), true);
    else return -1;

    foundRow:;
    if (isTransposed) return foldRow;
    else return 100 * foldRow;
  }
}