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
            Console.Error.WriteLine(string.Join(" ", args));
            var isAbsolute = args.Contains("-a") || args.Contains("/a");
            Console.Error.WriteLine(isAbsolute);
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
                        // TODO: 分かりやすい Exception を投げる
                        oldPath = (arg == "-u" || arg == "/u") ? args[++i] : arg;
                        newPath = ConvertWinToWslPath(oldPath, isAbsolute);
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

        static string ConvertWinToWslPath(string winPath, bool isAbsolute = false)
        {
            var delimiter = "/";
            var wslPath = ConvertWinToWinPath(winPath, delimiter, isAbsolute);
            if (Path.IsPathRooted(wslPath))
            {
                var qualifier = Path.GetPathRoot(wslPath);
                // TODO: こんな愚直で良いものか？
                var newQualifier = String.Format("{0}mnt{0}{1}{0}", delimiter, qualifier.Split(':').First().ToLower());
                wslPath = wslPath.Replace(qualifier, newQualifier);
            }
            wslPath = wslPath.Replace("\\", delimiter);
            return wslPath;
        }

        static string ConvertWinToWinPath(string oldPath, string delimiter = "\\", bool isAbsolute = false)
        {
            var newPath = oldPath;
            if (newPath.IndexOf('~') == 0)
            {
                var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                newPath = newPath.Replace("~", homeDirectory);
            }
            if (isAbsolute) newPath = Path.GetFullPath(newPath);
            if (delimiter != "\\") newPath.Replace("\\", delimiter);
            return newPath;
        }
    }
}