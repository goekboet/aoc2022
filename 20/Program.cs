using System.Text;

// long mod(long n, long m) => (n % m + m) % m;

const long decryptionKey = 811589153L;

Node Fwd(Node n, int l, int? steps = null)
{
    var i = (steps ?? n.Value) % (l - 1);
    if (i == 0 && n.Prev is not null)
    {
        return n.Prev;
    }
    while (i-- > 0)
    {
        if (n?.Next is null) throw new Exception($"Broken link at {n?.Value}");
        n = n.Next;
    }

    return n;
}




Node Rev(Node n, int l)
{
    var i = Math.Abs(n.Value) % (l - 1);
    if (i == 0 && n.Next is not null)
    {
        return n.Next;
    }
    while (i-- > 0)
    {
        if (n?.Prev is null) throw new Exception($"Broken link at {n?.Value}");
        n = n.Prev;
    }

    return n;
}

void InsertAfter(Node ptr, Node target)
{
    if (ptr == target)
    {
        throw new Exception("Trying to insert after itself.");
    }
    // Detach a
    // a.Prev.Next = a.Next;
    // a.Next.Prev = a.Prev;

    if (target.Next is null) throw new Exception("Broken link");
    target.Next.Prev = ptr;
    ptr.Next = target.Next;

    ptr.Prev = target;
    target.Next = ptr;
}

void InsertBefore(Node ptr, Node target)
{
    if (ptr == target)
    {
        Console.WriteLine(ptr.Value);
        throw new Exception("Trying to insert before itself.");
    }

    if (target.Prev is null) throw new Exception("Broken link");
    target.Prev.Next = ptr;
    ptr.Prev = target.Prev;

    ptr.Next = target;
    target.Prev = ptr;
}

// int Count(Node n)
// {
//     var r = 1;
//     var ptr = n.Next;
//     while (ptr != n)
//     {
//         if (ptr is null) throw new Exception("Broken link.");
//         r++;
//         ptr = ptr.Next;
//     }

//     return r;
// }

// string Print(Node n)
// {
//     var sb = new StringBuilder();
//     sb.Append(n.Value.ToString());
//     var ptr = n.Next;
//     while (ptr != n)
//     {
//         if (ptr is null) throw new Exception("Broken link.");
//         sb.Append($", {ptr.Value}");
//         ptr = ptr.Next;
//     }

//     return sb.ToString();
// }

var ns = File.ReadAllLines(args[0]).Select(int.Parse).ToArray();
var init = new Node { Value = ns[0] * decryptionKey };
var current = init;
var initialOrder = new Node[ns.Length];
initialOrder[0] = init;
Node? zero = null;
for (int i = 1; i < ns.Length; i++)
{
    var n = new Node { Value = ns[i] * decryptionKey, Prev = current };
    current.Next = n;

    current = n;
    initialOrder[i] = n;
    if (ns[i] == 0)
    {
        zero = n;
    }
}
current.Next = init;
init.Prev = current;

Node? ptr = null;
for (int i = 0; i < ns.Length * 10; i++)
{
    ptr = initialOrder[i % ns.Length];
    if (ptr.Prev is null || ptr.Next is null) throw new Exception("Broken link");
    if (ptr.Value == 0) continue;
    ptr.Prev.Next = ptr.Next;
    ptr.Next.Prev = ptr.Prev;

    if (ptr.Value < 0)
    {
        var target = Rev(ptr, ns.Length);
        InsertBefore(ptr, target);
    }
    if (ptr.Value > 0)
    {
        var target = Fwd(ptr, ns.Length);
        InsertAfter(ptr, target);
    }
}

if (zero is null) throw new Exception("Did not find zero");

var r = new[] { 0L, 0L, 0L };
for (int i = 0; i < r.Length; i++)
{
    if (zero is null) throw new Exception("Broken link.");
    zero = Fwd(zero, ns.Length, 1000);
    r[i] = zero.Value;
}

// var finalCount = Count(zero);

// Console.WriteLine($"input count: {ns.Length} result count: {finalCount}");
// Console.WriteLine(Print(init));
Console.WriteLine(r.Sum());
// -2424216800011 not



public class Node
{
    public long Value { get; set; }
    public Node? Prev { get; set; }
    public Node? Next { get; set; }
}
