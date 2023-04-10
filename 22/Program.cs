int faceWidth = 50;
int faceHeight = 50;

(int face, (int x, int y) offset)[] Offsets = new []
{
    (1, ( 50,  0)),
    (2, (100,  0)),
    (3, ( 50, 50)),
    (4, (  0,100)),
    (5, ( 50,100)),
    (6, (  0,150)),
};

Map Parse(string str)
{
    (Stack<int> dists, Stack<char> dirs) MapDirections(
        string s)
    {
        var dists = new List<int>();
        var dirs = new List<char>();

        var nextNumber = "";
        foreach (var c in s)
        {
            if (char.IsDigit(c))
            {
                nextNumber += c;
            }
            else
            {
                dists.Add(int.Parse(nextNumber));
                dirs.Add(c);
                nextNumber = "";
            }
        }
        dists.Add(int.Parse(nextNumber));

        dists.Reverse();
        dirs.Reverse();

        return (
            new Stack<int>(dists),
            new Stack<char>(dirs)
        );
    }

    var parts = str.Split("\n\n");
    var map = parts[0].Split("\n");
    
    var (dists, dirs) = MapDirections(parts[1]);

    var faces = new Dictionary<int, Face>();
    foreach (var o in Offsets)
    {
        var walls = new HashSet<(int x, int y)>();
        for (int y = 0; y < faceHeight; y++)
        {
            for (int x = 0; x < faceWidth; x++)
            {
                var c = (x: x + o.offset.x, y: y + o.offset.y);
                var v = map[c.y][c.x];
                if (v == '#')
                {
                    walls.Add((x, y));
                }
            }
        }
        var face = new Face(o.offset, walls);
        faces.Add(o.face, face);
    }

    return new Map(
        faces,
        dists,
        dirs
    );
}

(State s, bool legal) Next(
    Map m,
    State s
)
{
    (int face, char d, (int x, int y) c) Wrap(
        int face,
        char d,
        (int x, int y) c
    )
    {
        switch ((face, d))
        {
            case (1, 'N'): return (6, 'E', (            0, c.x                 ));
            case (1, 'E'): return (2, 'E', (            0, c.y                 ));
            case (1, 'S'): return (3, 'S', (          c.x,   0                 ));
            case (1, 'W'): return (4, 'E', (            0, faceHeight - c.y - 1));
            case (2, 'N'): return (6, 'N', (          c.x, faceHeight          ));
            case (2, 'E'): return (5, 'W', (faceWidth - 1, faceHeight - c.y - 1));
            case (2, 'S'): return (3, 'W', (faceWidth - 1, c.x                 ));
            case (2, 'W'): return (1, 'W', (faceWidth - 1, c.y                 ));
            case (3, 'N'): return (1, 'N', (          c.x, faceHeight - 1      ));
            case (3, 'E'): return (2, 'N', (          c.y, faceHeight - 1      ));
            case (3, 'S'): return (5, 'S', (          c.x, 0                   ));
            case (3, 'W'): return (4, 'S', (          c.y, 0                   ));
            case (4, 'N'): return (3, 'E', (            0, c.x                 ));
            case (4, 'E'): return (5, 'E', (            0, c.y                 ));
            case (4, 'S'): return (6, 'S', (          c.x, 0                   ));
            case (4, 'W'): return (1, 'E', (            0, faceHeight - c.y - 1));
            case (5, 'N'): return (3, 'N', (          c.x, faceHeight - 1      ));
            case (5, 'E'): return (2, 'W', (faceWidth - 1, faceHeight - c.y - 1));
            case (5, 'S'): return (6, 'W', (faceWidth - 1, c.x                 ));
            case (5, 'W'): return (4, 'W', (faceWidth - 1, c.y                 ));
            case (6, 'N'): return (4, 'N', (          c.x, faceHeight - 1      ));
            case (6, 'E'): return (5, 'N', (          c.y, faceHeight - 1      ));
            case (6, 'S'): return (2, 'S', (          c.x, 0                   ));
            case (6, 'W'): return (1, 'S', (          c.y, 0                   ));
            default: throw new Exception($"Invalid face/direction {face}/{d}");
        }
    }

    var next = (x: 0, y: 0);
    var legal = false;
    switch (s.dir)
    {
        case 'N':
            next = (x: s.c.x, y: s.c.y - 1);
            if (next.y < 0)
            {
                var (f, d, n) = Wrap(s.face, s.dir, s.c);
                s = s with { face = f, c = n, dir = d };
            }
            else
            {
                s = s with { c = next };
            }
            legal = !m.Faces[s.face].walls.Contains(s.c);
            return (s, legal);
        case 'E':
            next = (x: s.c.x + 1, y: s.c.y);
            if (next.x == faceWidth)
            {
                var (f, d, n) = Wrap(s.face, s.dir, s.c);
                s = s with { face = f, c = n, dir = d };
            }
            else
            {
                s = s with { c = next };
            }
            legal = !m.Faces[s.face].walls.Contains(s.c);
            return (s, legal);
        case 'S':
            next = (x: s.c.x, y: s.c.y + 1);
            if (next.y == faceHeight)
            {
                var (f, d, n) = Wrap(s.face, s.dir, s.c);
                s = s with { face = f, c = n, dir = d };
            }
            else
            {
                s = s with { c = next };
            }
            legal = !m.Faces[s.face].walls.Contains(s.c);
            return (s, legal);
        case 'W':
            next = (x: s.c.x - 1, y: s.c.y);
            if (next.x < 0)
            {
                var (f, d, n) = Wrap(s.face, s.dir, s.c);
                s = s with { face = f, c = n, dir = d };
            }
            else
            {
                s = s with { c = next };
            }
            legal = !m.Faces[s.face].walls.Contains(s.c);
            return (s, legal);
        default: throw new Exception($"Invalid direction {s.dir}");
    }
}

