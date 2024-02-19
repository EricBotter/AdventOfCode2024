
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

var nodes = map.Keys.Where(node => node[2] == 'A').Select(node => new Node(node, 0)).ToList();

foreach (var node in nodes)
{
    var steps = 0;
    while (node.Name[2] != 'Z')
    {
        var direction = commands[steps++ % commands.Length];
        node.Name = direction == 'L' ? map[node.Name].Left : map[node.Name].Right;
        node.Steps++;
    }
}

var list = nodes.Select(node => node.Steps).ToList();

Console.WriteLine(Lcm(list));
return;

long Lcm(IEnumerable<long> numbers)
{
    return numbers.Aggregate(LcmAggregator);
    
    long LcmAggregator(long a, long b)
    {
        return Math.Abs(a * b) / Gcd(a, b);

        long Gcd(long x, long y)
        {
            while (true)
            {
                if (y == 0) return x;
                var a1 = x;
                x = y;
                y = a1 % y;
            }
        }
    }
}

record Destination(string Left, string Right);

class Node(string name, long steps)
{
    public string Name { get; set; } = name;
    public long Steps { get; set; } = steps;
}
