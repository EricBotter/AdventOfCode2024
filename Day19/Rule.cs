using System.Text.RegularExpressions;

namespace Day19;

partial record Rule(string Field, Operator Op, int Value, string Destination)
{
    [GeneratedRegex("[0-9]+")]
    private static partial Regex NumberRegex();

    [GeneratedRegex("(?<destination>[a-zAR]+)")]
    public static partial Regex DestinationRuleRegex();

    private static readonly Regex RuleRegex =
        new($"(?<rating>[xmas])(?<op>[<>])(?<value>{NumberRegex}):{DestinationRuleRegex}");

    public static Rule FromString(string line)
    {
        var match = RuleRegex.Match(line);
        return new Rule(
            match.Groups["rating"].Value,
            match.Groups["op"].Value == "<" ? Operator.LessThan : Operator.GreaterThan,
            int.Parse(match.Groups["value"].Value),
            match.Groups["destination"].Value);
    }

    public bool Match(Part part)
    {
        var rangeToCheck = Field switch
        {
            "x" => part.X,
            "m" => part.M,
            "a" => part.A,
            "s" => part.S,
            _ => throw new Exception("mannaggia")
        };

        return Op switch // 0-100 < 90 => 0-89, 90-100 || 0-100 > 90 => 0-90, 91-100 || 0-100 < 100 => 0-99, 100-100
        {
            Operator.LessThan => rangeToCheck.Min < Value && rangeToCheck.Max >= Value,
            Operator.GreaterThan => rangeToCheck.Min <= Value && rangeToCheck.Max > Value,
            _ => throw new Exception("mannaggia2")
        };
    }

    public override string ToString() => $"Rule {{ {Field} {(Op == Operator.LessThan ? '<' : '>')} {Value} => {Destination} }}";
}