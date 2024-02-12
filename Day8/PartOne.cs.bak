
using System.Text.RegularExpressions;

var regex = new Regex("[A-Z]+");

var lines = File.ReadAllLines("input");

var commands = lines[0];

var map = new Dictionary<string, Destination>();

foreach (var line in lines[2..])
{
    var matches = regex.Matches(line);
    if (matches.Count < 3) break;
    
    var source = matches[0].Value;
    var destination = new Destination(matches[1].Value, matches[2].Value);
    map[source] = destination;
}

var node = "AAA";
var steps = 0;

while (node != "ZZZ")
{
    var direction = commands[steps++ % commands.Length];
    node = direction == 'L' ? map[node].Left : map[node].Right;
}

Console.WriteLine(steps);

record Destination(string Left, string Right);