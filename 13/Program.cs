(Packet p, int pos) Parse(string s, int pos = 0)
{
    if (s[pos] == '[')
    {
        pos++;
        var l = new List<Packet>();
        while (s[pos] != ']')
        {
            var (np, npos) = Parse(s, pos);
            l.Add(np);
            pos = npos;
            if (s[pos] == ',')
            {
                pos++;
            }
        }

        return (new PacketList { List = l.ToArray() }, pos + 1);
    }
    else
    {
        var digits = new List<char>();
        while (char.IsDigit(s[pos]))
        {
            digits.Add(s[pos++]);
        }


        return (new Digit { Value = int.Parse(new string(digits.ToArray())) }, pos);
    }
}

static bool? Correct(Packet lhs, Packet rhs)
{
    bool? correct = null;
    switch ((lhs, rhs))
    {
        case (PacketList ll, PacketList rl):
            if (ll.List.Length < rl.List.Length)
            {
                correct = true;
            }
            if (ll.List.Length > rl.List.Length)
            {
                correct = false;
            }
            for (int i = 0; i < ll.List.Length; i++)
            {
                bool? itemOrder = null;
                if (i < rl.List.Length)
                {
                    itemOrder = Correct(ll.List[i], rl.List[i]);
                    if (itemOrder is not null)
                    {
                        correct = itemOrder;
                        break;
                    }
                }
            }
            break;
        case (PacketList ll, Digit rd):
            correct = Correct(ll, new PacketList { List = new[] { rd } });
            break;
        case (Digit ld, PacketList rl):
            correct = Correct(new PacketList { List = new[] { ld } }, rl);
            break;
        case (Digit ld, Digit rd):
            if (ld.Value < rd.Value)
            {
                correct = true;
            }
            else if (ld.Value > rd.Value)
            {
                correct = false;
            }
            break;
        default:
            break;
    }

    return correct;
}

// var input = File.ReadAllText(args[0])
//     .Split($"{Environment.NewLine}{Environment.NewLine}")
//     // .Skip(3)
//     .Select((x, i) => {
//         var ps = x.Split(Environment.NewLine);
//         var (lhs, _) = Parse(ps[0]);
//         var (rhs, _) = Parse(ps[1]);
//         return (idx: i + 1, correct: Correct(lhs, rhs));
//     });

Packet sep1 = new PacketList { List = new[] { new Digit { Value = 2 } } };
Packet sep2 = new PacketList { List = new[] { new Digit { Value = 6 } } };

var input2 = File.ReadAllLines(args[0])
    .Where(x => x != "")
    .Select(x => Parse(x).p)
    .Concat(new[] { sep1, sep2 })
    .ToList();

input2.Sort(new PacketComparer());

var idx1 = input2.FindIndex(x => Correct(sep1, x) is null);
var idx2 = input2.FindIndex(x => Correct(sep2, x) is null);

Console.WriteLine((idx1 + 1) * (idx2 + 1));




// Console.WriteLine(input.Where(x => x.correct ?? false).Select(x => x.idx).Sum());

// var r = Parse(input[1].Split(Environment.NewLine)[0]);

// Console.WriteLine("done");

// var fst = new PacketList
// {
//     List = new Packet[]
//     {
//         new Digit { Value = 1},
//         new Digit { Value = 1},
//         new Digit { Value = 3},
//         new Digit { Value = 1},
//         new Digit { Value = 1}
//     }
// };

// var snd = new PacketList
// {
//     List = new Packet[]
//     {
//         new PacketList { List = new Packet[] { new Digit { Value = 1 }}},
//         new PacketList { List = new Packet[] { new Digit { Value = 1 }, new Digit { Value = 1 }, new Digit { Value = 1 }}},
//     }
// };


public abstract class Packet { };

public class Digit : Packet
{
    public int Value { get; set; }
};

public class PacketList : Packet
{
    public Packet[] List { get; set; } = new Packet[0];
}

public class PacketComparer : IComparer<Packet>
{
    static bool? Correct(Packet lhs, Packet rhs)
    {
        bool? correct = null;
        switch ((lhs, rhs))
        {
            case (PacketList ll, PacketList rl):
                if (ll.List.Length < rl.List.Length)
                {
                    correct = true;
                }
                if (ll.List.Length > rl.List.Length)
                {
                    correct = false;
                }
                for (int i = 0; i < ll.List.Length; i++)
                {
                    bool? itemOrder = null;
                    if (i < rl.List.Length)
                    {
                        itemOrder = Correct(ll.List[i], rl.List[i]);
                        if (itemOrder is not null)
                        {
                            correct = itemOrder;
                            break;
                        }
                    }
                }
                break;
            case (PacketList ll, Digit rd):
                correct = Correct(ll, new PacketList { List = new[] { rd } });
                break;
            case (Digit ld, PacketList rl):
                correct = Correct(new PacketList { List = new[] { ld } }, rl);
                break;
            case (Digit ld, Digit rd):
                if (ld.Value < rd.Value)
                {
                    correct = true;
                }
                else if (ld.Value > rd.Value)
                {
                    correct = false;
                }
                break;
            default:
                break;
        }

        return correct;
    }
    public int Compare(Packet? x, Packet? y)
    {
        if (x is not null && y is not null)
        {
            var r = Correct(x, y);

            if (r is null) return 0;
            if (r.Value) return -1;
            else return 1;
        }
        else
        {
            return 0;
        }
    }
}