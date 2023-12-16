using System.Collections.Generic;
using Nixill.Collections;

public static class Day15
{
  public static string Part1(string fname, StreamReader input)
  {
    int sum = 0;
    foreach (string step in input.GetEverything().Split(","))
    {
      sum += Hash(step);
    }

    return sum.ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    DictionaryGenerator<int, List<(string Label, int FocalLength)>> lenses
      = new(new EmptyConstructorGenerator<int, List<(string, int)>>());

    // Follow instructions to build the array
    foreach (string step in input.GetEverything().Split(","))
    {
      if (step.EndsWith("-"))
      {
        string label = step[..^1];

        lenses[Hash(label)].RemoveAll(x => x.Label == label);
      }

      else
      {
        int pos = step.IndexOf('=');
        string label = step[..pos];
        int val = int.Parse(step[(pos + 1)..]);

        List<(string Label, int FocalLength)> list = lenses[Hash(label)];

        int index = list.WithIndex().Where(x => x.Item.Label == label).Select(x => x.Index).FirstOrDefault(-1);

        if (index >= 0)
        {
          list[index] = (label, val);
        }
        else
        {
          list.Add((label, val));
        }
      }
    }

    int sum = 0;
    // Get the focus power of the array
    foreach (int box in lenses.Keys)
    {
      foreach (((string label, int focalLength), int index) in lenses[box].WithIndex())
      {
        sum += (box + 1) * (index + 1) * focalLength;
      }
    }

    return sum.ToString();
  }

  public static int Hash(string input)
  {
    int hash = 0;
    foreach (char c in input)
    {
      hash += c;
      hash *= 17;
      hash %= 256;
    }

    return hash;
  }
}