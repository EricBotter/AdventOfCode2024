namespace Day19;

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