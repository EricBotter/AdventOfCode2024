using System.Text.RegularExpressions;

namespace Day19;

record Workflow(string Name, List<Rule> Rules, string DefaultDestination)
{
    private static readonly Regex WorkflowRegex = new($@"^(?<name>[a-z]+)\{{(?<rules>.*),{Rule.DestinationRuleRegex}\}}$");

    public static Workflow FromString(string line)
    {
        var match = WorkflowRegex.Match(line);

        var rulesToParse = match.Groups["rules"].Value;

        var rules = rulesToParse.Split(',')
            .Select(Rule.FromString)
            .ToList();

        return new Workflow(match.Groups["name"].Value, rules, match.Groups["destination"].Value);
    }

    public IEnumerable<Part> ApplyTo(Part part)
    {
        var remainingPart = part;
        foreach (var rule in Rules)
        {
            Console.WriteLine($"        Evaluating {rule}");
            if (rule.Match(remainingPart))
            {
                var takeLeft = rule.Op == Operator.LessThan;
                switch (rule.Field)
                {
                    case "x":
                        var (leftX, rightX) = remainingPart.X.SplitAt(takeLeft ? rule.Value : rule.Value + 1);
                        yield return remainingPart with { X = takeLeft ? leftX : rightX, CurrentWorkflow = rule.Destination};
                        remainingPart = remainingPart with { X = takeLeft ? rightX : leftX };
                        break;
                    case "m":
                        var (leftM, rightM) = remainingPart.M.SplitAt(takeLeft ? rule.Value : rule.Value + 1);
                        yield return remainingPart with { M = takeLeft ? leftM : rightM, CurrentWorkflow = rule.Destination};
                        remainingPart = remainingPart with { M = takeLeft ? rightM : leftM };
                        break;
                    case "a":
                        var (leftA, rightA) = remainingPart.A.SplitAt(takeLeft ? rule.Value : rule.Value + 1);
                        yield return remainingPart with { A = takeLeft ? leftA : rightA, CurrentWorkflow = rule.Destination};
                        remainingPart = remainingPart with { A = takeLeft ? rightA : leftA };
                        break;
                    case "s":
                        var (leftS, rightS) = remainingPart.S.SplitAt(takeLeft ? rule.Value : rule.Value + 1);
                        yield return remainingPart with { S = takeLeft ? leftS : rightS, CurrentWorkflow = rule.Destination};
                        remainingPart = remainingPart with { S = takeLeft ? rightS : leftS };
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(rule.Field));
                }
            }
        }

        yield return remainingPart with {CurrentWorkflow = DefaultDestination};
    }

    public override string ToString() => $"Workflow {{ [{Name}], {Rules.Count} Rules, Default => [{DefaultDestination}] }}";
}