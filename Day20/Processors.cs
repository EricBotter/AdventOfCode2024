namespace Day20;

internal abstract class Processor
{
    public abstract string Name { get; }

    public abstract IEnumerable<Pulse> Process(Pulse pulse);
}

internal class Broadcaster(string name, IEnumerable<string> destinations) : Processor
{
    public override string Name { get; } = name;
    
    private readonly List<string> _destinations = destinations.ToList();

    public override IEnumerable<Pulse> Process(Pulse pulse) =>
        _destinations.Select(d => pulse with { Source = Name, Destination = d });
}

internal class FlipFlop(string name, IEnumerable<string> destinations) : Processor
{
    public override string Name { get; } = name;

    private readonly List<string> _destinations = destinations.ToList();
    private bool _isOn;

    public override IEnumerable<Pulse> Process(Pulse pulse)
    {
        if (pulse.High)
            return Enumerable.Empty<Pulse>();

        _isOn = !_isOn;
        return _destinations.Select(d => new Pulse(Name, _isOn, d));
    }
}

internal class Conjunction(string name, IEnumerable<string> destinations, IEnumerable<string> sources) : Processor
{
    public override string Name { get; } = name;

    private readonly Dictionary<string, bool> _inputs = sources.ToDictionary(s => s, _ => false);
    private readonly List<string> _destinations = destinations.ToList();

    public override IEnumerable<Pulse> Process(Pulse pulse)
    {
        _inputs[pulse.Source] = pulse.High;
        var allInputsHigh = _inputs.Values.All(value => value);
        return _destinations.Select(d => new Pulse(Name, !allInputsHigh, d));
    }
}

internal class Destination : Processor
{
    public override string Name => "rx";

    public override IEnumerable<Pulse> Process(Pulse pulse) => Enumerable.Empty<Pulse>();
}