char Turn(State s, char dir)
{
    var current = s.dir;
    switch (current)
    {
        case 'N':
            current = dir == 'L' ? 'W' : 'E';
            break;
        case 'E':
            current = dir == 'L' ? 'N' : 'S';
            break;
        case 'S':
            current = dir == 'L' ? 'E' : 'W';
            break;
        case 'W':
            current = dir == 'L' ? 'S' : 'N';
            break;
        default: throw new Exception($"Invalid direction: {s.dir}");
    }

    return current;
}

State Path(
    Map m,
    State s
)
{
    var dist = m.dists.Pop();
    while (true)
    {
        while (dist > 0)
        {
            var (n, legal) = Next(m, s);
            if (legal)
            {
                s = n;
                dist--;
            }
            else
            {
                dist = 0;
            }
        }

        if (m.dirs.Count == 0)
        {
            break;
        }
        var nextDir = Turn(s, m.dirs.Pop());
        dist = m.dists.Pop();
        s = s with { dir = nextDir };
    }

    return s;
}

int Score(
    Map m,
    State s)
{
    int ScoreFacing(char d)
    {
        switch (d)
        {
            case 'N': return 3;
            case 'E': return 0;
            case 'S': return 1;
            case 'W': return 2;
            default: throw new Exception($"Invalid direction {d}");
        }
    }

    var o = m.Faces[s.face].offset; 
    var row = s.c.y + 1 + o.y;
    var col = s.c.x + 1 + o.x;
    var d = ScoreFacing(s.dir);

    return (row * 1000) + (4 * col) + d;
}

var str = File.ReadAllText(args[0]);
var m = Parse(str);
var s = new State(1, (0,0), 'E');
var r = Path(m, s);
Console.WriteLine(Score(m, r));
// 1204 too low
// 314157 too high
// 46497 too low

public record Face(
    (int x, int y) offset,
    HashSet<(int x, int y)> walls
);

public record Map(
    Dictionary<int, Face> Faces,
    Stack<int> dists,
    Stack<char> dirs
);

public record State(
    int face,
    (int x, int y) c,
    char dir
);

// Map Parse(string str)
// {
//     (Stack<int> dists, Stack<char> dirs) MapDirections(
//         string s)
//     {
//         var dists = new List<int>();
//         var dirs = new List<char>();

//         var nextNumber = "";
//         foreach (var c in s)
//         {
//             if (char.IsDigit(c))
//             {
//                 nextNumber += c;
//             }
//             else
//             {
//                 dists.Add(int.Parse(nextNumber));
//                 dirs.Add(c);
//                 nextNumber = "";
//             }
//         }
//         dists.Add(int.Parse(nextNumber));

//         dists.Reverse();
//         dirs.Reverse();

//         return (
//             new Stack<int>(dists),
//             new Stack<char>(dirs)
//         );
//     }
//     var parts = str.Split("\n\n");
//     var map = parts[0].Split("\n");
//     var directions = parts[1];

//     var height = map.Length;
//     var width = map.Select(x => x.Length).Max();
//     var coordinates = new Dictionary<(int x, int y), char>();
//     for (int y = 0; y < height; y++)
//     {
//         for (int x = 0; x < map[y].Length; x++)
//         {
//             coordinates.Add((x, y), map[y][x]);
//         }
//     }

