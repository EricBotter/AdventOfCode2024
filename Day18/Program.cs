using System.Text.RegularExpressions;

// ReSharper disable ArrangeTypeModifiers

var inputs = File.ReadAllLines("input").Select(line => new Input(line)).ToList();

var point = new Location(0, 0);
var area = 0L;

foreach (var input in inputs)
{
    var newPoint = point.Move(input.Direction, input.Length);
    var currentArea = point * newPoint;
    area += currentArea;
    Console.WriteLine($"{input,45}, {point,10} -> {newPoint},    Added: {currentArea,3}, Total: {area,3}");
    point = newPoint;
}

Console.WriteLine(area);

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

    public static long operator *(Location a, Location b)
    {
        var diffX = a.X - b.X;
        var fullCellAdj = diffX switch
        {
            < 0 => -1,
            0 => 0,
            > 0 => 1
        };
        return a.Y * (diffX + fullCellAdj);
    }

    public override string ToString() => $"({X}, {Y})";
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