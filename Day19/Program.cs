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

while (i < lines.Length)
{
    parts.Add(Part.FromString(lines[i]));
    i++;
}

var output = 0;
foreach (var part in parts)
{
    Console.WriteLine($"Current part: {part}");
    var currentWorkflow = workflowDict["in"];
    var destination = currentWorkflow.DestinationFor(part);
    Console.WriteLine($"    {currentWorkflow} sent part to {destination}");
    while (destination != "A" && destination != "R")
    {
        currentWorkflow = workflowDict[destination];
        destination = currentWorkflow.DestinationFor(part);
        Console.WriteLine($"    {currentWorkflow} sent part to {destination}");
    }

    if (destination == "A")
    {
        Console.WriteLine($"  Part is rated {part.X + part.M + part.A + part.S}");
        output += part.X + part.M + part.A + part.S;
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
        var valueToCheck = Field switch
        {
            "x" => part.X,
            "m" => part.M,
            "a" => part.A,
            "s" => part.S,
            _ => throw new Exception("mannaggia")
        };

        return Op switch
        {
            Operator.LessThan => valueToCheck < Value,
            Operator.GreaterThan => valueToCheck > Value,
            _ => throw new Exception("mannaggia2")
        };
    }
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

    public string DestinationFor(Part part)
    {
        foreach (var rule in Rules)
        {
            Console.WriteLine($"        Evaluating {rule}");
            if (rule.Match(part))
            {
                return rule.Destination;
            }
        }

        return DefaultDestination;
    }
}

record Part(int X, int M, int A, int S)
{
    public static Part FromString(string line)
    {
        var matches = Program.NumberRegex.Matches(line);
        return new Part(
            int.Parse(matches[0].Value),
            int.Parse(matches[1].Value),
            int.Parse(matches[2].Value),
            int.Parse(matches[3].Value)
        );
    }
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