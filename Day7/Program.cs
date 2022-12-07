using System.Text;

namespace Day7
{
    internal class Program
    {
        public class Directory
        {
            public string Name;
            public int Size;
            public List<Directory> Children = new List<Directory>();
            public Directory Parent = null;
        }

        static void Main(string[] args)
        {
            var commands = File.ReadAllLines("input.txt");
            Dictionary<string, int> sizes = new Dictionary<string, int>();

            var root = new Directory()
            {
                Name = "/",
                Size = 0
            };

            var allDirectories = new List<Directory>();
            allDirectories.Add(root);
            var current = root;
            var i = 1;
            while (true)
            {
                if (i >= commands.Length)
                {
                    break;
                }

                switch (commands[i])
                {
                    case String cd when cd.StartsWith("$ cd"):
                        var directory = cd.Substring(5, cd.Length - 5);
                        if (directory == "..")
                        {
                            current = current.Parent;
                        }
                        else
                        {
                            var child = new Directory()
                            {
                                Name = directory,
                                Parent = current
                            };
                            current.Children.Add(child);
                            allDirectories.Add(child);
                            current = child;
                        }
                        break;
                    case String ls when ls.StartsWith("$ ls"):
                        break;
                    case String dir when dir.StartsWith("dir"):
                        break;
                    default:
                        var size = int.Parse(commands[i].Split(' ')[0]);
                        var current2 = current;
                        while (current2 != null)
                        {
                            current2.Size += size;
                            current2 = current2.Parent;
                        }

                        break;
                }
                i++;
            }

            var part1 = allDirectories.Where(dir => dir.Size <= 100000).Sum(dir => dir.Size);

            var free = 70000000 - root.Size;
            var needToFree = 30000000 - free;
            var part2 = allDirectories.Where(dir => dir.Size > needToFree).OrderBy(dir => dir.Size).First().Size;
        }
    }
}

//$ cd /
//$ ls
//dir a
//14848514 b.txt
//8504156 c.dat
//dir d
//$ cd a
//$ ls
//dir e
//29116 f
//2557 g
//62596 h.lst
//$ cd e
//$ ls
//584 i
//$ cd ..
//$ cd ..
//$ cd d
//$ ls
//4060174 j
//8033020 d.log
//5626152 d.ext
//7214296 k