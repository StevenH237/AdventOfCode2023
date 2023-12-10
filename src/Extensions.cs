public static class AoC2023Extensions
{
  public static IEnumerable<T> Repeat<T>(this IEnumerable<T> seq, int count)
  {
    foreach (int i in Enumerable.Range(0, count))
    {
      foreach (T item in seq)
      {
        yield return item;
      }
    }
  }

  public static IEnumerable<T> RepeatInfinite<T>(this IEnumerable<T> seq)
  {
    while (true)
    {
      foreach (T item in seq)
      {
        yield return item;
      }
    }
  }

  public static T Pop<T>(this IList<T> list)
  {
    T val = list[0];
    list.RemoveAt(0);
    return val;
  }

  public static T AggregateFromFirst<T>(this IEnumerable<T> elems, Func<T, T, T> aggregation)
  {
    bool assigned = false;
    T aggregate = default(T);

    foreach (T item in elems)
    {
      if (!assigned)
      {
        aggregate = item;
        assigned = true;
      }
      else
      {
        aggregate = aggregation(aggregate, item);
      }
    }

    if (!assigned) throw new InvalidOperationException("Sequence contains no elements.");
    return aggregate;
  }

  public static IEnumerable<(T Item, int Index)> WithIndex<T>(this IEnumerable<T> original)
  {
    return original.Select((x, i) => (x, i));
  }
}