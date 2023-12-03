using System.Text.RegularExpressions;
using Nixill.Utils;

public static class Day2
{
  static Regex gamePattern = new(@"^Game (\d+): (.+)$");
  static Regex countPattern = new(@"^(\d+) (red|green|blue)$");

  public static string Part1(StreamReader input)
  {
    int redLimit = 12;
    int greenLimit = 13;
    int blueLimit = 14;

    int gameSum = 0;

    foreach (string line in input.GetLines())
    {
      if (gamePattern.TryMatch(line, out Match mtcGame))
      {
        // Extract the game id
        int gameID = int.Parse(mtcGame.Groups[1].Value);

        // Extract the text of the game
        string gameText = mtcGame.Groups[2].Value;

        // Evaluate each semicolon-separated set in the game
        foreach (string setText in gameText.Split(";", StringSplitOptions.TrimEntries))
        {
          int red = 0;
          int green = 0;
          int blue = 0;

          // Evaluate each comma-chunked count in the set
          foreach (string count in setText.Split(",", StringSplitOptions.TrimEntries))
          {
            if (countPattern.TryMatch(count, out Match mtcCount))
            {
              int number = int.Parse(mtcCount.Groups[1].Value);
              string color = mtcCount.Groups[2].Value.ToLower();

              switch (color)
              {
                case "red":
                  red += number;
                  break;
                case "green":
                  green += number;
                  break;
                case "blue":
                  blue += number;
                  break;
              }
            }
          }

          if (red > redLimit || green > greenLimit || blue > blueLimit) goto nextGame;
        }

        gameSum += gameID;
      }

    nextGame:;
    }

    return gameSum.ToString();
  }

  public static string Part2(StreamReader input)
  {
    int powerSum = 0;

    foreach (string line in input.GetLines())
    {
      if (gamePattern.TryMatch(line, out Match mtcGame))
      {
        // Extract the text of the game
        string gameText = mtcGame.Groups[2].Value;

        int maxRed = 0;
        int maxGreen = 0;
        int maxBlue = 0;

        // Evaluate each semicolon-separated set in the game
        foreach (string setText in gameText.Split(";", StringSplitOptions.TrimEntries))
        {
          int red = 0;
          int green = 0;
          int blue = 0;

          // Evaluate each comma-chunked count in the set
          foreach (string count in setText.Split(",", StringSplitOptions.TrimEntries))
          {
            if (countPattern.TryMatch(count, out Match mtcCount))
            {
              int number = int.Parse(mtcCount.Groups[1].Value);
              string color = mtcCount.Groups[2].Value.ToLower();

              switch (color)
              {
                case "red":
                  red += number;
                  break;
                case "green":
                  green += number;
                  break;
                case "blue":
                  blue += number;
                  break;
              }
            }
          }

          maxRed = int.Max(red, maxRed);
          maxGreen = int.Max(green, maxGreen);
          maxBlue = int.Max(blue, maxBlue);
        }

        powerSum += maxRed * maxGreen * maxBlue;
      }
    }

    return powerSum.ToString();
  }
}