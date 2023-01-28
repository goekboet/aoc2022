using System.Numerics;
using System.Text.RegularExpressions;

const string monkeyPattern = @"Monkey (\d+):
  Starting items: (.+)
  Operation: new = old ([+|*]) (.+)
  Test: divisible by (\d+)
    If true: throw to monkey (\d+)
    If false: throw to monkey (\d+)";

Monkey Parse(string s)
{
    var m = Regex.Match(s, monkeyPattern.Replace("\r\n", "\n"));
    if (!m.Success) throw new Exception($"Failed to parse {s}");

    var id = int.Parse(m.Groups[1].Value);
    var items = m.Groups[2].Value.Split(", ").Select(x => long.Parse(x)).ToList();
    var op = m.Groups[3].Value;
    int? arg = null;
    var argCapture = m.Groups[4].Value;
    if (argCapture != "old")
    {
        arg = int.Parse(m.Groups[4].Value);
    }
    var test = int.Parse(m.Groups[5].Value);
    var trueMonkey = int.Parse(m.Groups[6].Value);
    var falseMonkey = int.Parse(m.Groups[7].Value);

    return new Monkey(id, items, new Operation(op, arg), test, trueMonkey, falseMonkey);
}

var currentState = System.IO.File
  .ReadAllText(args[0])
  .Split("\n\n")
  .Select(Parse)
  .ToDictionary(
      x => x.id, 
      x => x
    );

// var nextState = new Dictionary<int, Monkey>();
var modulo = currentState.Select(x => x.Value.test).Aggregate((s, x) => s * x);
var business = new int[currentState.Count];
var targetRound = 10000;
for (int i = 0; i < targetRound; i++)
{
    if (i != 0 && (i == 1 || i == 20 || (i % 1000 == 0)))
    {
        Console.WriteLine($"== After round {i} ==");
        for (int b = 0; b < business.Length; b++)
        {
            Console.WriteLine($"Monkey {b} inspected items {business[b]} times.");
        }
        Console.WriteLine();
    }
    // foreach (var (k,v) in currentState)
    // {
    //     nextState[k] = v with { items = new List<int>() };
    // }

    foreach (var k in currentState.Keys)
    {
        var m = currentState[k];
        foreach (var item in m.items)
        {
            var newLevel = 0L;
            if (m.op.op == "*")
            {
                newLevel = ((item * (m.op.arg ?? item)) % modulo);
            }
            else
            {
                newLevel = ((item + (m.op.arg ?? item)) % modulo);
            }
            if (newLevel % m.test == 0)
            {
                currentState[m.trueMonkey].items.Add(newLevel);
            }
            else
            {
                currentState[m.falseMonkey].items.Add(newLevel);
            }

            business[m.id]++;
        }
        m.items.Clear();
    }
    // currentState = nextState.ToDictionary(x => x.Key, x => x.Value);
}

var r = business.OrderDescending().Take(2).ToArray();
Console.WriteLine((long)r[0] * (long)r[1]);

// 1000: 15387
// Console.WriteLine(string.Join("\n\n", monkeys.Select(x => x.ToString())));

public record Operation(string op, int? arg);

public record Monkey(
    int id, 
    List<long> items,
    Operation op,
    int test,
    int trueMonkey,
    int falseMonkey);
