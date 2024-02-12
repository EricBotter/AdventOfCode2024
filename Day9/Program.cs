
var lines = File.ReadAllLines("input");

var numberLists = lines.Select(line => line.Split(" ").Select(int.Parse).ToList()).ToList();

var output = 0L;
foreach (var numberList in numberLists)
{
    var listHistory = new List<List<int>> {numberList};

    while (listHistory[^1].Any(number => number != 0))
    {
        var currentList = listHistory[^1];
        var newList = new List<int>();
        var previous = currentList[0];
        foreach (var number in currentList[1..])
        {
            newList.Add(number - previous);
            previous = number;
        }
        
        listHistory.Add(newList);
    }

    output += listHistory.Select(list => list[^1]).Sum();
}

Console.WriteLine(output);