//     var xs = new Dictionary<int, (int w, int e)>();
//     for (int y = 0; y < height; y++)
//     {
//         var e = -1;
//         var w = -1;
//         for (int x = 0; x < map[y].Length; x++)
//         {
//             var c = coordinates[(x, y)];
//             if (c != ' ')
//             {
//                 w = x;
//                 e = map[y].Length - 1;
//                 xs.Add(y, (w, e));
//                 break;
//             }
//         }
        
//     }

//     var ys = new Dictionary<int, (int w, int e)>();
//     for (int x = 0; x < width; x++)
//     {
//         var n = -1;
//         var s = -1;
//         for (int y = 0; y < height; y++)
//         {
//             var c = coordinates.ContainsKey((x, y)) ? coordinates[(x, y)] : ' ';
//             if (c != ' ' && n == -1)
//             {
//                 n = y;
//             }
//             if (c == ' ' && n != -1)
//             {
//                 s = y - 1;
//                 break;
//             }
//             if (y == height - 1)
//             {
//                 s = y;
//             }
//         }
//         ys.Add(x, (n, s));
//     }

//     var (dists, dirs) = MapDirections(directions);

//     return new Map(
//         new HashSet<(int x, int y)>(coordinates.Where(x => x.Value == '#').Select(x => x.Key)), 
//         xs,
//         ys, 
//         dists,
//         dirs);   
// }

// State Init(Map m)
// {
//     var minX = m.xs[0].w;
//     return new State('E', (minX,0));
// }

// ((int x, int y) p, bool legal) Next(
//     Map m,
//     State s)
// {
//     var x = s.pos.x;
//     var y = s.pos.y;
//     var (minY, maxY) = m.ys[x];
//     var (minX, maxX) = m.xs[y];
//     switch (s.dir)
//     {
//         case 'N':
//             y--;
//             if (y < minY)
//             {
//                 y = maxY;
//             }
//             break;
//         case 'E':
//             x++;
//             if (x > maxX)
//             {
//                 x = minX;
//             }
//             break;
//         case 'S':
//             y++;
//             if (y > maxY)
//             {
//                 y = minY;
//             }
//             break;
//         case 'W':
//             x--;
//             if (x < minX)
//             {
//                 x = maxX;
//             }
//             break;
//         default: throw new Exception($"Invalid direction: {s.dir}");
//     }

//     var legal = !m.walls.Contains((x, y));

//     return ((x, y), legal);
// }

// char Turn(State s, char dir)
// {
//     var current = s.dir;
//     switch (current)
//     {
//         case 'N':
//             current = dir == 'L' ? 'W' : 'E';
//             break;
//         case 'E':
//             current = dir == 'L' ? 'N' : 'S';
//             break;
//         case 'S':
//             current = dir == 'L' ? 'E' : 'W';
//             break;
//         case 'W':
//             current = dir == 'L' ? 'S' : 'N';
//             break;
//         default: throw new Exception($"Invalid direction: {s.dir}");
//     }

//     return current;
// }

// State Path(Map m, State s)
// {
//     var dist = m.dists.Pop();
//     while (true)
//     {
//         while (dist > 0)
//         {
//             var (p, legal) = Next(m, s);
//             if (legal)
//             {
//                 s = s with { pos = p };
//                 dist--;
//             }
//             else
//             {
//                 dist = 0;
//             }
//         }

//         if (m.dirs.Count == 0)
//         {
//             break;
//         }
//         var nextDir = Turn(s, m.dirs.Pop());
//         dist = m.dists.Pop();
//         s = s with { dir = nextDir };
//     }

//     return s;
// }

// int Score(State s)
// {
//     int ScoreFacing(char d)
//     {
//         switch (d)
//         {
//             case 'N': return 3;
//             case 'E': return 0;
//             case 'S': return 1;
//             case 'W': return 2;
//             default: throw new Exception($"Invalid direction {d}");
//         }
//     }

//     var row = s.pos.y + 1;
//     var col = s.pos.x + 1;
//     var d = ScoreFacing(s.dir);

//     return (row * 1000) + (4 * col) + d;
// }

// var m = Parse(File.ReadAllText(args[0]));
// var s = Init(m);
// var r = Path(m, s);

// Console.WriteLine(Score(r));

// public record Map(
//     HashSet<(int x, int y)> walls, 
//     Dictionary<int, (int w, int e)> xs,
//     Dictionary<int, (int n, int s)> ys,
//     Stack<int> dists,
//     Stack<char> dirs);

// public record State(
//     char dir,
//     (int x, int y) pos
// );
