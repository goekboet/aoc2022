(int x, int y)[] Parse(string s)
{
    var r = new List<(int x, int y)>();
    var i = 0;
    while (i < s.Length)
    {
        var point = new List<char>();
        while (i < s.Length && s[i] != ' ')
        {
            point.Add(s[i++]);
        }
        if (point.Count > 0)
        {
            var ds = new string(point.ToArray()).Split(',');
            r.Add((int.Parse(ds[0]), int.Parse(ds[1])));
        }
        while (i < s.Length && !char.IsDigit(s[i]))
        {
            i++;
        }
    }

    return r.ToArray();
}

IEnumerable<((int x, int y) a, (int x, int y) b)> ToPairs((int x, int y)[] wall)
{
    for (int i = 0; i < wall.Length - 1; i++)
    {
        yield return (wall[i], wall[i + 1]);
    }
}

bool WallCollision(
    ((int x, int y) a, (int x, int y) b) wall,
    (int x, int y) pos
)
{
    if (wall.a.x == wall.b.x)
    {
        var s = Math.Min(wall.a.y, wall.b.y);
        var e = Math.Max(wall.a.y, wall.b.y);

        return pos.y >= s && pos.y <= e && pos.x == wall.a.x;
    }
    else
    {
        var s = Math.Min(wall.a.x, wall.b.x);
        var e = Math.Max(wall.a.x, wall.b.x);

        return pos.x >= s && pos.x <= e && pos.y == wall.a.y;
    }
}

// bool FreeFall(Cave c, (int x, int y) pos)
// {
//     return pos.x < c.bounds.w || pos.x > c.bounds.e || pos.y > c.bounds.s;
// }

bool CaveFull(Cave c, (int x, int y) pos) => c.source == pos;

(int x, int y) Source = (500, 0);

Cave ToCave((int x, int y)[][] ws)
{
    var eps = ws.SelectMany(x => x).ToArray();
    var minX = eps.Select(x => x.x).Min();
    var maxX = eps.Select(x => x.x).Max();
    var maxY = eps.Select(x => x.y).Max();

    var walls = ws.SelectMany(x => ToPairs(x)).ToArray();
    
    return new Cave((minX, maxX, maxY),Source, walls);
}

(int x, int y)[] Fall((int x, int y) pos) => new [] { (pos.x, pos.y + 1),(pos.x - 1,pos.y + 1), (pos.x + 1, pos.y + 1)};

var input = File.ReadAllLines(args[0])
    .Select(Parse)
    .ToArray();

var cave = ToCave(input);
var sand = new HashSet<(int x, int y)>();
var done = false;
var next = cave.source;
while (!done)
{
    var atRest = false;
    while (!atRest && !done)
    {
        (int x, int y)? clear = Fall(next).FirstOrDefault(x => !sand.Contains(x) && !cave.walls.Any(w => WallCollision(w, x)) && x.y < cave.bounds.s + 2);
        if (clear == (0,0))
        {
            atRest = true;
            sand.Add(next);
        }
        else
        {
            next = clear.Value;
        }
        done = CaveFull(cave, next);
    }
    
    next = Source;
}

// void Print(Cave c, HashSet<(int x, int y)> sand)
// {
//     var minX = sand.Select(x => x.x).Min();
//     var maxX = sand.Select(x => x.x).Max();
//     for (int s = Math.Min(c.bounds.w, minX); s <= Math.Max(c.bounds.e, maxX); s++)
//     {
//         if (s == c.source.x)
//         {
//             Console.Write('*');
//         }
//         else
//         {
//             Console.Write(' ');
//         }
//     }
//     Console.WriteLine();
//     for (int y = 0; y <= c.bounds.s + 2; y++)
//     {
//         for (int x = Math.Min(c.bounds.w, minX); x <= Math.Max(c.bounds.e, maxX); x++)
//         {
//             if (sand.Contains((x,y)))
//             {
//                 Console.Write('o');
//                 continue;
//             }
//             var collision = c.walls.Any(w => WallCollision(w, (x,y)));
//             if (collision || y == cave.bounds.s + 2)
//             {
//                 Console.Write('#');
//             }
//             else
//             {
//                 Console.Write('.');
//             }
//         }
//         Console.WriteLine();
//     }
// }

Console.WriteLine(sand.Count());
// Print(cave, sand);

public record Cave(
    (int w, int e, int s) bounds, 
    (int x, int y) source, 
    ((int x, int y) a, (int x, int y) b)[] walls);
