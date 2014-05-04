using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Fafrotskies;

namespace Fafrotskies
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

                var rand = new Random();
                IEnumerable<Case> cases = null;
                switch (Problem.CaseGeneratorType)
                {
                    case CaseGeneratorType.List:
                        cases = Problem.Cases.OrderBy(_ => rand.Next());
                        break;
                    case CaseGeneratorType.ExternalCommand:
                        break;
                    default:
                        throw new InvalidOperationException("Unknown generator type.");
                }

                var answeredCases = 0;
                foreach (var st in Problem.Stages.Select((v, i) => new { Index = i, Stage = v}))
                {
                    writer.WriteLine("Stage #{0}", st.Index + 1);
                    if (!string.IsNullOrWhiteSpace(st.Stage.EnterMessage))
                        writer.WriteLine(st.Stage.EnterMessage);
                    writer.Flush();

                    IEnumerable<Tuple<int, Case>> stageCases;
                    switch (Problem.CaseGeneratorType)
                    {
                        case CaseGeneratorType.List:
                            stageCases = Enumerable.Range(1, st.Stage.NumberOfCases)
                                .Zip(cases.Skip(answeredCases), (i, v) => Tuple.Create(i, v));
                            break;
                        case CaseGeneratorType.ExternalCommand:
                            stageCases = Enumerable.Range(1, st.Stage.NumberOfCases)
                                .Select(
                                    i => Tuple.Create(i, Case.Create(Problem.DefaultLimit, Problem.GeneratorCommand, st.Index + 1))
                                );
                            break;
                        default:
                            throw new InvalidOperationException("Unknown generator type.");
                    }

                    foreach (var t in stageCases)
                    {
                        if (cancelToken.IsCancellationRequested)
                        {
                            writer.WriteLine("Sorry, the server is now shutting down...");
                            writer.Flush();
                            cancelToken.ThrowIfCancellationRequested();
                        }

                        writer.WriteLine("#{0}", t.Item1);
                        writer.WriteLine(t.Item2.Problem);
                        writer.Flush();

                        var answer = await ReadAnswer(reader, t.Item2.Limit, cancelToken);
                        if (answer == null)
                        {
                            writer.WriteLine("oops...");
                            writer.Flush();
                            isCleared = false;
                            break;
                        }

                        if (!t.Item2.Check(answer))
                        {
                            writer.WriteLine("wrong answer!");
                            writer.Flush();
                            isCleared = false;
                            break;
                        }
                    }

                    if (isCleared)
                    {
                        if (!string.IsNullOrWhiteSpace(st.Stage.ClearMessage))
                        {
                            writer.WriteLine(st.Stage.ClearMessage);
                            writer.Flush();
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private Task<string> ReadAnswer(System.IO.StreamReader reader, int limit, System.Threading.CancellationToken cancelToken)
        {
            return Task.Factory.StartNew<string>(() =>
            {
                var actualTask = Task.Factory.StartNew<string>(() =>
                {
                    var answerBuilder = new StringBuilder();
                    while (true)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                            break;
                        answerBuilder.AppendLine(line.Trim());
                    }

                    return answerBuilder.ToString();
                });

                if (actualTask.Wait(limit * 1000, cancelToken))
                    return actualTask.Result;
                else
                    return null;
            });
        }
    }
}

