using System.Text.RegularExpressions;

var lines = File.ReadAllLines("input");

var lineRegex = new Regex("(?<hand>[2-9TJQKA]{5}) (?<bid>[0-9]+)");

var games = lines.Select(line => lineRegex.Matches(line)[0])
    .Select(match => new Game(match.Groups["hand"].Value, int.Parse(match.Groups["bid"].Value)))
    .ToList();

games.Sort();

var output = games.Select((game, i) => game.Bid * (i + 1L)).ToList();

Console.WriteLine(output.Sum());

class Game : IComparable<Game>
{
    private string Hand { get; }
    private string ComparisonString { get; }
    public int Bid { get; }

    public Game(string hand, int bid)
    {
        Hand = hand;
        Bid = bid;
        ComparisonString = HandComparisonString();
    }

    public int CompareTo(Game? other)
    {
        if (other == null) return 1;
        var compare = string.Compare(ComparisonString, other.ComparisonString, StringComparison.Ordinal);
        if (compare != 0)
            return compare;

        for (var i = 0; i < 5; i++)
        {
            var i1 = "AKQJT98765432".IndexOf(Hand[i]);
            var i2 = "AKQJT98765432".IndexOf(other.Hand[i]);
            if (i1 != i2)
                return i2 - i1;
        }

        return 0;
    }

    private string HandComparisonString()
    {
        var list = Enumerable.Repeat(0, 13).ToList();
        foreach (var c in Hand)
        {
            int index = "AKQJT98765432".IndexOf(c);
            list[index]++;
        }
        list.Sort();
        list.Reverse();
        return string.Join("", list.Where(x => x > 0));
    }
}