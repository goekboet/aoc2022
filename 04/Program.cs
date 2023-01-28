using System.Text.RegularExpressions;

const string ElfPairPattern = @"(\d+)-(\d+),(\d+)-(\d+)";

((int a, int b) s1, (int a, int b) s2) Parse(string l)
{
    var m = Regex.Match(l, ElfPairPattern);

    return (
        (int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value)),
        (int.Parse(m.Groups[3].Value), int.Parse(m.Groups[4].Value))
        );
}

bool SectionContains((int a, int b) s1, (int a, int b) s2) => (s1.a <= s2.a) && (s1.b >= s2.b);

bool SectionOverlaps((int a, int b) s1, (int a, int b) s2) =>
    (s1.b >= s2.a && s1.b <= s2.b) ||
    (s1.a >= s2.a && s1.a <= s2.b);

// var r1 = File.ReadAllLines(args[0])
//     .Select(Parse)
//     .Where(x => SectionContains(x.s1, x.s2) || SectionContains(x.s2, x.s1))
//     .Count();

var r2 = File.ReadAllLines(args[0])
    .Select(Parse)
    .Where(x => 
        SectionContains(x.s1, x.s2) || 
        SectionContains(x.s2, x.s1) ||
        SectionOverlaps(x.s1, x.s2))
    // .Where(x =>
    // {
    //     Console.WriteLine(x);
    //     var ol = SectionOverlaps(x.s1, x.s2);
    //     Console.WriteLine(ol);
    //     return ol;
    // })
    .Count();

Console.WriteLine(r2);


