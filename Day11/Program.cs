var lines = File.ReadAllLines("input");

var galaxies = new List<Galaxy>();

for (var i = 0; i < lines.Length; i++)
{
    for (var j = 0; j < lines[i].Length; j++)
    {
        if (lines[i][j] == '#')
        {
            galaxies.Add(new Galaxy(j, i));
        }
    }
}

var emptyRows = lines.Select((line, i) => (line, i)).Where(tuple => tuple.line.All(c => c == '.')).Select(tuple => tuple.i).ToList();
var emptyCols = Enumerable.Range(0, lines[0].Length).Where(i => lines.All(line => line[i] == '.')).ToList();

var sum = 0L;
foreach (var (a, b) in GalaxyCombinations(galaxies))
{
    var xInterval = new Interval(a.X, b.X);
    var yInterval = new Interval(a.Y, b.Y);

    sum += xInterval.Length + yInterval.Length +
           emptyRows.Count(row => yInterval.IsContained(row)) * 999_999L +
           emptyCols.Count(col => xInterval.IsContained(col)) * 999_999L;
}

Console.WriteLine(sum);
return;


IEnumerable<(Galaxy a, Galaxy b)> GalaxyCombinations(IReadOnlyList<Galaxy> galaxyList)
{
    for (var i = 0; i < galaxyList.Count; i++)
    for (var j = i + 1; j < galaxyList.Count; j++)
        yield return (galaxyList[i], galaxyList[j]);
}

record Galaxy(int X, int Y);

class Interval
{
    public Interval(int start, int end)
    {
        Start = int.Min(start, end);
        End = int.Max(start, end);
    }

    public int Start { get; }
    public int End { get; }

    public int Length => End - Start;

    public bool IsContained(int x) => x > Start && x < End;
}