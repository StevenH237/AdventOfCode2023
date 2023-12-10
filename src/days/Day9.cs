public class Day9
{
  readonly string Part1Result;
  readonly string Part2Result;

  List<D9NumberLine> lines = new();

  internal Day9(string fname, StreamReader input)
  {
    foreach (string line in input.GetLines())
    {
      lines.Add(new D9NumberLine(line));
    }

    Part1Result = lines.Select(x => x.RightExtrapolatedNumber).Sum().ToString();
    Part2Result = lines.Select(x => x.LeftExtrapolatedNumber).Sum().ToString();
  }

  // --------- Static stuff can sit at the bottom of the page --------- //
  static Dictionary<string, Day9> results = new();

  static Day9 Get(string fname, StreamReader input)
  {
    if (!results.ContainsKey(fname))
      results[fname] = new Day9(fname, input);
    return results[fname];
  }

  public static string Part1(string fname, StreamReader input)
  {
    Day9 result = Get(fname, input);
    return result.Part1Result;
  }

  public static string Part2(string fname, StreamReader input)
  {
    Day9 result = Get(fname, input);
    return result.Part2Result;
  }
}

public class D9NumberLine
{
  internal List<int> OriginalList;
  internal List<List<int>> AllLists = new();
  internal int Depth = 0;
  internal int LastNonzero;
  internal int RightExtrapolatedNumber;
  internal int LeftExtrapolatedNumber;

  public D9NumberLine(string input)
  {
    OriginalList = input.Split(" ").Select(int.Parse).ToList();
    List<int> lastList = OriginalList;
    bool keepLooping = true;

    // Get all the derivative lists
    while (keepLooping)
    {
      // We won't add the all 0 lists
      AllLists.Add(lastList);
      Depth += 1;
      keepLooping = false;
      List<int> nextList = new();

      for (int i = 1; i < lastList.Count; i++)
      {
        int nextVal = lastList[i] - lastList[i - 1];
        nextList.Add(nextVal);
        if (nextVal != 0)
        {
          keepLooping = true;
          LastNonzero = nextVal;
        }
      }

      lastList = nextList;
    }

    // And now expand them all!
    // First we'll do the final one outside the list just so it's the size
    // we expect it to be.
    lastList = AllLists[AllLists.Count - 1];
    lastList.Insert(0, LastNonzero);
    lastList.Add(LastNonzero);

    for (int l = AllLists.Count - 2; l >= 0; l--)
    {
      List<int> thisList = AllLists[l];
      thisList.Insert(0, thisList[0] - lastList[0]);

      int len = thisList.Count;
      thisList.Add(thisList[len - 1] + lastList[len - 1]);

      lastList = thisList;
    }

    // And lastly...
    RightExtrapolatedNumber = OriginalList[OriginalList.Count - 1];
    LeftExtrapolatedNumber = OriginalList[0];
  }
}