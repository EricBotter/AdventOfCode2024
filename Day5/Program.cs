using System.Text.RegularExpressions;

var numberRegex = new Regex("[0-9]+");
var parseLineOfNumbers = (string line) =>
    numberRegex.Matches(line).Select(match => match.Value).Select(long.Parse).ToList();

var groups = File.ReadAllText("input").Split("\n\n");

var seeds = parseLineOfNumbers(groups[0])
    .Chunk(2)
    .Select(pair => new SeedRange(pair[0], pair[1]))
    .ToList();

var mapGroups = groups
    .Skip(1)
    .Select(input => input.Trim().Split("\n").Skip(1))
    .Select(lines => lines
        .Select(parseLineOfNumbers)
        .Select(numbers => new NumberMap(numbers[0], numbers[1], numbers[2]))
        .ToList());

foreach (var mapGroup in mapGroups)
{
    var mappedSeeds = new List<SeedRange>();

    var seedsBeingProcessed = new List<SeedRange>(seeds);
    foreach (var map in mapGroup)
    {
        var newUnchangedSeeds = new List<SeedRange>();
        foreach (var seed in seedsBeingProcessed)
        {
            var (unchanged, changed) = map.Map(seed);
            mappedSeeds.AddRange(changed);
            newUnchangedSeeds.AddRange(unchanged);
        }
        seedsBeingProcessed = newUnchangedSeeds;
    }

    seeds = mappedSeeds.Concat(seedsBeingProcessed).Where(seed => seed.Length > 0).ToList();
}

Console.WriteLine(seeds.Select(seed => seed.Start).Min());


record NumberMap(long DestStart, long SourceStart, long Length)
{
    public (List<SeedRange> unchanged, List<SeedRange> changed) Map(SeedRange seedRange)
    {
        if (seedRange.Start + seedRange.Length <= SourceStart || seedRange.Start >= SourceStart + Length)
        {
            return ([seedRange], []);
        }

        if (seedRange.Start <= SourceStart)
        {
            if (seedRange.Start + seedRange.Length <= SourceStart + Length)
            {
                return ([new SeedRange(seedRange.Start, SourceStart - seedRange.Start)],
                    [new SeedRange(DestStart, seedRange.Length - (SourceStart - seedRange.Start))]);
            }

            return (
                [
                    new SeedRange(seedRange.Start, SourceStart - seedRange.Start),
                    new SeedRange(SourceStart + Length, seedRange.Length - Length - (SourceStart - seedRange.Start))
                ],
                [new SeedRange(DestStart, Length)]);
        }

        if (seedRange.Start + seedRange.Length <= SourceStart + Length)
        {
            return ([], [new SeedRange(DestStart + (seedRange.Start - SourceStart), seedRange.Length)]);
        }

        return (
            [new SeedRange(SourceStart + Length, seedRange.Length - (seedRange.Start - SourceStart))],
            [new SeedRange(DestStart + (seedRange.Start - SourceStart), seedRange.Start - SourceStart)]);
    }
}

record SeedRange(long Start, long Length);