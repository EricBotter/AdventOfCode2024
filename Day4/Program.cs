using System.Text.RegularExpressions;

var numberRegex = new Regex("[0-9]+");
var lineRegex = new Regex($"^Card *{numberRegex}: (?<winning>[0-9 ]*)\\|(?<our>[0-9 ]*)");

var lines = File.ReadAllLines("input");

var list = lines.Select(line => lineRegex.Match(line))
    .Select(match => (winning: match.Groups["winning"].Value, our: match.Groups["our"].Value))
    .Select(tuple => (winning: numberRegex.Matches(tuple.winning).Select(match => match.Value).Select(int.Parse),
        our: numberRegex.Matches(tuple.our).Select(match => match.Value).Select(int.Parse)))
    .Select(tuple => tuple.winning.Intersect(tuple.our).ToList().Count)
    .ToList();

var stack = Enumerable.Repeat(1, list.Count).ToList();

var sum = 0;
foreach (var winning in list)
{
    var currentCount = stack[0];
    stack.RemoveAt(0);

    for (var i = 0; i < winning && i < stack.Count; i++)
    {
        stack[i] += currentCount;
    }

    sum += currentCount;
}

Console.WriteLine(sum);