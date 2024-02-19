
var lines = File.ReadAllLines("input");

var sizeX = lines[0].Length;
var sizeY = lines.Length;

var map = new char[sizeX, sizeY];

for (var i = 0; i < sizeY; i++)
for (var j = 0; j < sizeX; j++)
    map[j, i] = lines[i][j];

var northSegments = new List<Segment>();
var southSegments = new List<Segment>();
for (var x = 0; x < sizeX; x++)
{
    var segmentBoundary = 0;
    for (var y = 0; y < sizeY; y++)
    {
        if (map[x, y] == '#')
        {
            if (segmentBoundary != y)
            {
                northSegments.Add(new Segment(x, y - 1, y - segmentBoundary, Direction.North));
                southSegments.Add(new Segment(x, segmentBoundary, y - segmentBoundary, Direction.South));
            }

            segmentBoundary = y + 1;
        }
    }

    if (segmentBoundary != sizeY)
    {
        northSegments.Add(new Segment(x, sizeY - 1, sizeY - segmentBoundary, Direction.North));
        southSegments.Add(new Segment(x, segmentBoundary, sizeY - segmentBoundary, Direction.South));
    }
}

var westSegments = new List<Segment>();
var eastSegments = new List<Segment>();
for (var y = 0; y < sizeY; y++)
{
    var segmentBoundary = 0;
    for (var x = 0; x < sizeX; x++)
    {
        if (map[x, y] == '#')
        {
            if (segmentBoundary != x)
            {
                westSegments.Add(new Segment(x - 1, y, x - segmentBoundary, Direction.West));
                eastSegments.Add(new Segment(segmentBoundary, y, x - segmentBoundary, Direction.East));
            }

            segmentBoundary = x + 1;
        }
    }

    if (segmentBoundary != sizeX)
    {
        westSegments.Add(new Segment(sizeX - 1, y, sizeX - segmentBoundary, Direction.West));
        eastSegments.Add(new Segment(segmentBoundary, y, sizeX - segmentBoundary, Direction.East));
    }
}

var balls = new List<Ball>();
for (var x = 0; x < sizeX; x++)
{
    for (var y = 0; y < sizeY; y++)
    {
        if (map[x, y] == 'O')
        {
            balls.Add(new Ball(x, y));
        }
    }
}

balls.Sort();
var ballSnapshots = new List<List<Ball>>();
var iterations = 0;

do
{
    ballSnapshots.Add(balls);
    foreach (var direction in new[] { Direction.North, Direction.West, Direction.South, Direction.East })
    {
        var currentSegments = direction switch
        {
            Direction.North => northSegments,
            Direction.West => westSegments,
            Direction.South => southSegments,
            Direction.East => eastSegments,
            _ => throw new ArgumentOutOfRangeException()
        };

        var newBalls = new List<Ball>();
        foreach (var segment in currentSegments)
        {
            var containedBalls = balls.Where(segment.Contains);
            var count = 0;
            foreach (var ball in containedBalls)
            {
                var newX = direction switch
                {
                    Direction.North => ball.X,
                    Direction.West => segment.X - segment.Length + 1 + count,
                    Direction.South => ball.X,
                    Direction.East => segment.X + segment.Length - 1 - count,
                    _ => throw new ArgumentOutOfRangeException()
                };

                var newY = direction switch
                {
                    Direction.North => segment.Y - segment.Length + 1 + count,
                    Direction.West => ball.Y,
                    Direction.South => segment.Y + segment.Length - 1 - count,
                    Direction.East => ball.Y,
                    _ => throw new ArgumentOutOfRangeException()
                };

                newBalls.Add(new Ball(newX, newY));
                count++;
            }
        }

        balls = newBalls;
    }

    balls.Sort();
    
    iterations++;
    Console.WriteLine($"{DateTime.Now:hh:mm:ss.fff}: {iterations}");

} while (!ballSnapshots.Any(snapshot => snapshot.SequenceEqual(balls)));

for (var i = 0; i < ballSnapshots.Count; i++)
{
    if (ballSnapshots[i].SequenceEqual(balls))
    {
        var loopLength = ballSnapshots.Count - i;
        var mySnapshot = i;
        while (mySnapshot < 1_000_000_000)
        {
            mySnapshot += loopLength;
        }
        mySnapshot -= loopLength;
        var snapShotPos = i + (1_000_000_000 - mySnapshot);
        Console.WriteLine(ballSnapshots[snapShotPos == ballSnapshots.Count ? i : snapShotPos].Select(ball => ball.Load(sizeY)).Sum());
        return;
    }
}

enum Direction
{
    North,
    West,
    South,
    East
}

record Ball(int X, int Y) : IComparable<Ball>
{
    public int Load(int maxY) => maxY - Y;
    
    public int CompareTo(Ball? other)
    {
        return X == other!.X ? Y.CompareTo(other.Y) : X.CompareTo(other.X);
    }   
};

record Segment(int X, int Y, int Length, Direction Direction)
{
    public bool Contains(Ball ball)
    {
        return Direction switch
        {
            Direction.South => X == ball.X && Y <= ball.Y && Y + Length > ball.Y,
            Direction.West => Y == ball.Y && X >= ball.X && X - Length < ball.X,
            Direction.North => X == ball.X && Y >= ball.Y && Y - Length < ball.Y,
            Direction.East => Y == ball.Y && X <= ball.X && X + Length > ball.X,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
