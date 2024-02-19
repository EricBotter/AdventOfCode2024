var lines = File.ReadAllLines("input");

var sizeX = lines[0].Length;
var sizeY = lines.Length;

var map = new char[sizeX, sizeY];

for (int i = 0; i < sizeY; i++)
{
    for (int j = 0; j < sizeX; j++)
    {
        map[j, i] = lines[i][j];
    }
}

var northSegments = new List<Segment>();
for (var x = 0; x < sizeX; x++)
{
    var segmentEnd = 0;
    for (var y = 0; y < sizeY; y++)
    {
        if (map[x, y] == '#')
        {
            if (segmentEnd != y) 
                northSegments.Add(new Segment(x, y - 1, y - segmentEnd, Direction.North));

            segmentEnd = y + 1;
        }
    }

    if (segmentEnd != sizeY)
        northSegments.Add(new Segment(x, sizeY - 1, sizeY - segmentEnd, Direction.North));
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

var newBalls = new List<Ball>();
foreach (var segment in northSegments)
{
    var containedBalls = balls.Where(segment.Contains);
    var count = 0;
    foreach (var ball in containedBalls)
    {
        var newX = ball.X;
        var newY = segment.Y - segment.Length + 1 + count;
        newBalls.Add(new Ball(newX, newY));
        count++;
    }
}

Console.WriteLine(newBalls.Select(ball => ball.Load(sizeY)).Sum());

enum Direction
{
    North,
    West,
    South,
    East
}

record Ball(int X, int Y)
{
    public int Load(int maxY) => maxY - Y;
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

static class Exts
{
    public static string CharsToString(this IEnumerable<char> chars)
    {
        return chars.Aggregate("", (s, c) => s + c);
    }
}