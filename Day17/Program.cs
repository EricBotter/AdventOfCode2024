var lines = File.ReadAllLines("input");

var sizeX = lines[0].Length;
var sizeY = lines.Length;

var map = new byte[sizeX, sizeY];

var heatLoss = new int[sizeX, sizeY];

for (var i = 0; i < sizeY; i++)
for (var j = 0; j < sizeX; j++)
{
    map[j, i] = byte.Parse(lines[i][j].ToString());
    heatLoss[j, i] = int.MaxValue;
}

var startingCrucible = new Crucible(0, new Location(0, 0), []);
var activeCrucibles = new List<Crucible> { startingCrucible };

heatLoss[0, 0] = 0;

while (activeCrucibles.Count > 0)
{
    var newCrucibles = new List<Crucible>();
    foreach (var crucible in activeCrucibles)
    {
        foreach (var direction in new[] { Direction.North, Direction.East, Direction.South, Direction.West })
        {
            if (crucible.DirectionHistory.Count == 3 &&
                crucible.DirectionHistory.All(crucibleDirection => crucibleDirection == direction))
                continue;

            var newCrucible = crucible.MoveWithoutChangingHeatLoss(direction);
            if (newCrucible.IsOob(sizeX, sizeY))
                continue;

            newCrucibles.Add(newCrucible with { HeatLoss = newCrucible.HeatLoss + map.At(newCrucible.Location) });
        }
    }

    foreach (var newCrucible in newCrucibles)
        if (heatLoss.At(newCrucible.Location) > newCrucible.HeatLoss)
            heatLoss.At(newCrucible.Location) = newCrucible.HeatLoss;

    activeCrucibles = newCrucibles.Where(crucible => crucible.HeatLoss == heatLoss.At(crucible.Location)).ToList(); // optimization is incorrect

    for (var i = 0; i < sizeX; i++)
    {
        for (var j = 0; j < sizeY; j++)
        {
            Console.Write($"{heatLoss[i, j].ToString(),10}[{map[i, j]}] ");
        }

        Console.WriteLine();
    }

    Console.WriteLine();
}

Console.WriteLine(heatLoss[sizeX - 1, sizeY - 1]);

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

record Crucible(int HeatLoss, Location Location, List<Direction> DirectionHistory)
{
    public Crucible MoveWithoutChangingHeatLoss(Direction direction)
    {
        var newDirHist = DirectionHistory.Append(direction).Skip(DirectionHistory.Count == 3 ? 1 : 0).ToList();
        return new Crucible(HeatLoss, Location.Move(direction), newDirHist);
    }

    public bool IsOob(int maxX, int maxY) => Location.IsOob(maxX, maxY);
}

static class Exts
{
    public static ref T At<T>(this T[,] source, Location location) => ref source[location.X, location.Y];
}