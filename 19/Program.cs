using System.Text.RegularExpressions;

Blueprint Parse(string s)
{
    var m = Regex.Match(s, @"Blueprint (\d+): Each ore robot costs (\d+) ore. Each clay robot costs (\d+) ore. Each obsidian robot costs (\d+) ore and (\d+) clay. Each geode robot costs (\d+) ore and (\d+) obsidian.");
    var ore = new OreRobot(int.Parse(m.Groups[2].Value));
    var clay = new ClayRobot(int.Parse(m.Groups[3].Value));
    var obsidian = new ObsidianRobot(int.Parse(m.Groups[4].Value), int.Parse(m.Groups[5].Value));
    var geode = new GeodeRobot(int.Parse(m.Groups[6].Value), int.Parse(m.Groups[7].Value));

    return new Blueprint(
        int.Parse(m.Groups[1].Value),
        ore,
        clay,
        obsidian,
        geode
    );
}



State Init(Blueprint p) => new State(p, null, 0,0,0,0,1,0,0,0,0);




// int? TimeToNextOreRobot(State s) 
// {
//     var r = s.ore;
//     var t = s.m;
//     while (r < s.p.ore.oreCost)
//     {
//         t++;
//         r += s.oreRobots;
//     }

//     return r;
// }

int? TimeToNextOreRobot(State s) => s.oreRobots > 0
    ? (int)Math.Ceiling((s.p.ore.oreCost - s.ore) / (double)s.oreRobots)
    : null;


int? TimeToNextClayRobot(State s) => s.oreRobots > 0
    ? (int)Math.Ceiling((s.p.clay.oreCost - s.ore) / (double)s.oreRobots)
    : null;

int? TimeToNextObsidianRobot(State s) => s.oreRobots > 0 && s.clayRobots > 0
    ? Math.Max((int)Math.Ceiling((s.p.obsidian.oreCost - s.ore) / (double)s.oreRobots), (int)Math.Ceiling((s.p.obsidian.clayCost - s.clay ) / (double)s.clayRobots))
    : null;

int? TimeToNextGeodeRobot(State s) => s.oreRobots > 0 && s.obsidianRobots > 0
    ? Math.Max((int)Math.Ceiling((s.p.geode.oreCost - s.ore) / (double)s.oreRobots), (int)Math.Ceiling((s.p.geode.obsidianCost - s.obsidian) / (double)s.obsidianRobots))
    : null;

State Forward(State s, int m) => new State(
    s.p, 
    s, 
    s.m + m, 
    s.ore + (m * s.oreRobots),
    s.clay + (m * s.clayRobots),
    s.obsidian + (m * s.obsidianRobots),
    s.oreRobots,
    s.clayRobots,
    s.obsidianRobots,
    s.geodeRobots,
    s.openGeodes + (m * s.geodeRobots)); 

// State[] Adj2(State s)
// {
//     var m = s.m;
//     var rs = (ore: s.ore, clay: s.clay, obsidian: s.obsidian, openGeodes: s.openGeodes );
//     var targets = new [] 
//     {
//         s.oreRobots > 0,
//         s.clayRobots > 0,
//         s.obsidianRobots > 0,
//         s.geodeRobots > 0
//     };
//     var r = new List<State>();
    
//     while(targets[0] && targets[1] && targets[2] && targets[3])
//     {
//         m++;
//         rs = (rs.ore + s.oreRobots, rs.clay + s.clayRobots, rs.obsidian + s.obsidianRobots, rs.openGeodes + s.geodeRobots);
//         i
//     }
    
// }

bool CanUseMoreOreRobots(State s) => new [] { s.p.ore.oreCost, s.p.clay.oreCost, s.p.obsidian.oreCost, s.p.geode.oreCost}.Max() > s.oreRobots;
// bool CanUseMo
bool CanUseMoreClayRobots(State s) => s.p.obsidian.clayCost > s.clayRobots; 
bool CanUseMoreObsidianRobots(State s) => s.p.geode.obsidianCost > s.obsidianRobots; 

