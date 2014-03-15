using System;

namespace fafrotskies
{
    public class Case
    {
        private readonly string problem;
        public string Problem
        {
            get { return problem; }
        }

        private readonly object answer;

        private readonly MatchType matchType;
        public MatchType MatchType
        {
            get { return matchType; }
        }

        private Case(string problem, object answer, MatchType type)
        {
            this.problem = problem;
            this.matchType = type;
            this.answer = answer;
        }

        public static Case Create(string problem, object answer)
        {
            string s = (string)answer;

            int i;
            if (int.TryParse(s, out i))
                return new Case(problem, i, MatchType.Integer);

            double d;
            if (double.TryParse(s, out d))
                return new Case(problem, d, MatchType.Float);

            return new Case(problem, s, MatchType.String);
        }

        public bool Check(object answer)
        {
            switch (MatchType)
            {
                case MatchType.String:
                    return (answer is string && (string)answer == (string)this.answer);
                case MatchType.Integer:
                    return (answer is int && (int)answer == (int)this.answer);
                case MatchType.Float:
                    return (answer is double && (double)answer == (double)this.answer);
            }
            throw new InvalidOperationException("unknown `MatchType`.");
        }
    }
}

