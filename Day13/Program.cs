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
                var singleCharUsed = false;
                for (var j = 0; j < i + 1 && j + i + 1 < Lines.Length; j++)
                {
                    var a = Lines[i - j];
                    var b = Lines[i + 1 + j];
                    if (a == b) continue;
                    
                    if (singleCharUsed)
                    {
                        valid = false;
                        break;
                    }
                    if (a.OneCharDiffers(b))
                    {
                        singleCharUsed = true;
                        continue;
                    }
                    valid = false;
                    break;
                }

                if (valid && singleCharUsed)
                {
                    return 100 * (i + 1);
                }
            }

            var lineLength = Lines[0].Length;
            for (var i = 0; i < lineLength - 1; i++)
            {
                var valid = true;
                var singleCharUsed = false;
                for (var j = 0; j < i + 1 && j + i + 1 < lineLength; j++)
                {
                    var a = Lines.Select(line => line[i - j]).CharsToString();
                    var b = Lines.Select(line => line[i + 1 + j]).CharsToString();
                    if (a == b) continue;
                    
                    if (singleCharUsed)
                    {
                        valid = false;
                        break;
                    }
                    if (a.OneCharDiffers(b))
                    {
                        singleCharUsed = true;
                        continue;
                    }
                    
                    valid = false;
                    break;
                }

                if (valid && singleCharUsed)
                {
                    return i + 1;
                }
            }

            throw new Exception("allachi");
        }
    }
};

static class Exts
{
    public static string CharsToString(this IEnumerable<char> chars)
    {
        return chars.Aggregate("", (s, c) => s + c);
    }

    public static bool OneCharDiffers(this string a, string b)
    {
        var charFound = false;
        for (var i = 0; i < a.Length; i++)
        {
            if (a[i] == b[i]) continue;
            if (charFound)
                return false;

            charFound = true;
        }

        return charFound;
    }
}