using System.Text.RegularExpressions;

Motion Parse(string s)
{
    var m = Regex.Match(s, @"([A-Z]) (\d+)");
    return new Motion(m.Groups[1].Value, int.Parse(m.Groups[2].Value));
}

(int x, int y) MoveN((int x, int y) c) => (c.x, c.y - 1);
(int x, int y) MoveE((int x, int y) c) => (c.x + 1, c.y);
(int x, int y) MoveS((int x, int y) c) => (c.x, c.y + 1);
(int x, int y) MoveW((int x, int y) c) => (c.x - 1, c.y);

(int x, int y) MoveHead(string dir, (int x, int y) h)
{
    switch (dir)
    {
        case "U": return MoveN(h);
        case "R": return MoveE(h);
        case "D": return MoveS(h);
        default: return MoveW(h);
    }
}

(int x, int y) MoveTail((int x, int y) t, (int x, int y) h)
{
    var deltaX = h.x - t.x;
    var deltaY = h.y - t.y;
    var newT = t;
    if (Math.Abs(deltaX) > 1 || Math.Abs(deltaY) > 1)
    {
        if (deltaX > 0)
        {
            newT = MoveE(newT);
        }
        if (deltaX < 0)
        {
            newT = MoveW(newT);
        }
        if (deltaY > 0)
        {
            newT = MoveS(newT);
        }
        if (deltaY < 0)
        {
            newT = MoveN(newT);
        }
    }
    
    return newT;
}

var motions = System.IO.File.ReadAllLines(args[0]).Select(Parse);

var visited = new HashSet<(int x, int y)>();
// var head = (0,0);
// var tail = (0,0);
// foreach (var m in motions)
// {
//     for (int i = 0; i < m.steps; i++)
//     {
//         head = MoveHead(m.dir, head);
//         tail = MoveTail(tail, head);
//         visited.Add(tail);
//     }
// }
var rope = new [] {(0,0),(0,0),(0,0),(0,0),(0,0),(0,0),(0,0),(0,0),(0,0),(0,0)};
foreach (var m in motions)
{
    for (int s = 0; s < m.steps; s++)
    {
        rope[0] = MoveHead(m.dir, rope[0]);
        for (int i = 1; i < rope.Length; i++)
        {
            rope[i] = MoveTail(rope[i], rope[i - 1]);
        }
        visited.Add(rope[rope.Length - 1]);
    }
}

// Console.WriteLine(string.Join(", ", visited));
Console.WriteLine(visited.Count);
// 6196 too low

public record Motion(string dir, int steps);
