List<Elf> Parse(string[] input)
{
    var elves = new List<Elf>();

    for (int y = 0; y < input.Length; y++)
    {
        for (int x = 0; x < input[y].Length; x++)
        {
            var c = input[y][x];
            if (c == '#')
            {
                var elf = new Elf((x, y), null);
                elves.Add(elf);
            }
        }
    }

    return elves;
}



(int x, int y)[] Consider(char d, (int x, int y) c)
{
    switch (d)
    {
        case 'E': return new [] { (c.x + 1, c.y - 1),(c.x + 1, c.y),(c.x + 1, c.y + 1)};
        case 'W': return new [] { (c.x - 1, c.y - 1),(c.x - 1, c.y),(c.x - 1, c.y + 1)};
        case 'S': return new [] { (c.x - 1, c.y + 1),(c.x, c.y + 1),(c.x + 1, c.y + 1)};
        case 'N': return new [] { (c.x - 1, c.y - 1),(c.x, c.y - 1),(c.x + 1, c.y - 1)};
        default: throw new Exception($"Invalid direction {d}");
    }
}

(int x, int y) Move(char d, (int x, int y) c)
{
    switch (d)
    {
        case 'E': return (c.x + 1, c.y);
        case 'W': return (c.x - 1, c.y);
        case 'S': return (c.x, c.y + 1);
        case 'N': return (c.x, c.y - 1);
        default: throw new Exception($"Invalid direction {d}");
    }
}

(List<Elf>, int) Solve(List<Elf> elves)
{
    var dirs = new Queue<char>();
    dirs.Enqueue('N');
    dirs.Enqueue('S');
    dirs.Enqueue('W');
    dirs.Enqueue('E');

    var round = 0;
    while (true)
    {
        var moved = false;
        var lookup = elves.ToDictionary(x => x.current);
        for (int i = 0; i < elves.Count; i++)
        {
            var propose = new Queue<char>();
            var elf = elves[i];
            foreach (var d in dirs)
            {
                if (!Consider(d, elf.current).Any(x => lookup.ContainsKey(x)))
                {
                    propose.Enqueue(d);
                }
            }
            if (propose.Count > 0 && propose.Count < 4)
            {
                var proposed = propose.Dequeue();
                elves[i] = elf with { proposed = Move(proposed, elf.current)};
            }
        }
        var proposals = elves
            .Where(x => x.proposed is not null)
            .GroupBy(x => x.proposed.Value)
            .ToDictionary(x => x.Key, x => x.Count());

        var next = new List<Elf>();
        foreach (var e in elves)
        {
            if (e.proposed.HasValue && proposals[e.proposed.Value] == 1)
            {
                next.Add(new Elf(e.proposed.Value, null));
                moved = true;
            }
            else
            {
                next.Add(new Elf(e.current, null));
            }
        }
        elves = next;
        var q = dirs.Dequeue();
        dirs.Enqueue(q);
        round++;

        if (!moved)
        {
            break;
        }
    }

    return (elves, round);
}

// void Debug(List<Elf> elves)
// {
//     var minX = elves.Select(x => x.current.x).Min();
//     var maxX = elves.Select(x => x.current.x).Max();
//     var minY = elves.Select(x => x.current.y).Min();
//     var maxY = elves.Select(x => x.current.y).Max();

//     var o = (x: minX, y: minY);
//     var width = (maxX - minX) + 1;
//     var height = (maxY - minY) + 1;
//     var lookup = elves.Select(x => x.current).ToHashSet();
//     for (int y = 0; y < height; y++)
//     {
//         for (int x = 0; x < width; x++)
//         {
//             var t = (x + o.x, y + o.y);
//             if (lookup.Contains(t))
//             {
//                 Console.Write("#");
//             }
//             else
//             {
//                 Console.Write(".");
//             }
//         }
//         Console.WriteLine();
//     }
//     Console.WriteLine();
// }

int Score(List<Elf> elves)
{
    var minX = elves.Select(x => x.current.x).Min();
    var maxX = elves.Select(x => x.current.x).Max();
    var minY = elves.Select(x => x.current.y).Min();
    var maxY = elves.Select(x => x.current.y).Max();  
    var width = (maxX - minX) + 1;
    var height = (maxY - minY) + 1;

    return (width * height) - elves.Count; 
}

var input = File.ReadAllLines(args[0]);
var elves = Parse(input);
var (es, r) = Solve(elves);
// Debug(elves);
// Console.WriteLine(Score(elves));
Console.WriteLine(r);

public record Elf(
    (int x, int y) current,
    (int x, int y)? proposed
);
