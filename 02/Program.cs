(char op, char pl) Parse(string l) => (l[0], l[2]);

// int Score((char op, char pl) round)
// {
//     var score = 0;
//     switch (round)
//     {
//         case ('A', 'X'):
//             score = 1 + 3;
//             break;
//         case ('A', 'Y'):
//             score = 2 + 6;
//             break;
//         case ('A', 'Z'):
//             score = 3 + 0;
//             break;
//         case ('B', 'X'):
//             score = 1 + 0;
//             break;
//         case ('B', 'Y'):
//             score = 2 + 3;
//             break;
//         case ('B', 'Z'):
//             score = 3 + 6;
//             break;
//         case ('C', 'X'):
//             score = 1 + 6;
//             break;
//         case ('C', 'Y'):
//             score = 2 + 0;
//             break;
//         case ('C', 'Z'):
//             score = 3 + 3;
//             break;
//         default:
//             throw new Exception($"Unexpected value for round: {round}");
//     }

//     return score;
// }

int Score2((char op, char pl) round)
{
    var score = 0;
    switch (round)
    {
        case ('A', 'A'):
            score = 1 + 3;
            break;
        case ('A', 'B'):
            score = 2 + 6;
            break;
        case ('A', 'C'):
            score = 3 + 0;
            break;
        case ('B', 'A'):
            score = 1 + 0;
            break;
        case ('B', 'B'):
            score = 2 + 3;
            break;
        case ('B', 'C'):
            score = 3 + 6;
            break;
        case ('C', 'A'):
            score = 1 + 6;
            break;
        case ('C', 'B'):
            score = 2 + 0;
            break;
        case ('C', 'C'):
            score = 3 + 3;
            break;
        default:
            throw new Exception($"Unexpected value for round: {round}");
    }

    return score;
}

(char op, char pl) ToRound2((char op, char pl) strategy)
{
    var round2 = ('0', '0');
    switch (strategy)
    {
        case ('A', 'X'):
            round2 = ('A', 'C');
            break;
        case ('A', 'Y'):
            round2 = ('A', 'A');
            break;
        case ('A', 'Z'):
            round2 = ('A', 'B');
            break;
        case ('B', 'X'):
            round2 = ('B', 'A');
            break;
        case ('B', 'Y'):
            round2 = ('B', 'B');
            break;
        case ('B', 'Z'):
            round2 = ('B', 'C');
            break;
        case ('C', 'X'):
            round2 = ('C', 'B');
            break;
        case ('C', 'Y'):
            round2 = ('C', 'C');
            break;
        case ('C', 'Z'):
            round2 = ('C', 'A');
            break;
        default:
            throw new Exception($"Unexpected value for round: {strategy}");
    }

    return round2;
}

// var r = File.ReadAllLines(args[0])
//     .Select(Parse)
//     .Select(Score)
//     .Sum();

var r = File.ReadAllLines(args[0])
    .Select(Parse)
    .Select(ToRound2)
    .Select(Score2)
    .Sum();

Console.WriteLine(r);
