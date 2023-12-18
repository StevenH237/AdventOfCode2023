using System.Numerics;

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

  public bool Equals(Direction other) => this.Normal == other.Normal;

  public static Direction operator -(Direction value)
  {
    return new(-value.Normal);
  }

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