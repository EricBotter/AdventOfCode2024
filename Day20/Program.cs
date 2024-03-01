
using Day20;

var lines = File.ReadAllLines("input");

var processors = new Dictionary<string, Processor>();

var parsedLines = new Dictionary<string, string[]>();
foreach (var line in lines)
{
    var split = line.Split("->", StringSplitOptions.TrimEntries);
    var source = split[0];
    var destinations = split[1].Split(",", StringSplitOptions.TrimEntries);

    if (source == "broadcaster")
    {
        processors[source] = new Broadcaster(source, destinations);
        continue;
    }
    
    parsedLines[source] = destinations;
}

processors["rx"] = new Destination();

foreach (var source in parsedLines.Keys)
{
    switch (source[0])
    {
        case '%':
        {
            var name = source[1..];
            processors[name] = new FlipFlop(name, parsedLines[source]);
            break;
        }
        case '&':
        {
            var name = source[1..];
            var sources = parsedLines.Where(pair => pair.Value.Contains(name)).Select(pair => pair.Key[1..]);

            processors[name] = new Conjunction(name, parsedLines[source], sources);
            break;
        }
        default:
            throw new Exception("mannaggia");
    }
}

for (var i = 0;; i++)
{
    if (i % 10000 == 0) 
        Console.WriteLine($"Run # {i}");

    var pulseQueue = new Queue<Pulse>();
    pulseQueue.Enqueue(new Pulse("", false, "broadcaster"));

    while (pulseQueue.TryDequeue(out var pulse))
    {
        // Console.WriteLine($"  Processing {pulse}");

        if (pulse is { High: false, Source: "rx" })
        {
            Console.WriteLine($"Completed! Button press: {i}");
            throw new Exception();
        }

        if (processors.TryGetValue(pulse.Destination, out var value))
            foreach (var newPulse in value.Process(pulse))
                pulseQueue.Enqueue(newPulse);
    }
}
