
using System.Text.RegularExpressions;

var numberPattern = new Regex("[0-9]+");
var gamePattern = new Regex($"(?<red>{numberPattern} red)|(?<green>{numberPattern} green)|(?<blue>{numberPattern} blue)");

var lines = File.ReadAllLines("input.txt");

var limitsByColor = new Dictionary<string, int>()
{
    { "red", 12 }, { "green", 13 }, { "blue", 14 }
};

var sum = 0;

foreach (var line in lines)
{
    var split = line.Split(':');

    var index = int.Parse(numberPattern.Match(split[0]).Value);
    
    var throws = split[1].Split(';');
    var selectMany = throws.SelectMany(round => gamePattern.Matches(round));

    var allGroupsInLimits = selectMany
        .Select(match => match.Groups.Values.First(group => limitsByColor.ContainsKey(group.Name) && group.Success))
        .All(group => int.Parse(numberPattern.Match(group.Value).Value) <= limitsByColor[group.Name]);
    if (allGroupsInLimits)
    {
        sum += index;
    }
}

Console.WriteLine(sum);