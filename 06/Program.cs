bool isMarker(string s) => s.Distinct().Count() == 14;

var input = File.ReadAllText(args[0]);

var r = 0;
for (int i = 0; i < input.Length - 14; i++)
{
    var s = input.Substring(i, 14);
    if (isMarker(s))
    {
        r = i + 14;
        break;
    }
}

Console.WriteLine(r);
