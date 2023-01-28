// IEnumerable<(int x, int y)> GetCircumference(
//     int height, 
//     int width,
//     (int x, int y) offset)
// {
//     var w = 0;
//     while (w < width)
//     {
//         yield return (w + offset.x, offset.y);
//         yield return (w + offset.x, offset.y + height - 1);
//         w++;
//     }
//     var h = 1;
//     while (h < height - 1)
//     {
//         yield return (offset.x, h + offset.y);
//         yield return (offset.x + width - 1, h + offset.y);
//         h++;
//     }
// }

Forest Parse(string[] ls)
{
    var width = ls[0].Length;
    var height = ls.Length;
    var trees = new Dictionary<(int x, int y), int>();
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            var c = ls[y][x];
            trees.Add((x, y), (int)char.GetNumericValue(c));
        }
    }

    return new Forest(width, height, trees);
}

var input = System.IO.File.ReadAllLines(args[0]);

// var firstC = GetCircumference(height - 2, width - 2, (1,1));

// Console.WriteLine(string.Join(", ", firstC));
// Console.WriteLine($"width: {width} height: {height}");

var forest = Parse(input);

// Console.WriteLine(forest);
// var seen = new HashSet<(int x, int y)>();

// // North
// for (int x = 0; x < forest.width; x++)
// {
//     var h = -1;
//     for (int y = 0; y < forest.height; y++)
//     {
//         var t = forest.trees[(x, y)];
//         if (t > h)
//         {
//             seen.Add((x, y));
//             h = t;
//         }
//         if (h == 9) break;
//     }
// }

// // East
// for (int y = 0; y < forest.height; y++)
// {
//     var h = -1;
//     for (int x = forest.width - 1; x >= 0; x--)
//     {
//         var t = forest.trees[(x, y)];
//         if (t > h)
//         {
//             seen.Add((x, y));
//             h = t;
//         }
//         if (h == 9) break;
//     }
// }

// // South
// for (int x = 0; x < forest.width; x++)
// {
//     var h = -1;
//     for (int y = forest.height - 1; y >= 0; y--)
//     {
//         var t = forest.trees[(x, y)];
//         if (t > h)
//         {
//             seen.Add((x, y));
//             h = t;
//         }
//         if (h == 9) break;
//     }
// }

// // West
// for (int y = 0; y < forest.height; y++)
// {
//     var h = -1;
//     for (int x = 0; x < forest.width; x++)
//     {
//         var t = forest.trees[(x, y)];
//         if (t > h)
//         {
//             seen.Add((x, y));
//             h = t;
//         }
//         if (h == 9) break;
//     }
// }

var highScore = 0;
for (int x = 0; x < forest.width; x++)
{
    for (int y = 0; y < forest.height; y++)
    {
        var c = (x, y);
        var t = forest.trees[c];
        // var m = -1;
        var distances = new [] {0,0,0,0};

        for (int n = c.y - 1; n >= 0; n--)
        {
            var h = forest.trees[(c.x, n)];
            distances[0]++;
            if (h < t)
            {
                continue;
            }
            else
            {
                break;
            }
            // if (h >= m)
            // {
            //     distances[0]++;
            //     m = h;
            // }
                
            // if (h >= t)
            // {
            //     break;
            // } 
        }

        // m = -1;
        for (int e = c.x + 1; e < forest.width; e++)
        {
            var h = forest.trees[(e, c.y)];
            distances[1]++;
            if (h < t)
            {
                continue;
            }
            else
            {
                break;
            }
            // if (h >= m)
            // {
            //     distances[1]++;
            //     m = h;
            // }
            // if (h >= t)
            // {
            //     break;
            // }  
        }

        // m = -1;
        for (int s = c.y + 1; s < forest.height; s++)
        {
            var h = forest.trees[(c.x, s)];
            distances[2]++;
            if (h < t)
            {
                continue;
            }
            else
            {
                break;
            }
            // if (h >= m)
            // {
            //     distances[2]++;
            //     m = h;
            // }
            // if (h >= t)
            // {
            //     break;
            // } 
        }

        // m = -1;
        for (int w = c.x - 1; w >= 0; w--)
        {
            var h = forest.trees[(w, c.y)];
            distances[3]++;
            if (h < t)
            {
                continue;
            }
            else
            {
                break;
            }
            // if (h >= m)
            // {
            //     distances[3]++;
            //     m = h;
            // }
            // if (h >= t)
            // {
            //     break;
            // } 
        }

        // var score = distances.Where(x => x > 0).Aggregate(1, (s, x) => s * x);
        var score = distances.Aggregate(1, (s, x) => s * x);
        if (score > highScore)
        {
            highScore = score;
        }
    }
}

Console.WriteLine(highScore);
// Console.WriteLine(seen.Count);
// Console.WriteLine($"(1,1) - {seen.Contains((1,1))}");
// Console.WriteLine($"(2,1) - {seen.Contains((2,1))}");
// Console.WriteLine($"(3,1) - {seen.Contains((3,1))}");
// Console.WriteLine($"(1,2) - {seen.Contains((1,2))}");
// Console.WriteLine($"(2,2) - {seen.Contains((2,2))}");
// Console.WriteLine($"(3,2) - {seen.Contains((2,3))}");
// Console.WriteLine($"(2,3) - {seen.Contains((2,3))}");

public record Forest(int width, int height, Dictionary<(int x, int y), int> trees);
