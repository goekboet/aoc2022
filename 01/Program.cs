var input = File.ReadAllText(args[0]).Split("\n\n");

// var r = input.Select(x => x.Split('\n').Select(c => int.Parse(c)).Sum()).Max();

var r = input
    .Select(x => x.Split('\n').Select(c => int.Parse(c)).Sum())
    .OrderByDescending(x => x)
    .Take(3)
    .Sum();

Console.WriteLine(r);
