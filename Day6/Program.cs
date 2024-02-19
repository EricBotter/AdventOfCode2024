
using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input");

var numberRegex = new Regex("[0-9]+");
var parseLineOfNumbers = (string line) =>
    numberRegex.Matches(line).Select(match => match.Value).Select(long.Parse).ToList();

var racesToGroup = lines.Select(line => line.Replace(" ", "")).Select(parseLineOfNumbers).ToList();
var races = racesToGroup[0].Zip(racesToGroup[1]).Select(zip => new Race(zip.First, zip.Second)).ToList();

var ways = 0;
foreach (var race in races)
{
    for (var i = 0; i < race.Time; i++)
    {
        var distance = i * (race.Time - i);
        if (distance > race.RecordDistance)
        {
            ways++;
        }
    }
}

Console.WriteLine(ways);

record Race(long Time, long RecordDistance);