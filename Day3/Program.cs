using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input.txt");
var numbersRegex = new Regex("[0-9]+");
var symbolsRegex = new Regex("[^0-9.]");

var symbols = new List<Symbol>();

List<Symbol> AdjacentSymbolsForValue(int lineIndex, int beginIndex, int endIndex)
{
    return symbols
        .Where(symbol => symbol.line - lineIndex <= 1 && symbol.line - lineIndex >= -1)
        .Where(symbol => beginIndex <= symbol.pos + 1 && endIndex >= symbol.pos - 1)
        .ToList();
}

for (var index = 0; index < lines.Length; index++)
{
    var line = lines[index];
    foreach (Match match in symbolsRegex.Matches(line))
    {
        symbols.Add(new Symbol(index, match.Index, match.Value[0], []));
    }
}

for (var index = 0; index < lines.Length; index++)
{
    var line = lines[index];
    foreach (Match match in numbersRegex.Matches(line))
    {
        foreach (var symbol in AdjacentSymbolsForValue(index, match.Index, match.Index + match.Length - 1))
        {
            symbol.adjacents.Add(int.Parse(match.Value));
        }
    }
}

var sum = symbols
    .Where(symbol => symbol is { symbol: '*', adjacents.Count: 2 })
    .Sum(symbol => symbol.adjacents[0] * symbol.adjacents[1]);

Console.WriteLine(sum);

record Symbol(int line, int pos, char symbol, List<int> adjacents);
