(int x, int y)[][] shapes = new[]
{
    new [] { (0,0), (1,0), (2,0), (3,0)},
    new [] { (1,0), (0,-1), (1,-1), (2,-1), (1, -2)},
    new [] { (0,0), (1,0), (2,0), (2,-1), (2,-2)},
    new [] { (0,0), (0,-1), (0,-2), (0,-3)},
    new [] { (0,0), (1,0), (0,-1), (1,-1)}
};

(long x, long y)[] ToTiles(ShapeId s, (long x, long y) pos) => shapes[(int)s].Select(x => (pos.x + x.x, pos.y + x.y)).ToArray();

(long x, long y)[] Push(char d, (long x, long y)[] s) =>
    d == '<'
        ? s.Select(x => (x.x - 1, x.y)).ToArray()
        : s.Select(x => (x.x + 1, x.y)).ToArray();

(long x, long y)[] Fall((long x, long y)[] s) =>
    s.Select(x => (x.x, x.y + 1)).ToArray();

int rightWall = 6;

bool Collision((long x, long y)[] s, HashSet<(long x, long y)> ts) => s.Any(x => ts.Contains(x) || x.y > 0 || x.x > rightWall || x.x < 0);

Dictionary<ShapeId, Shape> shapeSet = new Dictionary<ShapeId, Shape>
{
    [ShapeId.HBar] = new Shape(b: new[] { (0, 0), (1, 0), (2, 0), (3, 0) }, t: new[] { (0, 0), (1, 0), (2, 0), (3, 0) }),
    [ShapeId.Cross] = new Shape(b: new[] { (0, -1), (1, 0), (2, -1) }, t: new[] { (0, -1), (1, 0), (2, -1) }),
    [ShapeId.L] = new Shape(b: new[] { (0, 0), (1, 0) }, t: new[] { (0, 0), (1, 0), (2, -2) }),
    [ShapeId.Square] = new Shape(b: new[] { (0, 0), (1, 0) }, t: new[] { (0, -1), (1, -1) })
};

var gas = File.ReadAllText(args[0]);

// var lastHigh = 0;
// var lasti = 0;
// var width = 7;
var high = 0L;
var pos = (2L, -3L);
var shape = ShapeId.HBar;
HashSet<(long x, long y)> tiles = new();
var falling = ToTiles(shape, pos);
var rock = 1L;
var tick = 0;
// var offSet = 0;
var floor = new[] { 0L, 0L, 0L, 0L, 0L, 0L, 0L };
var states = new Dictionary<(ShapeId, int, string), (long rock, long high, long[] floor)>();
var goal = 1000000000000L;
while (true)
{
    var next = Push(gas[tick % gas.Length], falling);
    if (!Collision(next, tiles))
    {
        falling = next;
    }
    next = Fall(falling);
    if (!Collision(next, tiles))
    {
        falling = next;
        tick++;
    }
    else
    {
        foreach (var t in falling)
        {
            tiles.Add(t);
        }

        foreach (var t in falling)
        {
            floor[t.x] = Math.Min(floor[t.x], t.y - 1);
        }
        // var lowFloor = floor.Max();
        // if (lowFloor < 0)
        // {
        //     var ts = tiles.Select(x => (x: x.x, y: x.y - (lowFloor + 1))).Where(x => x.y <= 0);
        //     tiles = new HashSet<(int x, int y)>(ts);
        //     floor = floor.Select(x => x - lowFloor).ToArray();
        //     offSet += lowFloor;
        //     high = 0;
        // }
        high = Math.Min(high, floor.Min());
        shape = (ShapeId)(((int)shape + 1) % 5);
        pos = (2, high - 3);
        falling = ToTiles(shape, pos);
        rock++;
        tick++;
        var k = (shape, tick % gas.Length, NormalizedFloor(floor));
        var v = (rock: rock, high: high, floor = floor.ToArray() );
        if (states.TryGetValue(k, out var pv))
        {
            var pattern = states.Values
                .OrderBy(x => x.rock)
                .SkipWhile(x => x.rock < pv.rock)
                .Concat(new [] { v })
                .ToArray();

            var dH = v.high - pv.high;
            var dS = v.rock - pv.rock;
            var m = (goal - pv.rock) / dS;
            rock = pv.rock + (dS * m);
            high = pv.high + (dH * m);

            var rem = goal - rock;
            var end = pattern[rem + 1];
            high += (end.high - pv.high);
            // 1514285714288 - 1514285714235 = 53
            // 78 - 25 = 53
            break;
        }
        states.Add(k, v);
        // var s = new State(gas[tick % gas.Length], shape, rock, offSet + high, string.Join("-", floor), new HashSet<(int x, int y)>(tiles));
        // var key = ToKey(s);
        // if (states.TryGetValue(key, out var ps))
        // {
        //     var dH = s.h - ps.h;
        //     var dS = s.stone - ps.stone;
        //     var m = (2022 - ps.stone) / dS;
        //     rock = ps.stone + (m * dS);
        //     high = ps.h + (m * dH); 
        // }
        // else
        // {
        //     states.Add(key, s);
        // }

    }
}
// 105
// 2022 - 1957 
// 67 + (18 * 105) + 65;
// 67 + (18 * 160)
//
Console.WriteLine((high) * (-1));
// 1597142857145 too high
// 1594842406882

string NormalizedFloor(long[] floor)
{
    var max = floor.Max();
    return string.Join(" ", floor.Select(x => x - max));
}

// string ToKey(State s) => $"{s.s}-{s.d}-{s.floor}";

public record Shape((int x, int y)[] b, (int x, int y)[] t);
public enum ShapeId { HBar = 0, Cross = 1, L = 2, VBar = 3, Square = 4 }
public record State(int d, ShapeId s, int stone, int h, string floor, HashSet<(int x, int y)> tiles);

