using System;

namespace Fafrotskies
{
    public class Case
    {
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

