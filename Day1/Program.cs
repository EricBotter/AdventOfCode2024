
var lines = File.ReadAllLines("input.txt");

var dict = new Dictionary<string, string>
{
    { "one", "1" },
    { "two", "2" },
    { "three", "3" },
    { "four", "4" },
    { "five", "5" },
    { "six", "6" },
    { "seven", "7" },
    { "eight", "8" },
    { "nine", "9" }
};

string NumbersToDigits(string line)
{
    for (var i = 0; i < line.Length; i++)
    {
        if (char.IsDigit(line[i]))
            break;
        
        var (key, value) = dict.FirstOrDefault(pair => line[i..].StartsWith(pair.Key));
        if (key == null) continue;
        
        line = line[..i] + value + line[(i + key.Length)..];
        break;
    }

    for (var i = line.Length - 1; i >= 0; i--)
    {
        if (char.IsDigit(line[i]))
            break;

        var (key, value) = dict.FirstOrDefault(p => line[i..].StartsWith(p.Key));
        if (key == null) continue;
        
        line = line[..i] + value + line[(i + key.Length)..];
        break;
    }

    return line;
}

var firstLastChars = lines
    .Select((Func<string, string>)NumbersToDigits)
    .Select(line => (first: line.First(char.IsDigit), last: line.Last(char.IsDigit)));

var sum = firstLastChars.Select(x => int.Parse(x.first.ToString()) * 10 + int.Parse(x.last.ToString())).Sum();

Console.WriteLine(sum);
