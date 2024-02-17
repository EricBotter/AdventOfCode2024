
var lineClusters = File.ReadAllText("input").Split("\n\n");

var maps = lineClusters.Select(cluster => new Map(cluster.Split('\n', StringSplitOptions.RemoveEmptyEntries))).ToList();

var sum = maps.Select(map => map.Summary).Sum();

Console.WriteLine(sum);

record Map(string[] Lines)
{
    public int Summary
    {
        get
        {
            for (var i = 0; i < Lines.Length - 1; i++)
            {
                var valid = true;
                for (var j = 0; j < i + 1 && j + i + 1 < Lines.Length; j++)
                {
                    if (Lines[i - j] != Lines[i +1 +j])
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    return 100 * (i + 1);
                }
            }

            var lineLength = Lines[0].Length;
            for (var i = 0; i < lineLength - 1; i++)
            {
                var valid = true;
                for (var j = 0; j < i + 1 && j + i + 1 < lineLength; j++)
                {
                    if (!Lines.Select(line => line[i-j]).SequenceEqual(Lines.Select(line => line[i+1+j])))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    return i + 1;
                }
            }

            throw new Exception("allachi");
        }
    }
};
