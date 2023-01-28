using System.Text;

// long mod(long n, long m) => (n % m + m) % m;

Node Fwd(Node n, int? steps = null)
{
    var i = steps ?? n.Value;
    while (i-- > 0)
    {
        if (n?.Next is null) throw new Exception($"Broken link at {n?.Value}"); 
        n = n.Next;
    }

    return n;
}




Node Rev(Node n)
{
    var i = Math.Abs(n.Value);
    while (i-- > 0)
    {
        if (n?.Prev is null) throw new Exception($"Broken link at {n?.Value}"); 
        n = n.Prev;
    }

    return n;
}

void InsertAfter(Node a, Node b)
{
    if (a == b) 
    {
        throw new Exception("Trying to insert after itself.");
    }
    // Detach a
    // a.Prev.Next = a.Next;
    // a.Next.Prev = a.Prev;

    if (b.Next is null) throw new Exception("Broken link");
    b.Next.Prev = a;
    a.Next = b.Next;
    
    a.Prev = b;
    b.Next = a;
}

void InsertBefore(Node a, Node b)
{
    if (a == b)
    {
        throw new Exception("Trying to insert before itself.");
    } 

    if (b.Prev is null) throw new Exception("Broken link");
    b.Prev.Next = a;
    a.Prev = b.Prev;
    
    a.Next = b;
    b.Prev = a;
}

int Count(Node n)
{
    var r = 1;
    var ptr = n.Next;
    while (ptr != n)
    {
        if (ptr is null) throw new Exception("Broken link.");
        r++;
        ptr = ptr.Next;
    }

    return r;
}

string Print(Node n)
{
    var sb = new StringBuilder();
    sb.Append(n.Value.ToString());
    var ptr = n.Next;
    while (ptr != n)
    {
        if (ptr is null) throw new Exception("Broken link.");
        sb.Append($", {ptr.Value}");
        ptr = ptr.Next;
    }

    return sb.ToString();
}

var ns = File.ReadAllLines(args[0]).Select(int.Parse).ToArray();
var initialSet = new HashSet<int>(ns);
var init = new Node { Value = ns[0] };
var current = init;
var initialOrder = new Node[ns.Length];
initialOrder[0] = init;
Node? zero = null; 
for (int i = 1; i < ns.Length; i++)
{
    var n = new Node { Value = ns[i], Prev = current };
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
for (int i = 0; i < ns.Length; i++)
{
    ptr = initialOrder[i];
    if (ptr.Prev is null || ptr.Next is null) throw new Exception("Broken link");
    if (ptr.Value == 0) continue;
    ptr.Prev.Next = ptr.Next;
    ptr.Next.Prev = ptr.Prev;

    if (ptr.Value < 0)
    {
        var target = Rev(ptr);
        InsertBefore(ptr, target);
        
    }
    if (ptr.Value > 0)
    {
        var target = Fwd(ptr);
        InsertAfter(ptr, target);
    }
}

if (zero is null) throw new Exception("Did not find zero");

var r = new [] {0,0,0};
for (int i = 0; i < r.Length; i++)
{
    if (zero is null) throw new Exception("Broken link.");
    zero = Fwd(zero, 1000);
    r[i] = zero.Value;
}

var finalCount = Count(zero);

// Console.WriteLine($"input count: {ns.Length} result count: {finalCount}");
Console.WriteLine(Print(init));
Console.WriteLine(r.Sum());
// 23239 too high
// -17333 not correct
// 15032 not correct
// 8216 not correct
// 2637 not correct
// 12855 not correct


public class Node
{
    public int Value {get;set;}
    public Node? Prev {get;set;} 
    public Node? Next {get;set;}
}
