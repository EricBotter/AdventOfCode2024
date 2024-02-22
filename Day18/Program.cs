using System.Text.RegularExpressions;

// ReSharper disable ArrangeTypeModifiers

var inputs = File.ReadAllLines("input").Select(line => new Input(line)).ToList();

int maxX = 0, maxY = 0, minX = 0, minY = 0;

var point = new Location(0, 0);
foreach (var input in inputs)
{
    point = point.Move(input.Direction, input.Length);
    maxX = int.Max(maxX, point.X);
    minX = int.Min(minX, point.X);
    maxY = int.Max(maxY, point.Y);
    minY = int.Min(minY, point.Y);
}

var sizeX = maxX - minX + 1;
var sizeY = maxY - minY + 1;

var startingPoint = new Location(-minX, -minY);
var map = new char[sizeX, sizeY];

for (var i = 0; i < sizeX; i++)
for (var j = 0; j < sizeY; j++)
    map[i, j] = '.';

var currentLocation = startingPoint;

var firstCharToSet = inputs[0].Direction switch
{
    Direction.North or Direction.South => '|',
    Direction.East or Direction.West => '-',
    _ => throw new ArgumentOutOfRangeException()
};

foreach (var location in currentLocation.StepsTowards(inputs[0].Direction, inputs[0].Length))
    map.At(location) = firstCharToSet;

currentLocation = currentLocation.Move(inputs[0].Direction, inputs[0].Length);
var lastDirection = inputs[0].Direction;

foreach (var input in inputs.Skip(1))
{
    map.At(currentLocation) = (lastDirection, input.Direction) switch
    {
        (Direction.North, Direction.East) => 'F',
        (Direction.South, Direction.East) => 'L',
        (Direction.North, Direction.West) => '7',
        (Direction.South, Direction.West) => 'J',
        (Direction.East, Direction.North) => 'J',
        (Direction.West, Direction.North) => 'L',
        (Direction.East, Direction.South) => '7',
        (Direction.West, Direction.South) => 'F',
        _ => throw new Exception("momento Hoobastank: Same Direction")
    };

    var charToSet = input.Direction switch
    {
        Direction.North or Direction.South => '|',
        Direction.East or Direction.West => '-',
        _ => throw new ArgumentOutOfRangeException()
    };

    foreach (var location in currentLocation.StepsTowards(input.Direction, input.Length))
    {
        map.At(location) = charToSet;
    }

    currentLocation = currentLocation.Move(lastDirection = input.Direction, input.Length);
}

map.At(currentLocation) = (lastDirection, inputs[0].Direction) switch
{
    (Direction.North, Direction.East) => 'F',
    (Direction.South, Direction.East) => 'L',
    (Direction.North, Direction.West) => 'J',
    (Direction.South, Direction.West) => '7',
    (Direction.East, Direction.North) => 'J',
    (Direction.West, Direction.North) => 'L',
    (Direction.East, Direction.South) => '7',
    (Direction.West, Direction.South) => 'F',
    _ => throw new Exception("momento Hoobastank: Same Direction")
};


#region adapted copy-paste from Day 10

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
            case ('.', LoopBuildingState.OutOfLoop):
                break;

            default:
                throw new Exception("map is borked");
        }

        if (inLoop != LoopBuildingState.OutOfLoop || map[j, i] != '.')
        {
            map[j, i] = 'X';
            output += 1;
        }
    }
}

#endregion adapted copy-paste from Day 10

Console.WriteLine(output);

record Input(Direction Direction, int Length)
{
    public Input(string line) : this(line[0].ToDir(), int.Parse(Program.NumberRegex().Match(line).Value))
    {
    }
}

static partial class Program
{
    [GeneratedRegex("[0-9]+")]
    public static partial Regex NumberRegex();
}

enum Direction
{
    North,
    East,
    South,
    West
}

record Location(int X, int Y)
{
    public Location Move(Direction direction, int count) =>
        direction switch
        {
            Direction.North => this with { Y = Y - count },
            Direction.East => this with { X = X + count },
            Direction.South => this with { Y = Y + count },
            Direction.West => this with { X = X - count },
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    public IEnumerable<Location> StepsTowards(Direction direction, int count)
    {
        var current = this;
        for (var i = 0; i < count; i++)
        {
            current = current.Move(direction, 1);
            yield return current;
        }
    }

    public bool IsOob(int maxX, int maxY) => X < 0 || Y < 0 || X >= maxX || Y >= maxY;
}

static class Exts
{
    public static ref T At<T>(this T[,] source, Location location) => ref source[location.X, location.Y];

    public static Direction Opposite(this Direction direction) =>
        direction switch
        {
            Direction.North => Direction.South,
            Direction.East => Direction.West,
            Direction.South => Direction.North,
            Direction.West => Direction.East,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    public static Direction ToDir(this char c) =>
        c switch
        {
            'U' => Direction.North,
            'R' => Direction.East,
            'D' => Direction.South,
            'L' => Direction.West,
            _ => throw new ArgumentOutOfRangeException()
        };
}

enum LoopBuildingState
{
    InLoop,
    OutOfLoop,
    LoopAbove,
    LoopBelow,
}