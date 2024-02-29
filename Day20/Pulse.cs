namespace Day20;

internal record Pulse(string Source, bool High, string Destination)
{
    public override string ToString() => $"Pulse: {Source} -{(High ? "high" : "low")}-> {Destination}";
}