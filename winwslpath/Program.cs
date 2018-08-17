using System;

namespace winwslpath
{
    class Program
    {
        static void Main(string[] args)
        {
            var i = 0;
            if (i < args.Length)
            {
                var arg = args.Length > 0 ? args[i] : "/h";
                switch (arg)
                {
                    case "-u":
                    case "/u":
                        // TODO: 分かりやすい Exception
                        var path = args[++i];
                        var newPath = path.Replace('\\', '/');
                        Console.WriteLine(newPath);
                        break;
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
            // Console.WriteLine("\t/a\tforce result to absolute path format");
            Console.WriteLine("\t/u\ttranslate from a Windows path to a WSL path (default)");
            // Console.WriteLine("\t/w\ttranslate from a WSL path to a Windows path");
            // Console.WriteLine("\t/m\ttranslate from a Windows path to a Windows path, with '/' instead of '\\'");
            Console.WriteLine("\t/h\tdisplay usage information");
        }
    }
}