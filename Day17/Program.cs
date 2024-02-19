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

var startingCrucible = new Crucible(0, new Location(0, 0), Direction.South, 0);
var activeCrucibles = new List<Crucible> { startingCrucible };

heatLoss[0, 0] = 0;

while (activeCrucibles.Count > 0)
{
#if DEBUG_PRINT
    Console.WriteLine($"[{DateTime.Now:O}]: Moving crucibles...");
#endif
    var newCrucibles = new List<Crucible>();
    foreach (var crucible in activeCrucibles)
    {
        foreach (var direction in allDirections)
        {
            if (direction == crucible.LastDirection.Opposite())
                continue;
            if (crucible.LastDirectionCount == 7 && crucible.LastDirection == direction)
                continue;

            if (crucible.TryMove(direction, map, out var newCrucible)) 
                newCrucibles.Add(newCrucible);
        }
    }

#if DEBUG_PRINT
    Console.WriteLine($"[{DateTime.Now:O}]: Updating heatmap...");
#endif
    foreach (var newCrucible in newCrucibles)
    {
        if (heatLoss.At(newCrucible.Location) > newCrucible.HeatLoss)
            heatLoss.At(newCrucible.Location) = newCrucible.HeatLoss;
        var value = crucibleHistory.GetValueOrDefault(newCrucible.Footprint, int.MaxValue);
        if (value > newCrucible.HeatLoss)
            crucibleHistory[newCrucible.Footprint] = newCrucible.HeatLoss;
    }

#if DEBUG_PRINT
    Console.WriteLine($"[{DateTime.Now:O}]: Filtering crucibles...");
#endif
    activeCrucibles = newCrucibles.Where(crucible => crucibleHistory[crucible.Footprint] == crucible.HeatLoss)
        .Distinct()
        .ToList();
    
#if DEBUG_PRINT
    Console.WriteLine($"[{DateTime.Now:O}]: Complete!");
    Console.WriteLine($"[{DateTime.Now:O}]:     Active crucibles: {activeCrucibles.Count}");
    Console.WriteLine($"[{DateTime.Now:O}]:     Dictionary key count: {crucibleHistory.Keys.Count}");
    Console.WriteLine(
        heatLoss[sizeX - 1, sizeY - 1] == int.MaxValue
            ? $"[{DateTime.Now:O}]:     heatLoss cells not discovered: {heatLoss.Cast<int>().Count(c => c == int.MaxValue)}"
            : $"[{DateTime.Now:O}]:     Corner value: {heatLoss[sizeX - 1, sizeY - 1]}");
    
    // for (var i = 0; i < sizeY; i++)
    // {
    //     for (var j = 0; j < sizeX; j++)
    //     {
    //         Console.Write($"{heatLoss[j, i].ToString(),10}[{map[j, i]}] ");
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
    public Location Move(Direction direction, int count) =>
        direction switch
        {
            Direction.North => this with { Y = Y - count },
            Direction.East => this with { X = X + count },
            Direction.South => this with { Y = Y + count },
            Direction.West => this with { X = X - count },
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
        };

    public bool IsOob(int maxX, int maxY) => X < 0 || Y < 0 || X >= maxX || Y >= maxY;
}

record Crucible(
    int HeatLoss,
    Location Location,
    Direction LastDirection,
    int LastDirectionCount)
{
    public bool TryMove(Direction direction, byte[,] heatMap, out Crucible newCrucible)
    {
        var sameDirection = direction == LastDirection && LastDirectionCount > 0;

        var newLocation = Location.Move(direction, sameDirection ? 1 : 4);
        if (newLocation.IsOob(heatMap.GetLength(0), heatMap.GetLength(1)))
        {
            newCrucible = this;
            return false;
        }

        var heatIncrease = heatMap.At(newLocation) +
                          (sameDirection
                              ? 0
                              : heatMap.At(Location.Move(direction, 1)) + heatMap.At(Location.Move(direction, 2)) +
                                heatMap.At(Location.Move(direction, 3)));

        newCrucible = new Crucible(HeatLoss + heatIncrease, newLocation, direction,
            sameDirection ? LastDirectionCount + 1 : 1);
        return true;
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