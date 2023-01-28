using System.Collections;
using System.Text.RegularExpressions;

var input = File.ReadAllLines(args[0]);
var lineidx = 0;
var stackLevel = input[lineidx++];
var stackwidth = (stackLevel.Length + 1) / 4;
var stacks = new List<Stack<string>>();
for (int i = 0; i < stackwidth; i++)
{
    stacks.Add(new Stack<string>());
}
var stackEntryPattern = @"\[([A-Z])\]";
while (stackLevel != "")
{
    for (int i = 0; i < stackwidth; i++)
    {
        var entry = stackLevel.Substring(i * 4, 3);
        var m = Regex.Match(entry, stackEntryPattern);
        if (m.Success)
        {
            stacks[i].Push(m.Groups[1].Value);
        }
    }
    stackLevel = input[lineidx++];
}
for (int i = 0; i < stacks.Count; i++)
{
    stacks[i] = new Stack<string>(stacks[i]);
}

var craneMovementPattern = @"move (\d+) from (\d+) to (\d+)";
var craneMovements = new List<CraneMovement>();
while (lineidx < input.Length)
{
    var line = input[lineidx++];
    var m = Regex.Match(line, craneMovementPattern);
    craneMovements.Add(new CraneMovement(
        count: int.Parse(m.Groups[1].Value),
        from: int.Parse(m.Groups[2].Value),
        to: int.Parse(m.Groups[3].Value)));
}

// foreach (var cm in craneMovements)
// {
//     for (int i = 0; i < cm.count; i++)
//     {
//         var item = stacks[cm.from - 1].Pop();
//         stacks[cm.to - 1].Push(item);
//     }
// }

foreach (var cm in craneMovements)
{
    var movement = new List<string>();
    for (int i = 0; i < cm.count; i++)
    {
        movement.Add(stacks[cm.from - 1].Pop());
        // stacks[cm.to - 1].Push(item);
    }
    movement.Reverse();
    foreach (var m in movement)
    {
        stacks[cm.to - 1].Push(m);
    }
}

var r = new List<string>();
foreach (var s in stacks)
{
    r.Add(s.Peek());
}

Console.WriteLine(string.Join("", r));


// foreach (var s in stackInput)
// {
//     var current = 0;
//     while (current < s.Length)
//     {

//     }
// }


public record CraneMovement(int count, int from, int to);