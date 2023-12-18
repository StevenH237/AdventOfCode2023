using System.Numerics;
using Nixill.Collections.Grid;

public class Day17
{
  public static string Part1(string fname, StreamReader input)
  {
    D17Maze maze = new(input.GetAllLines(), 1, 3);
    return maze.GetShortestPathLength().ToString();
  }

  public static string Part2(string fname, StreamReader input)
  {
    D17Maze maze = new(input.GetAllLines(), 4, 10);
    return maze.GetShortestPathLength().ToString();
  }
}

public class D17Maze
{
  Grid<int> Scores;
  Grid<D17Node> HorizontalMoves;
  Grid<D17Node> VerticalMoves;

  int MinMove;
  int MaxMove;

  int Width => Scores.Width;
  int Height => Scores.Height;

  Queue<D17Node> UnresolvedNodes;

  public D17Maze(IEnumerable<IEnumerable<char>> input, int min, int max)
  {
    Scores = new(input.Select(l => l.Select(c => c - 48)));

    HorizontalMoves = new(Scores.Width, Scores.Height);
    VerticalMoves = new(Scores.Width, Scores.Height);
    UnresolvedNodes = new();

    MinMove = min;
    MaxMove = max;

    int finalX = Scores.Width - 1;
    int finalY = Scores.Height - 1;
    int finalScore = Scores[finalY, finalX];

    D17Node finalHorizontal = new(new(finalX, finalY), Direction.Right, 0, finalScore, null);
    HorizontalMoves[finalY, finalX] = finalHorizontal;
    UnresolvedNodes.Enqueue(finalHorizontal);

    D17Node finalVertical = new(new(finalX, finalY), Direction.Down, 0, finalScore, null);
    VerticalMoves[finalY, finalX] = finalVertical;
    UnresolvedNodes.Enqueue(finalVertical);
  }

  public IEnumerable<D17Node> ChildrenOf(D17Node node)
  {
    int x = node.X;
    int y = node.Y;
    bool hor = !node.Horizontal;
    Direction dir = hor ? Direction.Right : Direction.Down;
    int score = node.Score;

    int nx = x;
    int ny = y;
    int nscore = score;

    // Forward direction (away from start) first
    foreach (int steps in Enumerable.Range(1, MaxMove))
    {
      nx += dir.ΔX;
      ny += dir.ΔY;

      if (nx >= Width || ny >= Height) break;
      nscore += Scores[ny, nx];

      if (steps >= MinMove) yield return new(new(nx, ny), -dir, steps, nscore, node);
    }

    // Reset for the return journey
    nx = x;
    ny = y;
    nscore = score;

    // Reverse direction (towards start) afterwards
    foreach (int steps in Enumerable.Range(1, MaxMove))
    {
      nx -= dir.ΔX;
      ny -= dir.ΔY;
      if (ny < 0 || nx < 0) break;
      nscore += Scores[ny, nx];

      if (steps >= MinMove) yield return new(new(nx, ny), dir, steps, nscore, node);
    }
  }

  public void ProcessOneNode()
  {
    D17Node node = UnresolvedNodes.Dequeue();
    Grid<D17Node> activeBoard = (node.Horizontal) ? VerticalMoves : HorizontalMoves;

    // "Cancelled" means that a shorter path from that location was
    // already found, and this node should not spawn any children. It's
    // easier to just mark it cancelled than it is to remove it from the
    // middle of the queue.
    if (node.Cancelled) return;

    foreach (D17Node newNode in ChildrenOf(node))
    {
      D17Node existingNode = activeBoard[newNode.Y, newNode.X];

      // First of all, if there's an existing node, make sure the new one
      // beats it.
      if (existingNode != null)
      {
        if (newNode.Score < existingNode.Score)
        {
          existingNode.Cancelled = true;
        }
        else
        {
          continue;
        }
      }

      activeBoard[newNode.Y, newNode.X] = newNode;

      // Don't spawn any nodes that move *into* the start position
      if (newNode.Y == 0 && newNode.X == 0) continue;

      UnresolvedNodes.Enqueue(newNode);
    }
  }

  public void ProcessAllNodes()
  {
    while (UnresolvedNodes.Count > 0)
    {
      ProcessOneNode();
    }
  }

  public int GetShortestPathLength()
  {
    if (UnresolvedNodes.Count > 0) ProcessAllNodes();
    return Math.Min(HorizontalMoves[0, 0].Score, VerticalMoves[0, 0].Score) - Scores[0, 0];
  }
}

public class D17Node
{
  public readonly Vector2 Pos;
  public readonly Direction Dir;
  public readonly int Magnitude;
  public readonly int Score;
  public readonly D17Node Next;

  public bool Cancelled { get; internal set; } = false;

  public Vector2 Move => Dir.Normal * Magnitude;
  public Vector2 End => Pos + Move;

  public int X => (int)Pos.X;
  public int Y => (int)Pos.Y;

  public bool Horizontal => Dir.Horizontal;

  public D17Node(Vector2 pos, Direction dir, int mag, int score, D17Node next)
  {
    Pos = pos;
    Dir = dir;
    Magnitude = mag;
    Score = score;
    Next = next;
  }
}