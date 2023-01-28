using System.Text.RegularExpressions;


ValveEntry Parse(string s)
{
    var m = Regex.Match(s, @"Valve ([A-Z]+) has flow rate=(\d+); tunnels? leads? to valves? (.+)");
    var adj = m.Groups[3].Value.Split(", ");

    return new ValveEntry(m.Groups[1].Value, int.Parse(m.Groups[2].Value), adj);
}

Valve[] GetNodes(
    string start,
    ValveEntry[] graph
)
{
    var r = new List<Valve>();
    var s = graph.First(x => x.id == start);
    var batch = new List<Valve>();
    var visited = new HashSet<string>();
    batch.Add(new Valve(s.id, 0, 0));
    while (batch.Count > 0)
    {
        var next = new List<Valve>();
        foreach (var current in batch)
        {
            if (current.rate > 0)
            {
                r.Add(current);
            }
            var n = graph.First(x => x.id == current.id);
            foreach (var adj in n.adj.Where(x => !visited.Contains(x)))
            {
                var a = graph.First(x => x.id == adj);
                next.Add(new Valve(a.id, a.rate, current.distance + 1));
            }
            visited.Add(current.id);
        }
        batch = next;        
    }

    return r.ToArray();
}

Dictionary<string, int> BestFlowForPath = new();

void FindPaths(
    string allValves,
    int tLimit,
    List<ValveNode> nodes,
    State current,
    List<Path> paths)
{
    var n = nodes.First(x => x.id == current.id);
    var path = string.Join("-", current.path.Split("-", StringSplitOptions.RemoveEmptyEntries).Append(current.id).Where(x => x != "AA").OrderBy(x => x));
    var flow = current.flow + (n.rate * (tLimit - current.t));
    if (BestFlowForPath.TryGetValue(path, out int bestFlow) && flow > bestFlow)
    {
        BestFlowForPath[path] = flow;
    }
    if (!BestFlowForPath.ContainsKey(path))
    {
        BestFlowForPath[path] = flow;
    }
    if (path == allValves)
    {
        paths.Add(new Path(path, flow));
    }
    else
    {
        foreach (var v in n.nodes.Where(x => !path.Contains(x.id)))
        {
            var t = current.t + v.distance + 1;
            
            if (t > tLimit)
            {
                paths.Add(new Path(path, flow));
                return;
            }
            
            var state = new State(path, v.id, flow, t);
            FindPaths(allValves, tLimit, nodes, state, paths);
        }
    }
}

// var memo = new Dictionary<(string path, int t), int>();

// void FindPaths2(
//     string allValves,
//     int tLimit,
//     List<ValveNode> nodes,
//     State2 current,
//     List<Path2> paths)
// {
//     if (current.humanId == "" && current.elephantId == "")
//     {
//         paths.Add(new Path2(current.path, current.flow, current.history));
//         return;
//     }
//     var human = nodes.FirstOrDefault(x => x.id == current.humanId) ?? new ValveNode("", 0, new Valve[0]);
//     var elephant = nodes.FirstOrDefault(x => x.id == current.elephantId) ?? new ValveNode("", 0, new Valve[0]);
//     var path = string.Join(
//         "-", 
//         current.path
//             .Split("-", StringSplitOptions.RemoveEmptyEntries)
//             .Concat(new HashSet<string>(new [] { current.humanId, current.elephantId}))
//             .Where(x => x != "")
//             .OrderBy(x => x));

//     var flow = current.flow + (human.rate * (tLimit - current.humanT)) + (elephant.rate * (tLimit - current.elephantT));
    
//     if (memo is not null && memo.TryGetValue((path, current.humanT +current.elephantT), out int bestFlow))
//     {
//         if (bestFlow < flow)
//         {
//             memo[(path, current.humanT + current.elephantT)] = flow;
//         }
//         else
//         {
//             return;
//         }
//     }
//     if (memo is not null && !memo.ContainsKey((path, current.humanT + current.elephantT)))
//     {
//         memo[(path, current.humanT + current.elephantT)] = flow;
//     }

//     if (path == allValves)
//     {
//         paths.Add(new Path2(path, flow, current.history));
//         return;
//     }
//     else
//     {
//         var humanNext = human.nodes.Where(x => !path.Contains(x.id));
//         var elephantNext = elephant.nodes.Where(x => !path.Contains(x.id));
//         foreach (var hN in humanNext)
//         {
//             var ht = current.humanT + hN.distance + 1;
//             var nextHumanNode = ht > tLimit ? "" : hN.id;
//             foreach (var eN in elephantNext)
//             {
//                 if (hN.id == eN.id) continue;
//                 var et = current.elephantT + eN.distance + 1;
//                 var nextElephantNode = et > tLimit ? "" : eN.id;

