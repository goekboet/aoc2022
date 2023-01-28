using System.Text.RegularExpressions;
 
Sensor Parse(string s)
{
    var m = Regex.Match(s, @"Sensor at x=(-?\d+), y=(-?\d+): closest beacon is at x=(-?\d+), y=(-?\d+)");
 
    return new Sensor(
        (int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)),
        (int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value))
    );
}
 
int Distance((int x, int y) a, (int x, int y) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
int BeaconDistance(Sensor s) => Distance(s.pos, s.beacon);
 
bool HasBeaconCloserThan(Sensor s, int y) => BeaconDistance(s) <= Math.Abs(s.pos.y - y);
(int l, int r) BeaconFreeRange(Sensor s, int y)
{
    var dy = BeaconDistance(s) - Math.Abs(s.pos.y - y);
    if (dy > 0)
    {
        var r = (l: s.pos.x - dy, r: s.pos.x + dy);
        if (s.beacon.y == y && s.beacon.x == r.l) return (r.l + 1, r.r);
        if (s.beacon.y == y && s.beacon.x == r.r) return (r.l, r.r - 1); 
        return r;
    }
    else
    {
        return (s.pos.x, s.pos.x);
    }
}
 
// List<HashSet<T>> PowerSet<T>(T[] es)
// {
//     var ps = new List<HashSet<T>>();
//     FindSets<T>(es, ps, new HashSet<T>(), 0);
 
//     return ps;
// }
 
// void FindSets<T>(T[] es, List<HashSet<T>> ps, HashSet<T> s, int p)
// {
//     if (p == es.Length)
//     {
//         if (s.Count > 0)
//         {
//             ps.Add(s);
//         }
        
//         return;
//     }
 
//     FindSets<T>(es, ps, new HashSet<T>(s), p + 1);
//     s.Add(es[p]);
//     FindSets<T>(es, ps, new HashSet<T>(s), p + 1);
// }

// (int l, int r)? Intersect(
//     (int l, int r) a, 
//     (int l, int r) b)
// {
//     if (a.l <= b.l && a.r >= b.l) return (l: b.l, r: Math.Min(a.r, b.r));
//     if (b.l <= a.l && b.r >= a.l) return (l: a.l, r: Math.Min(b.r, a.r));

//     return null;
// }

// (int l, int r)? IntersectMany(IEnumerable<(int l, int r)> rs)
// {
//     (int l, int r)? h = rs.Any() ? rs.First() : null;
//     foreach (var r in rs)
//     {
//         if (h is null) break;
//         h = Intersect(h.Value, r);
//     }

//     return h;
// }

// int Count((int l, int r) x) => 1 + x.r - x.l;

bool OverLap((int l, int r) a, (int l, int r) b)
{
    if (a.l <= b.l && a.r + 1 >= b.l) return true;
    if (b.l <= a.l && b.r + 1 >= a.l) return true;

    return false;
}

(int l, int r)[] Union(
    (int l, int r)[] xs, 
    (int l, int r) x)
{
    var o = xs.Where(s => OverLap(s, x)).ToArray();
    if (o.Any())
    {
        var minL = o.Select(s => s.l).Min();
        var maxR = o.Select(s => s.r).Max();

        return xs.Except(o).Concat(new [] {(l: Math.Min(x.l, minL), r: Math.Max(x.r, maxR))}).ToArray();
    }

    return xs.Concat(new [] { x }).ToArray();
}

(int l, int r)[] Box((int l, int r)[] xs, int low, int high)
{
    var r = new List<(int l, int r)>();
    foreach (var x in xs)
    {
        if (x.r >= low && x.r <= high)
        {
            r.Add((Math.Max(x.l, low), x.r));
        }
        if (x.l >= low && x.l <= high)
        {
            r.Add((x.l, Math.Min(x.r, high)));
        }
    }

    return r.ToArray();
}

List<(int x, int y)> FindCandidates(
    (int l, int r)[] xs, 
    int y)
{
    var r = new List<(int x, int y)>();
    for (int i = 0; i < xs.Length - 1; i++)
    {
        var a = xs[i];
        var b = xs[i + 1];

        if (b.l - a.r == 2)
        {
            r.Add((a.r + 1, y));
        }
    }

    return r;
}

var line = int.Parse(args[1]); 

var beacons = File.ReadAllLines(args[0])
    .Select(Parse)
    .ToArray();

var candidates = new List<(int x, int y)>();
for (int i = 0; i <= line; i++)
{
    var overlaps = beacons
        .Where(x => !HasBeaconCloserThan(x, i))
        .Select(x => BeaconFreeRange(x, i))
        .ToArray();
    var box = Box(overlaps, 0, line).Aggregate(new (int l, int r)[0], Union).ToArray();
    candidates.AddRange(FindCandidates(box, i));
}

var known = beacons.Select(x => x.pos).Union(beacons.Select(x => x.beacon));
var distress = candidates.Except(known).First(); 
var freq = (long)distress.x * 4000000L + (long)distress.y;

Console.WriteLine(freq);

// var overlappingBeacons = 
//     beacons
//     .Where(x => !HasBeaconCloserThan(x, line))
    // .Select(x => BeaconFreeRange(x, line));

// var r = overlappingBeacons
//     .Aggregate(new (int l, int r)[0], Union)
//     .Select(x => Count(x))
    // .Sum();

// Console.WriteLine(r);
 
// var input = File.ReadAllLines(args[0])
//     .Select(Parse)
//     .Select(x => (sensor: x, d: BeaconDistance(x), inRange: !HasBeaconCloserThan(x, 10)))
//     .Where(x => x.inRange)
//     .Select(x => (sensor: x.sensor, range: BeaconFreeRange(x.sensor, 10)))
//     .Select(x => x.range);
 
// Console.WriteLine(input.Count());
// var ps = PowerSet(input.ToArray());
// foreach (var p in ps)
// {
//     Console.WriteLine($"[{string.Join(", ", p)}]");
// }
 
public record Sensor((int x, int y) pos, (int x, int y) beacon);