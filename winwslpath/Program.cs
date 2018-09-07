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
            // TODO: 現状ループすることは a を除いて無い。同時に使えない・不必要に多い引数に対してエクセプションを投げる
            // TODO: `~` をWindowsあるいはWSLのホームに変換する選択肢
            while (i < args.Length)
            {
                var arg = args[i];
                try
                {
                    switch (arg)
                    {
                        case "-a":
                        case "/a":
                            i++;
                            continue;
                        case "-w":
                        case "/w":
                            oldPath = args[++i];
                            newPath = ConvertWslToWinPath(oldPath, isAbsolute);
                            Console.WriteLine(newPath);
                            return;
                        case "-m":
                        case "/m":
                            oldPath = args[++i];
                            newPath = ConvertWinToWinPath(oldPath, isAbsolute, "/");
                            Console.WriteLine(newPath);
                            return;
                        case "-h":
                        case "/h":
                            ShowUsage();
                            return;
                        case "-u":
                        case "/u":
                        default:
                            oldPath = (arg == "-u" || arg == "/u") ? args[++i] : arg;
                            newPath = ConvertWinToWslPath(oldPath, isAbsolute);
                            Console.WriteLine(newPath);
                            return;
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new ArgumentException("Path must be specify", ex);
                }
            }
            ShowUsage();
        }

        static void ShowUsage()
        {
            Console.WriteLine("winwslpath usage:\n");
            Console.WriteLine("\t/a\tforce result to absolute path format");
            Console.WriteLine("\t/u\ttranslate from a Windows path to a WSL path (default)");
            Console.WriteLine("\t/w\ttranslate from a WSL path to a Windows path");
            // TODO: `/w` と `/m` を併用できるようにする
            Console.WriteLine("\t/m\ttranslate from a Windows path to a Windows path, with '/' instead of '\\'");
            Console.WriteLine("\t/h\tdisplay usage information");
        }

        static string ConvertWinToWslPath(string winPath, bool isAbsolute = false)
        {
            var delimiter = "/";
            var wslPath = ConvertWinToWinPath(winPath, isAbsolute, delimiter);
            if (Path.IsPathRooted(wslPath))
            {
                var qualifier = Path.GetPathRoot(wslPath).Trim('/').Trim('\\');
                var newQualifier = String.Format("{0}mnt{0}{1}", delimiter, qualifier.Split(':').First().ToLower());
                wslPath = wslPath.Replace(qualifier, newQualifier);
            }
            wslPath = wslPath.Replace("\\", delimiter);
            return wslPath;
        }

        static string ConvertWslToWinPath(string wslPath, bool isAbsolute = false, string delimiter = "\\")
        {
            var winPath = wslPath;
            var mountRoot = "/mnt/";
            if (winPath.IndexOf(mountRoot) == 0)
            {
                winPath = winPath.Substring(mountRoot.Length);
                var wslQualifier = winPath.Substring(0, winPath.IndexOf('/'));
                var winQualifier = $"{wslQualifier.ToUpper()}:";
                winPath = winPath.Replace(wslQualifier, winQualifier);
            }
            return ConvertWinToWinPath(winPath, isAbsolute, delimiter, "/");
        }

        static string ConvertWinToWinPath(string oldPath, bool isAbsolute = false, string delimiter = "\\", string oldDelimiter = "\\", bool isResolvedHome = false)
        {
            var newPath = oldPath;
            if (isResolvedHome && newPath.IndexOf('~') == 0)
            {
                var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                newPath = newPath.Replace("~", homeDirectory);
            }
            if (isAbsolute) newPath = Path.GetFullPath(newPath);
            if (delimiter != oldDelimiter) newPath = newPath.Replace(oldDelimiter, delimiter);
            return newPath;
        }
    }
}