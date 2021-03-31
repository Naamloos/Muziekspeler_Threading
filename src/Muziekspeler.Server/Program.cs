using System;
using System.Threading;
using System.Threading.Tasks;

namespace Muziekspeler.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Muziekspeler Server by de gucci gang");
            var cancellation = new CancellationTokenSource();
            var server = new Server(cancellation);

            Console.WriteLine("Starting server..");
            _ = Task.Run(async() => await server.StartServerLoopAsync());

            while (cancellation.IsCancellationRequested) { }

            Console.WriteLine("Server killed.");
        }
    }
}
