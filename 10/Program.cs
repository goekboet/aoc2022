var input = System.IO.File.ReadAllLines(args[0]);
var x = 1;
var t = 0;
var signalChange = new List<(int t, int x)>();
foreach (var i in input)
{
    if (i.StartsWith("noop"))
    {
        t += 1;
    }
    else
    {
        var arg = int.Parse(i.Substring(5));
        t += 2;
        x += arg;
        signalChange.Add((t + 1, x));
    }
}

// var signalStrengths = new List<int>();
// for (int c = 20; c <= 220; c += 40)
// {
//     var replay = signalChange.TakeWhile(x => x.t <= c).ToList();
//     var currentX = replay[replay.Count - 1].x;
//     signalChange = signalChange.Skip(replay.Count()).ToList();
//     signalStrengths.Add(c * currentX);
// }

// Console.WriteLine(signalStrengths.Sum());

var ps = new List<char>();
var currentX = 1;
for (int p = 0; p <= 240; p++)
{
    if (signalChange.Count > 0 && (p + 1) == signalChange[0].t)
    {
        currentX = signalChange[0].x;
        signalChange.RemoveAt(0);
    }
    if ((p % 40) >= currentX - 1 && (p % 40) <= currentX + 1)
    {
        ps.Add('#');
    }
    else
    {
        ps.Add('.');
    }
}

var image = ps.Chunk(40).Select(x => new string(x));
foreach (var row in image)
{
    Console.WriteLine(row);
}
