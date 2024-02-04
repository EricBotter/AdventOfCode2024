// See https://aka.ms/new-console-template for more information

var lines = File.ReadAllLines("input.txt");

var firstLastChars = from line in lines
    select (first: line.First(char.IsDigit), last: line.Last(char.IsDigit));

var sum = firstLastChars.Select(x => int.Parse(x.first.ToString()) * 10 + int.Parse(x.last.ToString())).Sum();

Console.WriteLine(sum);