State[] Adj(State s, int tLimit)
{
    var timeToNextOreRobot = TimeToNextOreRobot(s);
    if (timeToNextOreRobot < 1) timeToNextOreRobot = 0;
    
    var timeToNextClayRobot = TimeToNextClayRobot(s);
    if (timeToNextClayRobot < 1) timeToNextClayRobot = 0;
    
    var timeToNextObsidianRobot = TimeToNextObsidianRobot(s);
    if (timeToNextObsidianRobot < 1) timeToNextObsidianRobot = 0;
    
    var timeToNextGeodeRobot = TimeToNextGeodeRobot(s);
    if (timeToNextGeodeRobot < 1) timeToNextGeodeRobot = 0;

    var next = new List<State>();
    if (s.openGeodes > 0)
    {
        next.Add(Forward(s, tLimit - s.m));
    }
    if (timeToNextOreRobot is not null && CanUseMoreOreRobots(s) && s.m + timeToNextOreRobot < tLimit)
    {
        var fwd = Forward(s, timeToNextOreRobot.Value + 1);
        next.Add(fwd with { ore = fwd.ore - s.p.ore.oreCost, oreRobots = fwd.oreRobots + 1 });
    }

    if (timeToNextClayRobot is not null && CanUseMoreClayRobots(s) && s.m + timeToNextClayRobot < tLimit)
    {
        var fwd = Forward(s, timeToNextClayRobot.Value + 1);
        next.Add(fwd with { ore = fwd.ore - s.p.clay.oreCost, clayRobots = fwd.clayRobots + 1 });
    }

    if (timeToNextObsidianRobot is not null && CanUseMoreObsidianRobots(s) && s.m + timeToNextObsidianRobot < tLimit)
    {
        var fwd = Forward(s, timeToNextObsidianRobot.Value + 1);
        next.Add(fwd with { ore = fwd.ore - s.p.obsidian.oreCost, clay = fwd.clay - s.p.obsidian.clayCost, obsidianRobots = fwd.obsidianRobots + 1 });
    }

    if (timeToNextGeodeRobot is not null && s.m + timeToNextGeodeRobot <= tLimit)
    {
        var fwd = Forward(s, timeToNextGeodeRobot.Value + 1);
        next.Add(fwd with { ore = fwd.ore - s.p.geode.oreCost, obsidian = fwd.obsidian - s.p.geode.obsidianCost, geodeRobots = fwd.geodeRobots + 1 });
    }

    return next.ToArray();
}

string ToString(State s) => $"{s.m} {s.oreRobots} {s.clayRobots} {s.obsidianRobots} {s.geodeRobots} {s.ore} {s.clay} {s.obsidian} {s.openGeodes}";

int HypotheticalMaxOpenGeodes(State s, int tLimit)
{
    var timeLeft = tLimit - s.m;
    var withCurrentGeodeRobots = s.geodeRobots * timeLeft;
    var withAnExtraGeodeRobotEveryMinute = ((timeLeft - 1) * ((timeLeft - 1) + 1)) / 2;

    return s.openGeodes + withCurrentGeodeRobots + withAnExtraGeodeRobotEveryMinute;
}

State? Run(State s, int tLimit)
{
    var max = 0;
    State? winningState = null;
    var next = new Stack<State>();
    var visited = new HashSet<string>();
    next.Push(s);
    var i = 0;
    while (next.Count > 0)
    {
        var n = next.Pop();
        var key = ToString(n);
        if (visited.Contains(key)) continue;
        if (n.m > tLimit) continue;
        if (n.m == tLimit)
        {
            if (max < n.openGeodes)
            {
                max = n.openGeodes;
                winningState = n;
            }
            
            continue;
        }
        var hypotheticalMax = HypotheticalMaxOpenGeodes(n, tLimit);
        if (hypotheticalMax < max)
        {
            continue;
        }
        
        var adj = Adj(n, tLimit);
        foreach (var a in adj)
        {
            next.Push(a);
        }
        visited.Add(key);
        i++;
    }

    Console.WriteLine($"iterations: {i}");
    return winningState;
}

void PrintState(State s)
{
    if (s.prev is not null)
    {
        PrintState(s.prev);
    }

    Console.WriteLine($"{s.m, 2}: {s.oreRobots, 2} {s.clayRobots, 2} {s.obsidianRobots, 2} {s.geodeRobots, 2} - {s.ore, 2} {s.clay, 2} {s.obsidian, 2} {s.openGeodes, 2}");
}

var bps = File.ReadAllLines(args[0]).Select(Parse).ToArray();

// var r = 0;
// foreach (var bp in bps.Skip(0))
// {
//     var init = Init(bp);
//     Console.WriteLine($"Blueprint {bp.id}");
//     var winner = Run(init, 24);
//     var openGeodes = winner?.openGeodes ?? 0;
//     Console.WriteLine(openGeodes);
//     // if (winner != null) PrintState(winner);
//     Console.WriteLine();
//     r += (openGeodes * bp.id);
// }
// Console.WriteLine(r);

var r = new List<int>();
foreach (var bp in bps.Take(3))
{
    var init = Init(bp);
    Console.WriteLine($"Blueprint {bp.id}");
    var winner = Run(init, 32);
    var openGeodes = winner?.openGeodes ?? 0;
    Console.WriteLine(openGeodes);
    if (winner != null) PrintState(winner);
    Console.WriteLine();
    r.Add(openGeodes);
}
Console.WriteLine(r.Aggregate((s, x) => s * x));


public record OreRobot(int oreCost);
public record ClayRobot(int oreCost);
public record ObsidianRobot(int oreCost, int clayCost);
public record GeodeRobot(int oreCost, int obsidianCost);

public record Blueprint(
    int id,
    OreRobot ore, 
    ClayRobot clay, 
    ObsidianRobot obsidian, 
    GeodeRobot geode);

public record MaxNeed(
    int ore,
    int clay,
    int obsidian
);

public record State(
    Blueprint p,
    State? prev,
    int m,
    int ore,
    int clay,
    int obsidian,
    int oreRobots,
    int clayRobots,
    int obsidianRobots,
    int geodeRobots,
    int openGeodes
);



