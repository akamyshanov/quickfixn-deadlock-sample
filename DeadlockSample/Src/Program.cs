using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using QuickFix;
using QuickFix.FIX44;
using QuickFix.Transport;
using Serilog;

namespace DeadlockSample
{
    class Program
    {
        static void Main(string[] args)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName));
            var mode = args.FirstOrDefault();

            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File($"logs/{mode}-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            IInitiator initiator = null;
            IAcceptor acceptor = null;

            if (mode == "client")
            {
                var clientSettings = new SessionSettings("client.cfg");
                initiator = new SocketInitiator(new ClientApp(100_000), new MemoryStoreFactory(), clientSettings, new FileLogFactory(clientSettings), new MessageFactory());
                initiator.Start();
            }
            else
            {
                var serverSettings = new SessionSettings("server.cfg");
                acceptor = new ThreadedSocketAcceptor(new ServerApp(), new MemoryStoreFactory(), serverSettings, new FileLogFactory(serverSettings), new MessageFactory());
                acceptor.Start();
            }

            Console.ReadLine();
            Console.WriteLine("Bye");

            initiator?.Stop(true);
            acceptor?.Stop(true);
        }
    }
}
