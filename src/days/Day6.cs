public static class Day6
{
  public static string Part1(string fname, StreamReader input)
  {
    List<long> raceTimes = new(input.ReadLine().Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse));
    List<long> raceRecords = new(input.ReadLine().Split(":")[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(long.Parse));

    return raceTimes.Zip(raceRecords, (t, r) => new D6Race { RaceTime = t, RaceRecord = r }).Select(x => x.CountWinningTimes()).Aggregate(1L, (p, c) => p * c).ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    long raceTime = long.Parse(input.ReadLine().Split(":")[1].Replace(" ", ""));
    long raceRecord = long.Parse(input.ReadLine().Split(":")[1].Replace(" ", ""));

    return new D6Race { RaceTime = raceTime, RaceRecord = raceRecord }.CountWinningTimes().ToString();
  }
}

public struct D6Race
{
  public long RaceTime;
  public long RaceRecord;

  public long GetDistance(long ButtonTime) => ButtonTime * (RaceTime - ButtonTime);

  public long CountWinningTimes()
  {
    long HighestLoss = 0;
    long LowestWin = RaceTime / 2L;

    while (LowestWin > (HighestLoss + 1))
    {
      long TryMS = (HighestLoss + LowestWin) / 2;
      long TryDist = GetDistance(TryMS);
      if (TryDist > RaceRecord)
        LowestWin = TryMS;
      else
        HighestLoss = TryMS;
    }

    return (RaceTime / 2L - HighestLoss) * 2L - ((RaceTime % 2 == 0) ? 1 : 0);
  }
}