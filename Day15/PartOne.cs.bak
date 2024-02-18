
var input = File.ReadAllLines("input")[0].Split(',');

var list = input.Select(s => s.Aggregate(0, (i, c) => (i + c) * 17 % 256)).ToList();
var sum = list.Sum();

Console.WriteLine(sum);
