#define DEBUG_PRINT

using CrucibleFootprint = (Location location, Direction last, int count);

var allDirections = new[] { Direction.North, Direction.East, Direction.South, Direction.West };

var lines = File.ReadAllLines("input");

var sizeX = lines[0].Length;
var sizeY = lines.Length;

var map = new byte[sizeX, sizeY];

var heatLoss = new int[sizeX, sizeY];

var crucibleHistory = new Dictionary<CrucibleFootprint, int>();

for (var i = 0; i < sizeY; i++)
for (var j = 0; j < sizeX; j++)
{
    map[j, i] = byte.Parse(lines[i][j].ToString());
    heatLoss[j, i] = int.MaxValue;
}

var startingCrucible = new Crucible(0, new Location(0, 0), Direction.South, 0, []);
var activeCrucibles = new List<Crucible> { startingCrucible };

heatLoss[0, 0] = 0;

while (activeCrucibles.Count > 0)
{
    Console.WriteLine($"[{DateTime.Now:O}]: Moving crucibles...");
    var newCrucibles = new List<Crucible>();
    foreach (var crucible in activeCrucibles)
    {
        foreach (var direction in allDirections)
        {
            if (direction == crucible.LastDirection.Opposite())
                continue;
            if (crucible.LastDirectionCount == 3 && crucible.LastDirection == direction)
                continue;

            var newCrucible = crucible.MoveWithoutChangingHeatLoss(direction);
            if (newCrucible.IsOob(sizeX, sizeY) || newCrucible.LocationHistory.Contains(newCrucible.Location))
                continue;

            newCrucibles.Add(newCrucible with { HeatLoss = newCrucible.HeatLoss + map.At(newCrucible.Location) });
        }
    }

    Console.WriteLine($"[{DateTime.Now:O}]: Updating heatmap...");
    foreach (var newCrucible in newCrucibles)
    {
        if (heatLoss.At(newCrucible.Location) > newCrucible.HeatLoss)
            heatLoss.At(newCrucible.Location) = newCrucible.HeatLoss;
        if (crucibleHistory.TryGetValue(newCrucible.Footprint, out var value))
        {
            if (value > newCrucible.HeatLoss)
                crucibleHistory[newCrucible.Footprint] = newCrucible.HeatLoss;
        }
        else
            crucibleHistory[newCrucible.Footprint] = newCrucible.HeatLoss;
    }

    Console.WriteLine($"[{DateTime.Now:O}]: Filtering crucibles...");
    activeCrucibles = newCrucibles.Where(crucible => crucibleHistory[crucible.Footprint] == crucible.HeatLoss)
        .ToList();
    
    Console.WriteLine($"[{DateTime.Now:O}]: Complete!");
    Console.WriteLine($"[{DateTime.Now:O}]:     Active crucibles: {activeCrucibles.Count}");
    Console.WriteLine($"[{DateTime.Now:O}]:     Dictionary key count: {crucibleHistory.Keys.Count}");
    Console.WriteLine(
        heatLoss[sizeX - 1, sizeY - 1] == int.MaxValue
            ? $"[{DateTime.Now:O}]:     heatLoss cells not discovered: {heatLoss.Cast<int>().Count(c => c == int.MaxValue)}"
            : $"[{DateTime.Now:O}]:     Corner value: {heatLoss[sizeX - 1, sizeY - 1]}");
    
#if DEBUG_PRINT
    // for (var i = 0; i < sizeX; i++)
    // {
    //     for (var j = 0; j < sizeY; j++)
    //     {
    //         Console.Write($"{heatLoss[i, j].ToString(),10}[{map[i, j]}] ");
    //     }
    //
    //     Console.WriteLine();
    // }

    Console.WriteLine();

    // foreach (var group in activeCrucibles.GroupBy(c => c.Location))
    //     Console.WriteLine($"{group.Key}: {group.Count()}");
#endif
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

record Crucible(
    int HeatLoss,
    Location Location,
    Direction LastDirection,
    int LastDirectionCount,
    List<Location> LocationHistory)
{
    public Crucible MoveWithoutChangingHeatLoss(Direction direction)
    {
        return new Crucible(HeatLoss, Location.Move(direction), direction,
            direction == LastDirection ? LastDirectionCount + 1 : 1, LocationHistory.Append(Location).ToList());
    }

    public bool IsOob(int maxX, int maxY) => Location.IsOob(maxX, maxY);

    public CrucibleFootprint Footprint => (Location, LastDirection, LastDirectionCount);
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