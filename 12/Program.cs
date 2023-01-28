int GetElevation(char c)
{
    if (c == 'S') return 1;
    if (c == 'E') return 26;

    return (int)c - 96;
}

Map Parse(string[] ls)
{
    var width = ls[0].Length;
    var height = ls.Length;

    var nodes = new Dictionary<(int x, int y), (char c, int e)>();
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            var c = ls[y][x];
            nodes.Add((x, y), (c, GetElevation(c)));
        }
    }

    return new Map(width, height, nodes);
}

(int x, int y)[] GetAdjecency((int x, int y) c) => new [] {(c.x, c.y + 1),(c.x + 1, c.y),(c.x, c.y - 1),(c.x - 1, c.y)};
bool OnMap(Map m, (int x, int y) c) => c.x >= 0 && c.x < m.width && c.y >= 0 && c.y < m.height;

int Solve1(Map m, (int x, int y) s)
{
    var unvisited = new List<((int x, int y) c, int e, int d, bool goal)>();
    unvisited.Add((s, 1, 0, false));
    var visited = new HashSet<(int x, int y)>();
    while (unvisited.Any())
    {
        var consider = unvisited.MinBy(x => x.d);
        unvisited.Remove(consider);

        if (consider.goal)
        {
            return consider.d;
        }

        // if (unvisited.Count > 421)
        // {
        //     Console.WriteLine(consider);
        // }

        var next = GetAdjecency(consider.c)
            .Where(x => OnMap(m, x) && !visited.Contains(x))
            .Select(x => (c: x, node: m.nodes[x]))
            .Where(x => {
                var diff =  x.node.e - consider.e;
                return diff <= 1;
            });

        foreach (var n in next)
        {
            var e = unvisited.FindIndex(x => x.c == n.c);
            if (e == -1)
            {
                unvisited.Add((n.c, n.node.e, consider.d + 1, n.node.c == 'E'));
            }
            else
            {
                var ex = unvisited[e];
                if (ex.d > consider.d + 1)
                {
                    unvisited[e] = (n.c, n.node.e, consider.d + 1, n.node.c == 'E');
                }
            }
            // unvisited.Enqueue((n.c, n.node.e, consider.d + 1, n.node.c == 'E'));
        }
        visited.Add(consider.c);
        // Console.WriteLine($"visited: {visited.Count} queue: {unvisited.Count}");
    }

    return -1;
}

var ls = File.ReadAllLines(args[0]);
var map = Parse(ls);
var ds = map.nodes
    .Where(x => x.Value.e == 1)
    .Select(x => (c: x, d: Solve1(map, x.Key)));

var r = ds.Where(x => x.d != -1).MinBy(x => x.d);
Console.WriteLine(r.d);


public record Map(int width, int height, Dictionary<(int x, int y), (char c, int e)> nodes);
