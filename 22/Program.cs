// See https://aka.ms/new-console-template for more information
Map Parse(string str)
{
    int MapDirections(char d)
    {
        switch (d)
        {
            case '>': return 0;
            case 'v': return 1;
            case '<': return 2;
            default: return 3;
        }
    }
    var parts = str.Split("\n\n");
    var map = parts[0].Split("\n");
    var directions = parts[1];

    var height = map.Length;
    var width = map[0].Length;
    var coordinates = new Dictionary<(int x, int y), char>();
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            coordinates.Add((x, y), map[y][x]);
        }
    }

    var ys = new Dictionary<int, (int w, int e)>();
    for (int y = 0; y < height; y++)
    {
        var e = -1;
        var w = -1;
        for (int x = 0; x < width; x++)
        {
            var c = coordinates[(x, y)];
            if (c != ' ' && w != -1)
            {
                w = x;
            }
            if (c == ' ' && w != -1)
            {
                e = x - 1;
                break;
            }
        }
        ys.Add(y, (e, w));
    }

    var xs = new Dictionary<int, (int w, int e)>();
    for (int x = 0; x < width; x++)
    {
        var n = -1;
        var s = -1;
        for (int y = 0; y < height; y++)
        {
            var c = coordinates[(x, y)];
            if (c != ' ' && n != -1)
            {
                n = y;
            }
            if (c == ' ' && n != -1)
            {
                s = x - 1;
                break;
            }
        }
        xs.Add(x, (n, s));
    }

    return new Map(
        coordinates, 
        xs, 
        ys,
        directions.Select(MapDirections).ToArray());    
}

var input = Parse(File.ReadAllText(args[0]));

Console.WriteLine(input);

public record Map(
    Dictionary<(int x, int y), char> walls, 
    Dictionary<int, (int w, int e)> xs,
    Dictionary<int, (int n, int s)> ys,
    int[] directions);
