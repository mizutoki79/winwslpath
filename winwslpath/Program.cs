using System;

namespace winwslpath
{
    class Program
    {
        static void Main(string[] args)
        {
            var i = 0;
            do
            {
                var arg = args.Length > 0 ? args[i] : "/h";
                if (arg.IndexOf('-') == 0) arg = "/" + arg.Substring(1, arg.Length - 1);
                switch (arg)
                {
                    case "/h":
                    case "/u":
                        Usage();
                        return;
                    case "/w":
                        return;
                    case "/m":
                        return;
                    default:
                        return;
                }
            } while (++i < args.Length);
        }

        static void Usage()
        {
            Console.WriteLine("winwslpath usage:\n");
            Console.WriteLine("\t/a\tforce result to absolute path format (future use)");
            Console.WriteLine("\t/u\ttranslate from a Windows path to a WSL path (default)");
            Console.WriteLine("\t/w\ttranslate from a WSL path to a Windows path");
            Console.WriteLine("\t/m\ttranslate from a Windows path to a Windows path, with '/' instead of '\\'");
            Console.WriteLine("\t/h\tdisplay usage information");
        }
    }
}