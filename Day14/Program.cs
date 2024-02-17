
var lines = File.ReadAllLines("input");

var map = new char[lines[0].Length, lines.Length + 1];

for (int i = 0; i < lines.Length; i++)
{
    map[i, 0] = '#';
}

for (int i = 0; i < lines.Length; i++)
{
    for (int j = 0; j < lines[0].Length; j++)
    {
        map[j, i + 1] = lines[i][j];
    }
}

var rocks = new List<FixedRock>();
for (int x = 0; x < lines[0].Length; x++)
{
    for (int y = 0; y < lines.Length + 1; y++)
    {
        if (map[x, y] == '#')
        {
            var pos = y + 1;
            var count = 0;
            while (pos <= lines.Length && map[x, pos] != '#')
            {
                if (map[x, pos] == 'O')
                {
                    count++;
                }

                pos++;
            }
            rocks.Add(new FixedRock(x, lines.Length + 1 - y, count));
        }
    }
}

Console.WriteLine(rocks.Select(rock => rock.Load).Sum());

record FixedRock(int X, int Y, int Count)
{
    public int Load
    {
        get
        {
            var load = 0;
            for (var i = 0; i < Count; i++)
            {
                load += Y - i - 1;
            }

            return load;
        }
    }
};