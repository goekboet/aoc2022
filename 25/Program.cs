using System.Numerics;

int SnafuMap(char c)
{
    switch (c)
    {
        case '=': return -2;
        case '-': return -1;
        case '0': return 0;
        case '1': return 1;
        case '2': return 2;
        default: throw new Exception($"Undefined snafu-char: {c}");
    }
}

BigInteger SnafuToDecimal(string snafu)
{
    var d = new BigInteger(0);
    var p = new BigInteger(1);
    for (int i = 0; i < snafu.Length; i++)
    {
        var c = snafu[snafu.Length - 1 - i];
        d += SnafuMap(c) * p;
        p *= 5;
    }

    return d;
}

string DecimalToSnafu(BigInteger dec)
{
    var snafu = "";
    var n = dec;
    var carry = 0;
    while (true)
    {
        var rem = (int)(n % 5);
        if (rem == 3)
        {
            snafu += "=";
            carry = 1;
        }
        else if (rem == 4)
        {
            snafu += "-";
            carry = 1;
        }
        else
        {
            snafu += rem.ToString();
            carry = 0;
        }

        n = (n / 5) + carry;
        if (n == 0)
        {
            break;
        }
    }

    return new string(snafu.Reverse().ToArray());
}

var input = File.ReadAllLines(args[0]);
Console.WriteLine("               SNAFU              Decimal");
var r = new BigInteger(0);
// var ds = new List<long>();
foreach (var l in input)
{
    var dec = SnafuToDecimal(l);
    r += dec;
    Console.WriteLine($"{l, 20} {dec, 20}");
    // ds.Add(dec);
}
// Console.WriteLine();
Console.WriteLine($"Sum: {r} - {DecimalToSnafu(r)}");
// Console.WriteLine();
// Console.WriteLine("Decimal    Snafu");
// foreach (var d in ds)
// {
//     var snafu = DecimalToSnafu(d);
//     Console.WriteLine($"{d, 7}  {snafu, 8}");
// }