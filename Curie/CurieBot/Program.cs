using System;
using Serilog;

namespace CurieBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.AppSettings()
                .CreateLogger();

            var curie = new Curie();
            curie.Init();

            while (Console.ReadLine() != "close") { }

            curie.Stop();
        }
    }
}
