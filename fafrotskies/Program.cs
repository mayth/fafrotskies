using System;
using YamlDotNet.Serialization;

namespace fafrotskies
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("give me a path to the problem file!");
                return;
            }
            var problemPath = args[0];
            if (!System.IO.File.Exists(problemPath))
            {
                Console.WriteLine("The specified path '{0}' is not found.", problemPath);
                return;
            }
            var problem = Problem.Load(problemPath);
            Console.WriteLine("Name: {0}", problem.Name);
            Console.WriteLine("Desc: {0}", problem.Description);
            Console.WriteLine("Flag: {0}", problem.Flag);
            Console.WriteLine("Cases:");
            foreach (var pCase in problem.Cases)
            {
                Console.WriteLine("  Problem: {0}", pCase.Problem);
            }
        }
    }
}
