using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace fafrotskies
{
    public class Server
    {
        private int port;
        private Problem problem;
        private bool isRunning;

        public Server(int port, Problem problem)
        {
            this.port = port;
            this.problem = problem;
            isRunning = false;
        }

        public void Start()
        {
            var listener = new TcpListener(System.Net.IPAddress.Any, port);
            listener.Start();
            isRunning = true;
            Task.Factory.StartNew(
                () =>
                {
                    while (isRunning)
                    {
                        var client = listener.AcceptTcpClient();
                        Console.WriteLine("accepted.");
                        Task.Factory.StartNew(Communicate, client);
                    }
                });
        }

        public void Stop()
        {
            isRunning = false;
        }

        void Communicate(object obj)
        {
            var client = (TcpClient)obj;
            var isCleared = true;
            using (client)
            using (var stream = client.GetStream())
            using (var writer = new System.IO.StreamWriter(stream))
            using (var reader = new System.IO.StreamReader(stream))
            {
                writer.WriteLine("Welcome. We are the fafrotskies.");
                writer.WriteLine();
                writer.WriteLine("===== {0} =====", problem.Name);
                writer.WriteLine(problem.Description);
                writer.WriteLine();
                writer.Flush();
                foreach (var c in problem.Cases.Select((v, i) => new {Index = i, Case = v}))
                {
                    writer.WriteLine("#{0}", c.Index + 1);
                    writer.WriteLine(c.Case.Problem);
                    writer.Flush();
                    var answer = reader.ReadLine();
                    if (!c.Case.Check(answer))
                    {
                        isCleared = false;
                        break;
                    }
                }
                if (isCleared)
                {
                    writer.WriteLine("Congratulations!");
                    writer.WriteLine("Flag is {0}", problem.Flag);
                }
                else
                {
                    writer.WriteLine("Oops...");
                }
                writer.Flush();
            }
        }
    }
}

