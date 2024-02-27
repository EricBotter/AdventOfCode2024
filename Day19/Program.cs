using System.Text.RegularExpressions;

// ReSharper disable ArrangeTypeModifiers
#pragma warning disable SYSLIB1045

var lines = File.ReadAllLines("input");

var workflowDict = new Dictionary<string, Workflow>();
var parts = new List<Part>();

var i = 0;
while (lines[i].Length > 0)
{
    var workflow = Workflow.FromString(lines[i]);
    workflowDict[workflow.Name] = workflow;
    i++;
}

i++;

var output = 0L;

var partQueue = new Queue<Part>();
partQueue.Enqueue(new Part(new Range(1, 4000), new Range(1, 4000), new Range(1, 4000), new Range(1, 4000), "in"));

while(partQueue.TryDequeue(out var part))
{
    Console.WriteLine($"Current part: {part}");
    var currentWorkflow = workflowDict[part.CurrentWorkflow];
    
    Console.WriteLine($"  Applying {currentWorkflow}");
    var newParts = currentWorkflow.ApplyTo(part);
    
    foreach (var newPart in newParts)
    {
        Console.WriteLine($"    Workflow yielded {newPart}");
        if (newPart.CurrentWorkflow == "A")
        {
            output += newPart.Count;
            Console.WriteLine($" Added {newPart.Count} to global sum, it is now {output}");
        }
        else if (newPart.CurrentWorkflow != "R")
            partQueue.Enqueue(newPart);
    }
}

Console.WriteLine(output);

record Rule(string Field, Operator Op, int Value, string Destination)
{
    public static Rule FromString(string line)
    {
        var match = Program.RuleRegex.Match(line);
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

record Workflow(string Name, List<Rule> Rules, string DefaultDestination)
{
    public static Workflow FromString(string line)
    {
        var match = Program.WorkflowRegex.Match(line);

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
            if (rule.Match(part))
            {
                var takeLeft = rule.Op == Operator.LessThan;
                switch (rule.Field)
                {
                    case "x":
                        var (leftX, rightX) = part.X.SplitAt(takeLeft ? rule.Value : rule.Value + 1);
                        yield return remainingPart with { X = takeLeft ? leftX : rightX, CurrentWorkflow = rule.Destination};
                        remainingPart = remainingPart with { X = takeLeft ? rightX : leftX };
                        break;
                    case "m":
                        var (leftM, rightM) = part.M.SplitAt(takeLeft ? rule.Value : rule.Value + 1);
                        yield return remainingPart with { M = takeLeft ? leftM : rightM, CurrentWorkflow = rule.Destination};
                        remainingPart = remainingPart with { M = takeLeft ? rightM : leftM };
                        break;
                    case "a":
                        var (leftA, rightA) = part.A.SplitAt(takeLeft ? rule.Value : rule.Value + 1);
                        yield return remainingPart with { A = takeLeft ? leftA : rightA, CurrentWorkflow = rule.Destination};
                        remainingPart = remainingPart with { A = takeLeft ? rightA : leftA };
                        break;
                    case "s":
                        var (leftS, rightS) = part.S.SplitAt(takeLeft ? rule.Value : rule.Value + 1);
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

record Part(Range X, Range M, Range A, Range S, string CurrentWorkflow)
{
    public long Count => X.Count * M.Count * A.Count * S.Count;
}

record Range(int Min, int Max)
{
    public (Range left, Range right) SplitAt(int value)
    {
        if (value <= Min || value >= Max)
            throw new ArgumentOutOfRangeException(nameof(value)); 
        return ( this with { Max = value - 1 }, this with { Min = value });
    }

    public long Count => Max - Min + 1;

    public override string ToString() => $"({Min}-{Max})";
}

enum Operator
{
    LessThan,
    GreaterThan
}

partial class Program
{
    public static readonly Regex NumberRegex = new("[0-9]+");
    private static readonly Regex DestinationRuleRegex = new("(?<destination>[a-zAR]+)");

    public static readonly Regex RuleRegex =
        new($"(?<rating>[xmas])(?<op>[<>])(?<value>{NumberRegex}):{DestinationRuleRegex}");

    public static readonly Regex WorkflowRegex = new($@"^(?<name>[a-z]+)\{{(?<rules>.*),{DestinationRuleRegex}\}}$");
}