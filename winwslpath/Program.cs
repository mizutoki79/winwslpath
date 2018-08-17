using System;
using System.IO;
using System.Linq;

namespace winwslpath
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return;
            }
            var i = 0;
            var isAbsolute = args.Contains("-a") || args.Contains("/a");
            while (i < args.Length)
            {
                var arg = args[i];
                switch (arg)
                {
                    case "-a":
                    case "/a":
                        i++;
                        continue;
                    case "-u":
                    case "/u":
                        // TODO: 分かりやすい Exception
                        var oldPath = args[++i];
                        if (isAbsolute) oldPath = Path.GetFullPath(oldPath);
                        var newPath = oldPath.Replace('\\', '/');
                        Console.WriteLine(newPath);
                        return;
                    case "-w":
                    case "/w":
                    case "-m":
                    case "/m":
                    case "-h":
                    case "/h":
                    default:
                        Usage();
                        return;
                }
            }
        }

        static void Usage()
        {
            Console.WriteLine("winwslpath usage:\n");
            Console.WriteLine("\t/a\tforce result to absolute path format");
            Console.WriteLine("\t/u\ttranslate from a Windows path to a WSL path (default)");
            // Console.WriteLine("\t/w\ttranslate from a WSL path to a Windows path");
            // Console.WriteLine("\t/m\ttranslate from a Windows path to a Windows path, with '/' instead of '\\'");
            Console.WriteLine("\t/h\tdisplay usage information");
        }
    }
}