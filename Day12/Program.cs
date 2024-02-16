
using System.Text;
using System.Text.RegularExpressions;

var numberRegex = new Regex("[0-9]+");
var parseLineOfNumbers = (string line) =>
    numberRegex.Matches(line).Select(match => match.Value).Select(int.Parse).ToList();

var lines = File.ReadAllLines("input");

var springRecords = lines.Select(line => new Spring(line.Split(' ')[0], parseLineOfNumbers(line))).ToList();

var sum = springRecords.Select(spring => spring.AllPossibleValidRecords.Count(spring.IsNewRecordValid)).Sum();

Console.WriteLine(sum);

record Spring(string Record, List<int> Damaged)
{
    public IEnumerable<string> AllPossibleValidRecords
    {
        get
        {
            var unknownCount = Record.Count(c => c == '?');
            for (var i = 0; i < Math.Pow(2, unknownCount); i++) // omg exponential complexity
            {
                var builder = new StringBuilder();
                var tentativeReplacement = Convert.ToString(i, 2).PadLeft(unknownCount, '.').Replace('0', '.').Replace('1', '#');
                for (int j = 0, k = 0; j < Record.Length; j++)
                {
                    builder.Append(Record[j] == '?' ? tentativeReplacement[k++] : Record[j]);
                }
                yield return builder.ToString();
            }
        }
    }
    
    public bool IsNewRecordValid(string newRecord)
    {
        var newDamaged = newRecord.Split('.', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Length).ToList();
        return newDamaged.SequenceEqual(Damaged);
    }
};