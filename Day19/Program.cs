using Day19;
using Range = Day19.Range;

var lines = File.ReadAllLines("input");

var workflowDict = new Dictionary<string, Workflow>();

var i = 0;
while (lines[i].Length > 0)
{
    var workflow = Workflow.FromString(lines[i]);
    workflowDict[workflow.Name] = workflow;
    i++;
}

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
            output += newPart.Count;
        else if (newPart.CurrentWorkflow != "R")
            partQueue.Enqueue(newPart);
    }
}

Console.WriteLine(output);
