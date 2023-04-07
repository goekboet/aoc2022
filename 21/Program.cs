using System.Text.RegularExpressions;

Monkey Parse(string s)
{
    var m = Regex.Match(s, @"([^:]+): ([^ ]+) ([+-/*]) ([^ ]+)");
    if (m.Success)
    {
        return new CalculationMonkey
        {
            Id = m.Groups[1].Value,
            Lhs = m.Groups[2].Value,
            Operation = m.Groups[3].Value[0],
            Rhs = m.Groups[4].Value
        };
    }
    m = Regex.Match(s, @"([^:]+): (\d+)");

    return new NumberMonkey { Id = m.Groups[1].Value, Number = int.Parse(m.Groups[2].Value)};
}

long? FindNumber(
    Dictionary<string, Monkey> data, 
    string id)
{
    if (id == "humn") return null;

    var m = data[id];
    if (m is NumberMonkey nm) return nm.Number;
    else if (m is CalculationMonkey cm)
    {
        var lhs = FindNumber(data, cm.Lhs);
        var rhs = FindNumber(data, cm.Rhs);

        if (cm.Operation == '+') return lhs + rhs;
        if (cm.Operation == '-') return lhs - rhs;
        if (cm.Operation == '/') return lhs / rhs;
        return lhs * rhs;
    }

    throw new Exception($"Unuseable monkey {m.Id}"); 
}

long FindHumanNumber(
    Dictionary<string, Monkey> data, 
    string id,
    long target
)
{
    if (id == "humn") return target;

    var m = data[id];
    if (m is CalculationMonkey cm)
    {
        var lhs = FindNumber(data, cm.Lhs);
        var rhs = FindNumber(data, cm.Rhs);

        if (lhs is null && rhs is not null)
        {
            switch (cm.Operation)
            {
                case '+': return FindHumanNumber(data, cm.Lhs, target - rhs.Value);
                case '-': return FindHumanNumber(data, cm.Lhs, target + rhs.Value);
                case '*': return FindHumanNumber(data, cm.Lhs, target / rhs.Value);
                case '/': return FindHumanNumber(data, cm.Lhs, target * rhs.Value);
                default: throw new Exception($"Unexpected operation {cm.Operation}");
            }
        }
        else if (lhs is not null && rhs is null)
        {
            switch (cm.Operation)
            {
                case '+': return FindHumanNumber(data, cm.Rhs, target - lhs.Value);
                case '-': return FindHumanNumber(data, cm.Rhs, lhs.Value - target);
                case '*': return FindHumanNumber(data, cm.Rhs, target / lhs.Value);
                case '/': return FindHumanNumber(data, cm.Rhs, lhs.Value / target);
                default: throw new Exception($"Unexpected operation {cm.Operation}");
            }
        }
        else
        {
            throw new Exception($"rhs and lhs is both unexpectedly null for monkey with id {id}");
        }
    }
    else
    {
        throw new Exception("Unexpectedly got monkey that was a numbermonkey but not humn");
    }
}

var monkeys = File.ReadAllLines(args[0]).Select(Parse).ToDictionary(x => x.Id);
var m = monkeys["root"];
var r = 0L;
if (m is CalculationMonkey root)
{
    var lhs = FindNumber(monkeys, root.Lhs);
    var rhs = FindNumber(monkeys, root.Rhs);

    if (lhs is null && rhs is not null)
    {
        r = FindHumanNumber(monkeys, root.Lhs, rhs.Value);
    }
    else if (lhs is not null && rhs is null)
    {
        r = FindHumanNumber(monkeys, root.Rhs, lhs.Value);
    }
    else
    {
        throw new Exception("Both rhs and lhs is unexpectedly null");
    }
}
else
{
    throw new Exception("monkey root unexpectedly not a calculationmonkey");
}

Console.WriteLine(r);

public abstract class Monkey 
{
    public string Id {get;set;} = "";
}

public class NumberMonkey : Monkey
{
    public int Number {get;set;}
}

public class CalculationMonkey : Monkey
{
    public string Lhs { get;set;} = "";
    public string Rhs {get;set;} = "";
    public char Operation {get;set;}
}
