using System;
using System.IO;
using System.Linq;

namespace winwslpath
{
    class Program
    {
        static void Main(string[] args)
        {
            var i = 0;
            var isAbsolute = args.Contains("-a") || args.Contains("/a");
            var oldPath = "";
            var newPath = "";
            while (i < args.Length)
            {
                var arg = args[i];
                switch (arg)
                {
                    case "-a":
                    case "/a":
                        i++;
                        continue;
                    case "-w":
                    case "/w":
                        // FIXME: `~` を解決できない
                        oldPath = args[++i];
                        if (isAbsolute) newPath = Path.GetFullPath(oldPath);
                        else newPath = oldPath.Replace('/', '\\');
                        Console.WriteLine(newPath);
                        return;
                    case "-m":
                    case "/m":
                    case "-h":
                    case "/h":
                        ShowUsage();
                        return;
                    case "-u":
                    case "/u":
                    default:
                        // TODO: 分かりやすい Exception
                        oldPath = (arg == "-u" || arg == "/u") ? args[++i] : arg;
                        if (isAbsolute) oldPath = Path.GetFullPath(oldPath);
                        newPath = oldPath.Replace('\\', '/');
                        Console.WriteLine(newPath);
                        return;
                }
            }
            ShowUsage();
        }

        static void ShowUsage()
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