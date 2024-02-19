
var lines = File.ReadAllLines("input");

var map = new char[lines[0].Length, lines.Length];
var distanceMap = new int[map.GetLength(0), map.GetLength(1)];

var startingPoint = new Point(0, 0);

for (var i = 0; i < lines.Length; i++)
{
    for (var j = 0; j < lines[i].Length; j++)
    {
        map[j, i] = lines[i][j];
        distanceMap[j, i] = -1;
        if (map[j, i] == 'S')
        {
            startingPoint = new Point(j, i);
            distanceMap[startingPoint.X, startingPoint.Y] = 0;
        }
    }
}

var queue = new Queue<Point>();
queue.Enqueue(startingPoint);

while (queue.TryDequeue(out var currentPoint))
{
    foreach (var dir in new List<Direction> { Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT })
    {
        var newPoint = dir switch
        {
            Direction.UP => currentPoint with { Y = currentPoint.Y - 1 },
            Direction.RIGHT => currentPoint with { X = currentPoint.X + 1 },
            Direction.DOWN => currentPoint with { Y = currentPoint.Y + 1 },
            Direction.LEFT => currentPoint with { X = currentPoint.X - 1 },
            _ => throw new ArgumentOutOfRangeException()
        };

        if (IsConnected(dir, map.Get(newPoint)) && distanceMap.Get(newPoint)!.Value == -1)
        {
            queue.Enqueue(newPoint);
            distanceMap.Set(newPoint, distanceMap.Get(currentPoint)!.Value + 1);
        }
    }
}

Console.WriteLine(distanceMap.Cast<int>().Max());

bool IsConnected(Direction direction, char? destMaybe)
{
    if (destMaybe is not { } dest) return false;
    return direction switch
    {
        Direction.UP => "|7F".Contains(dest),
        Direction.RIGHT => "-J7".Contains(dest),
        Direction.DOWN => "|LJ".Contains(dest),
        Direction.LEFT => "-LF".Contains(dest),
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };
}

static class Exts
{
    public static T? Get<T>(this T[,] source, Point location) where T: struct
    {
        try
        {
            return source[location.X, location.Y];
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }
    
    public static void Set<T>(this T[,] source, Point location, T value) where T: struct
    {
        try
        {
            source[location.X, location.Y] = value;
        }
        catch (IndexOutOfRangeException)
        {
        }
    }
}

enum Direction
{
    UP,
    RIGHT,
    DOWN,
    LEFT
}

record Point(int X, int Y);