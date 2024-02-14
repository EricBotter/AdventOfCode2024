var lines = File.ReadAllLines("input");

var map = new char[lines[0].Length, lines.Length];
var loopMap = new int[map.GetLength(0), map.GetLength(1)];

var startingPoint = new Point(0, 0);

for (var i = 0; i < lines.Length; i++)
{
    for (var j = 0; j < lines[i].Length; j++)
    {
        map[j, i] = lines[i][j];
        loopMap[j, i] = 0;
        if (map[j, i] == 'S')
        {
            startingPoint = new Point(j, i);
            loopMap[j, i] = 1;
        }
    }
}

var queue = new Queue<Point>();
var processed = new List<Point>();
queue.Enqueue(startingPoint);

while (queue.TryDequeue(out var currentPoint))
{
    var directionsToCheck = map.Get(currentPoint) switch
    {
        'S' => new List<Direction> { Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT },
        '|' => new List<Direction> { Direction.UP, Direction.DOWN},
        '-' => new List<Direction> { Direction.RIGHT, Direction.LEFT },
        'L' => new List<Direction> { Direction.UP, Direction.RIGHT },
        'J' => new List<Direction> { Direction.UP, Direction.LEFT },
        '7' => new List<Direction> { Direction.DOWN, Direction.LEFT },
        'F' => new List<Direction> {Direction.RIGHT, Direction.DOWN },
        _ => throw new ArgumentOutOfRangeException()
    };
    
    foreach (var dir in directionsToCheck)
    {
        var newPoint = dir switch
        {
            Direction.UP => currentPoint with { Y = currentPoint.Y - 1 },
            Direction.RIGHT => currentPoint with { X = currentPoint.X + 1 },
            Direction.DOWN => currentPoint with { Y = currentPoint.Y + 1 },
            Direction.LEFT => currentPoint with { X = currentPoint.X - 1 },
            _ => throw new ArgumentOutOfRangeException()
        };

        if (IsConnected(dir, map.Get(newPoint)) && loopMap.Get(newPoint)!.Value == 0)
        {
            queue.Enqueue(newPoint);
            if (processed.Contains(currentPoint) && currentPoint != startingPoint)
                throw new Exception();
            processed.Add(currentPoint);
            loopMap.Set(newPoint, 1);
        }
    }
}

for (var i = 0; i < map.GetLength(1); i++)
{
    for (var j = 0; j < map.GetLength(0); j++)
    {
        if (loopMap[j, i] == 0)
            map[j, i] = '.';
    }
}

var dirs = new List<Direction> { Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT };
var connections = new[] { false, false, false, false };
for (var i = 0; i < dirs.Count; i++)
{
    var dir = dirs[i];
    var newPoint = dir switch
    {
        Direction.UP => startingPoint with { Y = startingPoint.Y - 1 },
        Direction.RIGHT => startingPoint with { X = startingPoint.X + 1 },
        Direction.DOWN => startingPoint with { Y = startingPoint.Y + 1 },
        Direction.LEFT => startingPoint with { X = startingPoint.X - 1 },
        _ => throw new ArgumentOutOfRangeException()
    };

    connections[i] = IsConnected(dir, map.Get(newPoint));
}

map.Set(startingPoint, (connections[0], connections[1], connections[2], connections[3]) switch
{
    (true, false, true, false) => '|',
    (false, true, false, true) => '-',
    (true, true, false, false) => 'L',
    (true, false, false, true) => 'J',
    (false, false, true, true) => '7',
    (false, true, true, false) => 'F',
    _ => throw new ArgumentOutOfRangeException()
});

PrintMap(map);

var output = 0;
for (var i = 0; i < map.GetLength(1); i++)
{
    var inLoop = LoopBuildingState.OutOfLoop;
    for (var j = 0; j < map.GetLength(0); j++)
    {
        switch (map[j, i], inLoop)
        {
            case ('|', LoopBuildingState.OutOfLoop):
                inLoop = LoopBuildingState.InLoop;
                break;
            case ('|', LoopBuildingState.InLoop):
                inLoop = LoopBuildingState.OutOfLoop;
                break;
            
            case ('F', LoopBuildingState.OutOfLoop):
                inLoop = LoopBuildingState.LoopBelow;
                break;
            case ('7', LoopBuildingState.LoopBelow):
                inLoop = LoopBuildingState.OutOfLoop;
                break;
            case ('J', LoopBuildingState.LoopBelow):
                inLoop = LoopBuildingState.InLoop;
                break;
            
            case ('L', LoopBuildingState.OutOfLoop):
                inLoop = LoopBuildingState.LoopAbove;
                break;
            case ('7', LoopBuildingState.LoopAbove):
                inLoop = LoopBuildingState.InLoop;
                break;
            case ('J', LoopBuildingState.LoopAbove):
                inLoop = LoopBuildingState.OutOfLoop;
                break;
            
            case ('F', LoopBuildingState.InLoop):
                inLoop = LoopBuildingState.LoopAbove;
                break;
            case ('L', LoopBuildingState.InLoop):
                inLoop = LoopBuildingState.LoopBelow;
                break;

            case ('-', LoopBuildingState.LoopAbove):
            case ('-', LoopBuildingState.LoopBelow):
                break;

            case ('.', LoopBuildingState.InLoop):
                output += 1;
                map[j, i] = 'X';
                break;
            case ('.', LoopBuildingState.OutOfLoop):
                break;

            default:
                map[j, i] = '!';
                break;
        }
    }
}

PrintMap(map);

Console.WriteLine(output);

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

void PrintMap(char[,] chars)
{
    for (var i = 0; i < chars.GetLength(1); i++)
    {
        for (var j = 0; j < chars.GetLength(0); j++)
        {
            Console.Write(chars[j, i] switch
            {
                '|' => '\u2503',
                '-' => '\u2501',
                'L' => '\u2517',
                'J' => '\u251b',
                '7' => '\u2513',
                'F' => '\u250f',
                'X' => 'X',
                _ => '.'
            });
        }

        Console.WriteLine();
    }

    Console.WriteLine();
}

static class Exts
{
    public static T? Get<T>(this T[,] source, Point location) where T : struct
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

    public static void Set<T>(this T[,] source, Point location, T value) where T : struct
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

enum LoopBuildingState
{
    InLoop,
    OutOfLoop,
    LoopAbove,
    LoopBelow,
}