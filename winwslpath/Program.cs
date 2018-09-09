using System;
using System.IO;
using System.Linq;

namespace winwslpath
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: エラーメッセージを分かりやすくする。
            try
            {
                if (args.Length == 0)
                {
                    ShowUsage();
                    return;
                }
                var i = 0;
                var oldPath = string.Empty;
                var newPath = string.Empty;
                // TODO: `~` をWindowsあるいはWSLのホームに変換する選択肢
                var acceptOption = true;
                var isAbsolute = false;
                char? mode = null;
                while (i < args.Length)
                {
                    var arg = args[i];
                    switch (arg)
                    {
                        case "-a":
                        case "/a":
                            if (!acceptOption) throw new ArgumentException($"winwslpath: Invalid argument ({arg})");
                            isAbsolute = true;
                            break;
                        case "-u":
                        case "/u":
                            if (!acceptOption || !string.IsNullOrEmpty(oldPath)) throw new ArgumentException($"winwslpath: Invalid argument ({arg})");
                            mode = 'u';
                            acceptOption = false;
                            break;
                        case "-w":
                        case "/w":
                            if (!acceptOption || !string.IsNullOrEmpty(oldPath)) throw new ArgumentException($"winwslpath: Invalid argument ({arg})");
                            mode = 'w';
                            acceptOption = false;
                            break;
                        case "-m":
                        case "/m":
                            if (!acceptOption || !string.IsNullOrEmpty(oldPath)) throw new ArgumentException($"winwslpath: Invalid argument ({arg})");
                            mode = 'm';
                            acceptOption = false;
                            break;
                        case "-h":
                        case "/h":
                            if (!acceptOption || args.Length != 1) throw new ArgumentException($"winwslpath: Invalid argument ({arg})");
                            ShowUsage();
                            return;
                        default:
                            if (!string.IsNullOrEmpty(oldPath)) throw new ArgumentException($"winwslpath: Invalid argument ({arg})");
                            mode = mode ?? 'u';
                            oldPath = arg;
                            acceptOption = true;
                            break;
                    }
                    i++;
                }

                if (mode == null) throw new ArgumentException("winwslpath: Invalid argument");
                if (string.IsNullOrEmpty(oldPath)) throw new ArgumentException("winwslpath: you must specify a path");
                switch (mode)
                {
                    case 'u':
                        newPath = ConvertWinToWslPath(oldPath, isAbsolute);
                        break;
                    case 'w':
                        newPath = ConvertWslToWinPath(oldPath, isAbsolute);
                        break;
                    case 'm':
                        newPath = ConvertWinToWinPath(oldPath, isAbsolute, "/");
                        break;
                }
                Console.WriteLine(newPath);
            }catch(ArgumentException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        static void ShowUsage()
        {
            Console.WriteLine("winwslpath usage:\n");
            Console.WriteLine("\t/a\tforce result to absolute path format");
            Console.WriteLine("\t/u\ttranslate from a Windows path to a WSL path (default)");
            Console.WriteLine("\t/w\ttranslate from a WSL path to a Windows path");
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