//                 var history = new [] {(actor: "human", id: nextHumanNode, t: ht),(actor: "elephant", id: nextElephantNode,t: et)};
//                 var state = new State2(path, nextHumanNode, nextElephantNode, ht, et, flow, current.history.Concat(history).ToList());
//                 FindPaths2(allValves, tLimit, nodes, state, paths);
//             }
//         }
//     }
// }

// void FindSets<T>(T[] es, List<HashSet<T>> ps, HashSet<T> s, int p)
// {
//     if (p == es.Length)
//     {
//         if (s.Count > 0)
//         {
//             ps.Add(s);
//         }
        
//         return;
//     }
 
//     FindSets<T>(es, ps, new HashSet<T>(s), p + 1);
//     s.Add(es[p]);
//     FindSets<T>(es, ps, new HashSet<T>(s), p + 1);
// }

// List<HashSet<T>> PowerSet<T>(T[] es)
// {
//     var ps = new List<HashSet<T>>();
//     FindSets<T>(es, ps, new HashSet<T>(), 0);
 
//     return ps;
// }

// HashSet<string> FromPath(string p) => new HashSet<string>(p.Split("-"));

var valves = File.ReadAllLines(args[0])
    .Select(Parse)
    .ToArray();

var entry = valves.Where(x => x.id == "AA").ToArray();
var flows = valves.Where(x => x.rate > 0).ToArray();
var map = new List<ValveNode>();
foreach (var v in entry.Concat(flows))
{
    var nodes = GetNodes(v.id, valves);
    map.Add(new ValveNode(v.id, v.rate, nodes));
}

HashSet<string> PathToSet(string path) => new HashSet<string>(path.Split("-"));

Console.WriteLine("Finding paths...");
var init = new State("", "AA", 0, 0);
var allValves = string.Join("-", map.Select(x => x.id).Where(x => x != "AA").OrderBy(x => x));
var r = new List<Path>();
FindPaths(allValves, 26, map, init, r);
var allValvesSet = PathToSet(allValves);
var lookup = BestFlowForPath.Select(x => (path: PathToSet(x.Key), x.Value)).ToArray();
var winner = 0;
foreach (var p in lookup)
{
    var complementaryPath = new HashSet<string>(allValvesSet.Except(p.path));
    (HashSet<string>, int)? bestComplementaryFlow = lookup
        .Where(x => x.path.IsSubsetOf(complementaryPath))
        .OrderByDescending(x => x.Value)
        .FirstOrDefault();

    if (bestComplementaryFlow is not null)
    {
        var candidate = p.Value + (bestComplementaryFlow?.Item2 ?? 0);
        if (candidate > winner)
        {
            winner = candidate;
        }
    }
}

// var r2 = r.GroupBy(x => x.path, (k, xs) => (path: k, flow: xs.Select(x => x.flow).Max()));
// var combined = new List<Path>();



// var best = r2.OrderByDescending(x => x.flow).First();





// var bestElefant = r.Select(x => (path: new HashSet<string>(x.Where(x => x.id != "AA").Select(n => n.id)), release: x.Select(p => p.rate * (timelimit - p.t)).Sum()));
// var best = r.OrderByDescending(x => x.flow).First();

Console.WriteLine(winner);

// var arr = cp.OrderByDescending(x => x.flow).First();
// Console.WriteLine($"cp count: {arr}");
// Console.WriteLine($"best release: {arr.flow}");

// Console.WriteLine(bestHuman.First().release);
// Console.WriteLine(bestElefant.First().release);

public record ValveEntry(string id, int rate, string[] adj);
public record Valve(string id, int rate, int distance);
public record ValveNode(string id, int rate, Valve[] nodes);
public record State(string path, string id, int flow, int t);
public record State2(string path, string humanId, string elephantId, int humanT, int elephantT, int flow, List<(string actor, string id, int t)> history);
public record Path(string path, int flow);
public record Path2(string path, int flow, List<(string actor, string id, int t)> history);
public record ValveOpening(string id, int rate, int t);
public record ValveOpening2(bool elephant, string id, int rate, int t);

