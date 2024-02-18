
var input = File.ReadAllLines("input")[0].Split(',');

var boxes = Enumerable.Repeat(0, 256).Select(_ => new Box([])).ToList();

foreach (var s in input)
{
    if (s.Contains('='))
    {
        var split = s.Split('=');
        var lens = new Lens(split[0], int.Parse(split[1]));
        var currentBox = boxes[lens.Hash];
        
        var indexOfLabel = currentBox.IndexOfLabel(lens.Label);
        if (indexOfLabel == -1)
            currentBox.Lenses.Add(lens);
        else
            currentBox.Lenses[indexOfLabel] = lens;
    }
    else
    {
        var lens = new Lens(s.TrimEnd('-'), 0);
        var currentBox = boxes[lens.Hash];
        var indexOfLabel = currentBox.IndexOfLabel(lens.Label);
        if (indexOfLabel != -1)
            currentBox.Lenses.RemoveAt(indexOfLabel);
    }
}

var sum = boxes.Select((box, i) => box.FocalPower(i)).Sum();
Console.WriteLine(sum);

record Lens(string Label, int FocalLength)
{
    public int Hash => Label.Aggregate(0, (i, c) => (i + c) * 17 % 256);
}

record Box(List<Lens> Lenses)
{
    public int IndexOfLabel(string label) =>
        Lenses.Select((lens, i) => (lens.Label, i)).FirstOrDefault(tuple => tuple.Item1.Equals(label), ("", -1)).Item2;

    public long FocalPower(int index) => Lenses.Select((lens, i) => (index + 1L) * (i + 1) * lens.FocalLength).Sum();
};