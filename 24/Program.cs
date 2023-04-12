int mod(int x, int m) {
    return (x%m + m)%m;
}

Map Parse(string[] str)
{
    var map = new Dictionary<int, Dictionary<(int x, int y), List<char>>>();
    var h = str.Length - 2;
    var w = str[0].Length - 2;

    var s = new Dictionary<(int x, int y), List<char>>();
    for (int y = 0; y < h; y++)
    {
        for (int x = 0; x < w; x++)
        {
            var c = (x: x + 1, y: y + 1);
            if (str[c.y][c.x] != '.')
            {
                var v = new [] { str[c.y][c.x] }.ToList();
                s.Add((x, y), v);
            }
        }
    }
    map.Add(0, s);

    return new Map(
        map,
        (0, -1),
        (w - 1, h),
        w, 
        h
    );
}

Map Compute(Map m, int n)
{
    var init = m.map[0];
    var s = new Dictionary<(int x, int y), List<char>>();
    foreach (var (k, vs) in init)
    {
        foreach (var v in vs)
        {
            var c = (x:0, y: 0);
            switch (v)
            {
                case '^':
                    c = (k.x, mod(k.y - n, m.h));
                    break;
                case '>':
                    c = (mod(k.x + n, m.w), k.y);
                    break;
                case 'v':
                    c = (k.x, mod(k.y + n, m.h));
                    break;
                case '<':
                    c = (mod(k.x - n, m.w), k.y);
                    break;
                default: throw new Exception($"Unexpected char in map: {v}");
            }
            if (s.ContainsKey(c))
            {
                s[c].Add(v);
            }
            else
            {
                s.Add(c, new List<char>() { v });
            }
        }
    }
    m.map.Add(n, s);

    return m;
}

List<(int x, int y)> Moves(Map m, ((int x, int y) p, int n, int t) s)
{
    var ms = new List<(int x, int y)>();
    
    var map = m.map[s.n + 1];
    foreach (var n in new [] {(x: s.p.x, y: s.p.y),(x: s.p.x, y: s.p.y - 1),(x: s.p.x + 1, y: s.p.y),(x: s.p.x, y: s.p.y + 1),(x: s.p.x - 1, y: s.p.y)})
    {
        if (n == m.e || n == m.g || (n.x >= 0 && n.x < m.w && n.y >= 0 && n.y < m.h && !map.ContainsKey(n)))
        {
            ms.Add(n);
        }
    }

    return ms;
}

int Distance((int x, int y) a, (int x, int y) b)
{

    return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
}

var str = File.ReadAllLines(args[0]);
var m = Parse(str);
var entrydistance = Distance(m.e, m.g);
var ss = new List<((int x, int y) p, int n, int t)>();
ss.Add((m.e, 0, 0));
int? best = null;
var visited = new HashSet<((int x, int y) p, int n, int t)>();
while (ss.Count > 0)
{
    var s = ss[0];
    ss.RemoveAt(0);
    var g = s.t == 1 ? m.e : m.g;
    if (!m.map.ContainsKey(s.n + 1))
    {
        m = Compute(m, s.n + 1);
    }

    foreach (var mv in Moves(m, s))
    {
        var d = Distance(mv, m.g) + ((2 - s.t) * entrydistance);
        if (d == 0 && (best is null || (s.n + 1) < best))
        {
            best = s.n + 1;
            ss = ss.Where(x => x.n + (Distance(x.p, x.t == 1 ? m.e : m.g) + ((2 - x.t) * entrydistance)) + 1 < best).ToList();
            Console.WriteLine(best);
        }
        else if (mv == g)
        {
            ss.Add((mv, s.n + 1, s.t + 1));
        }
        else if ((best is null || (s.n + d + 1) < best) && !visited.Contains((mv, s.n + 1, s.t)))
        {
            ss.Add((mv, s.n + 1, s.t));
        }
    }

    visited.Add(s);
    ss = ss.Distinct().OrderBy(x => Distance(x.p, x.t == 1 ? m.e : m.g) + ((2 - x.t) * entrydistance)).ToList();
}

Console.WriteLine(best);

public record Map(
    Dictionary<int, Dictionary<(int x, int y), List<char>>> map,
    (int x, int y) e,
    (int x, int y) g,
    int w,
    int h
);
