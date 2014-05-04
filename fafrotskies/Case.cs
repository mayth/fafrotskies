using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace Fafrotskies
{
    public class Case
    {
        public static int TimeoutForGeneratingCase
        {
            get { return 5000; }
        }

        private readonly string problem;
        public string Problem
        {
            get { return problem; }
        }

        private readonly object answer;

        private readonly int limit;
        public int Limit
        {
            get { return limit; }
        }

        private readonly MatchType matchType;
        public MatchType MatchType
        {
            get { return matchType; }
        }

        private Case(string problem, object answer, int limit, MatchType type)
        {
            this.problem = problem;
            this.answer = answer;
            this.limit = limit;
            this.matchType = type;
        }

        public static Case Create(string problem, object answer, int limit)
        {
            string s = (string)answer;

            int i;
            if (int.TryParse(s, out i))
                return new Case(problem, i, limit, MatchType.Integer);

            double d;
            if (double.TryParse(s, out d))
                return new Case(problem, d, limit, MatchType.Float);

            return new Case(problem, s, limit, MatchType.String);
        }

        public static Case Create(int defaultLimit, string command, int stage)
        {
            var cmd = command.Split(new []{' '}, 2)[0];
            var args = command.Split(new []{' '}, 2)[1];
            args = args.Replace("%s", stage.ToString());
            var info = new System.Diagnostics.ProcessStartInfo(cmd, args);
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            string output;
            using (var p = System.Diagnostics.Process.Start(info))
            {
                output = p.StandardOutput.ReadToEnd();
                if (!p.WaitForExit(TimeoutForGeneratingCase))
                    throw new TimeoutException("Generating case timed out.");
            }
            if (string.IsNullOrWhiteSpace(output))
                throw new Exception("External generator returns empty string.");
            var deserializer = new Deserializer();
            var obj = deserializer.Deserialize(new System.IO.StringReader(output)) as Dictionary<object, object>;
            if (obj == null)
                throw new Exception("Generator returns non-YAML string. (parse failed)");
            var problem = obj["problem"];
            var answer = obj["answer"];
            object limitObj;
            int limit;
            if (obj.TryGetValue("limit", out limitObj))
                limit = int.Parse((string)limitObj);
            else
                limit = defaultLimit;
            return Case.Create(((string)problem).Trim(), answer, limit);
        }

        public bool Check(object answer)
        {
            switch (MatchType)
            {
                case MatchType.String:
                    return answer.ToString() == (string)this.answer;
                case MatchType.Integer:
                    {
                        int i;
                        return int.TryParse(answer.ToString(), out i) && i == (int)this.answer;
                    }
                case MatchType.Float:
                    {
                        double d;
                        return double.TryParse(answer.ToString(), out d) && d == (double)this.answer;
                    }
            }
            throw new InvalidOperationException("unknown `MatchType`.");
        }
    }
}

