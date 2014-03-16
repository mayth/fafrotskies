using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using fafrotskies;

namespace fafrotskies
{
    public class Server
    {
        public int Port { get; private set; }
        public Problem Problem { get; private set; }
        private Task listenTask;
        private bool isRunning;
        private List<Task> communicateTasks;
        private System.Threading.CancellationTokenSource cancelTokenSource;

        public Server(int port, Problem problem)
        {
            this.Port = port;
            this.Problem = problem;
            isRunning = false;
            communicateTasks = new List<Task>();
        }

        public void Start()
        {
            if (isRunning)
            {
                throw new InvalidOperationException("Server is already started.");
            }

            cancelTokenSource = new System.Threading.CancellationTokenSource();
            var listener = new TcpListener(System.Net.IPAddress.Any, Port);
            listener.Start();
            isRunning = true;
            listenTask = Task.Factory.StartNew(
                async () =>
                {
                    while (isRunning)
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        Console.WriteLine("accepted.");
                        communicateTasks.Add(Task.Factory.StartNew(Communicate, client, cancelTokenSource.Token));
                    }
                });
        }

        public void Stop()
        {
            if (isRunning)
            {
                Console.WriteLine("Shutting down listener...");
                isRunning = false;
                listenTask.Wait();
                listenTask.Dispose();
                Console.WriteLine("Closing all connections...");
                cancelTokenSource.Cancel();
                communicateTasks.Clear();
            }
        }

        async void Communicate(object obj)
        {
            var cancelToken = cancelTokenSource.Token;
            var client = (TcpClient)obj;
            var isCleared = true;
            using (client)
            using (var stream = client.GetStream())
            using (var writer = new System.IO.StreamWriter(stream))
            using (var reader = new System.IO.StreamReader(stream))
            {
                writer.WriteLine("Welcome. We are the fafrotskies.");
                writer.WriteLine();
                writer.WriteLine("===== {0} =====", Problem.Name);
                writer.WriteLine(Problem.Description);
                writer.WriteLine();
                writer.Flush();
                foreach (var c in Problem.Cases.Select((v, i) => new {Index = i, Case = v}))
                {
                    if (cancelToken.IsCancellationRequested)
                    {
                        writer.WriteLine("Sorry, the server is now shutting down...");
                        writer.Flush();
                        cancelToken.ThrowIfCancellationRequested();
                    }

                    writer.WriteLine("#{0}", c.Index + 1);
                    writer.WriteLine(c.Case.Problem);
                    writer.Flush();
                    string answer = await reader.ReadLineAsync(c.Case.Limit * 1000, cancelToken);
                    if (answer == null)
                    {
                        writer.WriteLine("oops...");
                        writer.Flush();
                        isCleared = false;
                        break;
                    }
                    if (!c.Case.Check(answer))
                    {
                        writer.WriteLine("wrong answer!");
                        writer.Flush();
                        isCleared = false;
                        break;
                    }
                }
                if (isCleared)
                {
                    writer.WriteLine("Congratulations!");
                    writer.WriteLine("Flag is {0}", Problem.Flag);
                    writer.Flush();
                }
            }
        }
    }
}

