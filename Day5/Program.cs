using System.Text.RegularExpressions;

var numberRegex = new Regex("[0-9]+");
var parseLineOfNumbers = (string line) =>
    numberRegex.Matches(line).Select(match => match.Value).Select(long.Parse).ToList();

var groups = File.ReadAllText("input").Split("\n\n");

var seeds = parseLineOfNumbers(groups[0]);
var mapGroups = groups
    .Skip(1)
    .Select(input => input.Trim().Split("\n").Skip(1))
    .Select(lines => lines
        .Select(parseLineOfNumbers)
        .Select(numbers => new NumberMap(numbers[0], numbers[1], numbers[2]))
        .ToList());

foreach (var mapGroup in mapGroups)
{
    for (var i = 0; i < seeds.Count; i++)
    {
        var seed = seeds[i];
        var applicableMap = mapGroup.Where(map => map.Match(seed)).ToList();
        if (applicableMap.Count > 0)
        {
            seeds[i] = applicableMap.First().Map(seed);
        }
        else
        {
            seeds[i] = seed;
        }
    }
}

Console.WriteLine(seeds.Min());


record NumberMap(long DestStart, long SourceStart, long Length)
{
    public bool Match(long input) => input >= SourceStart && input < SourceStart + Length;

    public long Map(long input) => input + (DestStart - SourceStart);
}