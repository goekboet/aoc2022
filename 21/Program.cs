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

long FindNumber(Dictionary<string, Monkey> data, string id)
{
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

var monkeys = File.ReadAllLines(args[0]).Select(Parse).ToDictionary(x => x.Id);
var r = FindNumber(monkeys, "root");

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
