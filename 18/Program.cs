(int x, int y, int z)[] GetAdjacency((int x, int y, int z) c) =>
    new []
    {
        (c.x + 1, c.y, c.z),
        (c.x - 1, c.y, c.z),
        (c.x, c.y + 1, c.z),
        (c.x, c.y - 1, c.z),
        (c.x, c.y, c.z + 1),
        (c.x, c.y, c.z - 1)
    };

(int x, int y, int z) Parse(string s)
{
    var ss = s.Split(",");

    return (x: int.Parse(ss[0]), y: int.Parse(ss[1]), z: int.Parse(ss[2]));
}

bool OutOfBounds(Bounds b, (int x, int y, int z) c) => 
    c.x < b.x.l || c.x > b.x.h || c.y < b.y.l || c.y > b.y.h || c.z < b.z.l || c.z > b.z.h;

HashSet<(int x, int y, int z)> FillBubble(
    Bounds b,
    HashSet<(int x, int y, int z)> solid,
    (int x, int y, int z) seed)
{
    var batch = new HashSet<(int x, int y, int z)>();
    var seen = new HashSet<(int x, int y, int z)>();
    batch.Add(seed);
    while (batch.Count > 0)
    {
        var next = new HashSet<(int x, int y, int z)>();
        foreach (var curr in batch)
        {
            if (OutOfBounds(b, curr)) return new HashSet<(int x, int y, int z)>();
            var adj = GetAdjacency(curr).Where(c => !solid.Contains(c) && !seen.Contains(c));
            foreach (var a in adj)
            {
                next.Add(a);
            }
            seen.Add(curr);
        }
        batch = next;
    }

    return seen;
}

var input = new HashSet<(int x, int y, int z)>(File.ReadAllLines(args[0]).Select(Parse).ToArray());

var free = new List<(int x, int y, int z)>();
foreach (var c in input)
{
    var adj = GetAdjacency(c).Where(x => !input.Contains(x));
    foreach (var a in adj)
    {
        free.Add(a);
    }
}

var maxX = input.Select(x => x.x).Max();
var maxY = input.Select(x => x.y).Max();
var maxZ = input.Select(x => x.z).Max();
var minX = input.Select(x => x.x).Min();
var minY = input.Select(x => x.y).Min();
var minZ = input.Select(x => x.z).Min();
var b = new Bounds((minX, maxX), (minY, maxY), (minZ, maxZ));
var voids = new HashSet<(int x, int y, int z)>();
foreach (var f in new HashSet<(int x, int y, int z)>(free))
{
    if (voids.Contains(f)) continue;
    var bubble = FillBubble(b, input, f);
    foreach (var c in bubble)
    {
        voids.Add(c);
    }
}

foreach (var v in voids)
{
    input.Add(v);
}

free.Clear();
foreach (var c in input)
{
    var adj = GetAdjacency(c).Where(x => !input.Contains(x));
    foreach (var a in adj)
    {
        free.Add(a);
    }
}



Console.WriteLine(free.Count);

public record Bounds((int l, int h) x, (int l, int h) y, (int l, int h) z);
