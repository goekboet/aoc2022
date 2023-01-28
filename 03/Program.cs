int Score(char c) => Char.IsLower(c) ? (int)c - 96 : (int)c - 38;

// char FindMisplaced(string l)
// {
//     var compartment1 = new HashSet<char>(l.Take(l.Length / 2));
//     var compartment2 = new HashSet<char>(l.Skip(l.Length / 2));

//     return compartment1.Intersect(compartment2).Single();
// }

char FindBadge(string[] group)
{
    if (group.Length != 3) throw new Exception("Each group must be of size 3.");
    var r1 = new HashSet<char>(group[0]);
    var r2 = new HashSet<char>(group[1]);
    var r3 = new HashSet<char>(group[2]);

    return r1.Intersect(r2).Intersect(r3).Single();
}

// var r1 = 
//     File.ReadAllLines(args[0])
//     .Select(FindMisplaced)
//     .Select(Score)
//     .Sum();

var r2 = 
    File.ReadAllLines(args[0])
    .Chunk(3)
    .Select(FindBadge)
    .Select(Score)
    .Sum();

Console.WriteLine(r2);

// var cs = new [] { 'A', 'a', 'B', 'b', 'Z', 'z'};
// foreach (var c in cs)
// {
//     Console.WriteLine($"{c} - {Score(c)}");
// }