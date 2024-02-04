using System.Text.RegularExpressions;

var numberPattern = new Regex("[0-9]+");
var gamePattern =
    new Regex($"(?<red>{numberPattern} red)|(?<green>{numberPattern} green)|(?<blue>{numberPattern} blue)");

var lines = File.ReadAllLines("input.txt");

var colors = new List<string> { "red", "green", "blue" };

var sum = lines.Sum(line => line.Split(':')[1]
    .Split(';')
    .SelectMany(round => gamePattern.Matches(round))
    .Select(match => match.Groups.Values.First(group => colors.Contains(group.Name) && group.Success))
    .GroupBy(group => group.Name,
        (_, values) => values.Select(value => int.Parse(numberPattern.Match(value.Value).Value)).Max())
    .Aggregate(1, (count1, count2) => count1 * count2));

Console.WriteLine(sum);
