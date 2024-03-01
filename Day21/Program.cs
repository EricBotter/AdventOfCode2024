
var lines = File.ReadAllLines("input");

var sizeX = lines[0].Length;
var sizeY = lines.Length;

var map = new bool[sizeX, sizeY];
var startingPoint = new Location(0, 0);

for (var i = 0; i < sizeY; i++)
for (var j = 0; j < sizeX; j++)
{
    map[j, i] = lines[i][j] != '#';
    if (lines[i][j] == 'S') 
        startingPoint = new Location(j, i);
}

// todo implement

for (var i = 0; i < sizeY; i++)
{
    for (var j = 0; j < sizeX; j++)
    {
        Console.Write($"{(map[j, i] ? '.' : '#')}");
    }

    Console.WriteLine();
}

Console.WriteLine();

enum Direction
{
    North,
    East,
    South,
    West
}

record Location(int X, int Y)
{
    public Location Move(Direction direction) =>
        direction switch
        {
            Direction.North => this with { Y = Y - 1 },
            Direction.East => this with { X = X + 1 },
            Direction.South => this with { Y = Y + 1 },
            Direction.West => this with { X = X - 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

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
}