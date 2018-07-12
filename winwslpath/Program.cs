using System;

namespace winwslpath
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Error.WriteLine($"args.Length = {args.Length}");
            if(args.Length >0)
            {
                Console.Error.WriteLine($"args[0] = {args[0]}");
            }
            if (args.Length == 0 || args[0] == "/h" || args[0] == "-h")
            {
                Usage();
            }

        }

        static void Usage()
        {
            Console.WriteLine("winwslpath usage:\n");
            Console.WriteLine("\t/a\tforce result to absolute path format");
            Console.WriteLine("\t/u\ttranslate from a Windows path to a WSL path (default)");
            Console.WriteLine("\t/w\ttranslate from a WSL path to a Windows path");
            Console.WriteLine("\t/m\ttranslate from a Windows path to a Windows path, with '/' instead of '\\'");
            Console.WriteLine("\t/h\tdisplay usage information");
        }
    }
}
