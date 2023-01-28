using System.Text.RegularExpressions;

List<(int level, string dir, long size)> GetDirSizes(int level, Directory d)
{
    var r = new List<(int level, string dir, long size)>();
    var size = d.files.Select(x => x.size).Sum();
    foreach (var sd in d.directories)
    {
        var dirSizes = GetDirSizes(level + 1, sd);
        size += dirSizes.Single(x => x.level == level +1 && x.dir == sd.name).size;
        r.AddRange(dirSizes);
    }
    r.Add((level, d.name, size));

    return r;
}

var commands = System.IO.File.ReadAllLines(args[0]).Skip(1);
var root = new Directory(null, "/", new List<Directory>(), new List<File>());

var currentDirectory = root;
foreach (var c in commands)
{
    if (!c.StartsWith("$"))
    {
        var dirMatch = Regex.Match(c, "dir (.+)");
        var fileMatch = Regex.Match(c, @"(\d+) (.+)");
        if (dirMatch.Success)
        {
            var dir = new Directory(currentDirectory, dirMatch.Groups[1].Value, new List<Directory>(), new List<File>());
            currentDirectory.directories.Add(dir);
        }
        else if (fileMatch.Success)
        {
            var size = long.Parse(fileMatch.Groups[1].Value);
            var name = fileMatch.Groups[2].Value;
            currentDirectory.files.Add(new File(name, size));
        }
        else
        {
            throw new Exception($"Unexpected line: {c}. Expected file or dir listing.");
        }
    }
    else
    {
        var cdCommandMatch = Regex.Match(c, @"\$ cd (.+)");
        if (cdCommandMatch.Success)
        {
            var dirname = cdCommandMatch.Groups[1].Value;
            if (dirname == "..")
            {
                if (currentDirectory.parent is null) throw new Exception("Unexpected command. Cannot set parent of root as current directory.");
                currentDirectory = currentDirectory.parent;
            }
            else
            {
                var newCd = currentDirectory.directories.Single(x => x.name == dirname);
                currentDirectory = newCd;
            }
        }
    }
}

while (currentDirectory.parent is not null)
{
    currentDirectory = currentDirectory.parent;
}
var fileSizes = GetDirSizes(0, currentDirectory);
var totalSpace = 70000000;
var neededSpace = 30000000;
var usedSpace = fileSizes.Single(x => x.level == 0).size;
var currentlyUnused = totalSpace - usedSpace;
var needToDelete = neededSpace - currentlyUnused;
var r = fileSizes.Where(x => x.size >= needToDelete).OrderBy(x => x.size).First().size;

Console.WriteLine(r);

// var r = fileSizes.Where(x => x.size <= 100000).Select(x => x.size).Sum();
// Console.WriteLine(r);



public record File(string name, long size);
public record Directory(
    Directory? parent,
    string name, 
    List<Directory> directories, 
    List<File> files);