using System.Numerics;

public struct Coordinate
{
  public int X;
  public int Y;

  public Coordinate(int x, int y)
  {
    X = x;
    Y = y;
  }

  public static Coordinate FromRC(int r, int c) => new(c, r);
  public static Coordinate FromXY(int x, int y) => new(x, y);

  public static Coordinate Abs(Coordinate inp) => new(int.Abs(inp.X), int.Abs(inp.Y));
  public static long DistanceSquared(Coordinate inp) => (long)inp.X * inp.X + (long)inp.Y * inp.Y;
}

public struct Direction : IEquatable<Direction>, IEqualityOperators<Direction, Direction, bool>, IUnaryNegationOperators<Direction, Direction>
{
  public readonly Vector2 Normal;
  public bool Horizontal => Normal.Y == 0;

  public int ΔX => (int)Normal.X;
  public int ΔY => (int)Normal.Y;

  private Direction(Vector2 normal)
  {
    Normal = normal;
  }

  public static readonly Direction Up = new(new(0, -1));
  public static readonly Direction Down = new(new(0, 1));
  public static readonly Direction Left = new(new(-1, 0));
  public static readonly Direction Right = new(new(1, 0));

  static readonly Dictionary<Direction, Direction> RightTurns = new()
  {
    [Up] = Right,
    [Right] = Down,
    [Down] = Left,
    [Left] = Up
  };
  public Direction TurnRight() => RightTurns[this];

  static readonly Dictionary<Direction, Direction> LeftTurns = new()
  {
    [Up] = Left,
    [Right] = Up,
    [Down] = Right,
    [Left] = Down
  };
  public Direction TurnLeft() => LeftTurns[this];

  static readonly Dictionary<Direction, Direction> AroundTurns = new()
  {
    [Up] = Down,
    [Right] = Left,
    [Down] = Up,
    [Left] = Right
  };
  public Direction TurnAround() => AroundTurns[this];

  public bool Equals(Direction other) => this.Normal == other.Normal;

  public static Direction operator -(Direction value) => value.TurnAround();

  public static bool operator ==(Direction left, Direction right) => left.Equals(right);
  public static bool operator !=(Direction left, Direction right) => !left.Equals(right);

  public override bool Equals(object obj)
  {
    return (obj is Direction other) && (this.Equals(other));
  }

  public override int GetHashCode()
  {
    return Normal.GetHashCode();
  }
}