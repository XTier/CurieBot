using System;
using Serilog;

namespace CurieBot.Net
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO AV: set up logs
            //Log.Logger = new LoggerConfiguration()
            //    .ReadFrom.AppSettings()
            //    .CreateLogger();

            var bootstrapper = new Bootstrapper();
            bootstrapper.Initialize();

            var curie = bootstrapper.ResolveWorkflow<Curie>();
            curie.Init();

            while (Console.ReadLine() != "close") { }

            curie.Stop();
        }
    }
}
