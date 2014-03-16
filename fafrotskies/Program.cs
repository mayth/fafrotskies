using System;

namespace fafrotskies
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            int port = 3939;
            var optParser = new NDesk.Options.OptionSet()
            {
                { "port=", v => port = int.Parse(v) }
            };
            var remain = optParser.Parse(args);

            if (remain.Count == 0)
            {
                Console.WriteLine("give me a path to the problem file!");
                return;
            }
            var problemPath = remain[0];
            if (!System.IO.File.Exists(problemPath))
            {
                Console.WriteLine("The specified path '{0}' is not found.", problemPath);
                return;
            }
            var problem = Problem.Load(problemPath);

            Console.WriteLine("Name: {0}", problem.Name);
            Console.WriteLine("Desc: {0}", problem.Description);
            Console.WriteLine("Flag: {0}", problem.Flag);
            Console.WriteLine("Port: {0}", port);
            var server = new Server(port, problem);
            server.Start();
            while (!Console.KeyAvailable) { }
            server.Stop();
        }
    }
}
