using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace fafrotskies
{
    public class Problem
    {
        private readonly string name;
        public string Name
        {
            get { return name; }
        }

        private readonly string description;
        public string Description
        {
            get { return description; }
        }

        private readonly string flag;
        public string Flag
        {
            get { return flag; }
        }

        private List<Case> cases;
        public IList<Case> Cases
        {
            get { return cases; }
        }

        private Problem(string name, string description, string flag, IList<Case> cases)
        {
            this.name = name;
            this.description = description;
            this.flag = flag;
            this.cases = new List<Case>(cases);
        }

        public static Problem Load(string path)
        {
            IDictionary<object, object> obj = null;
            using (var reader = new System.IO.StreamReader(path))
            {
                var serializer = new Deserializer();
                obj = serializer.Deserialize(reader) as Dictionary<object, object>;
            }
            if (obj == null)
            {
                throw new InvalidOperationException("The given yaml's root is not hash (dictionary).");
            }
            var yamlCases = obj["cases"] as List<object>;
            if (yamlCases == null)
            {
                throw new InvalidOperationException("The given yaml's `cases` is not list.");
            }
            var cases = new List<Case>();
            foreach (var dict in yamlCases)
            {
                var d = dict as Dictionary<object, object>;
                if (d == null)
                {
                    throw new InvalidOperationException("Failed to deserialize 'case'.");
                }
                cases.Add(Case.Create((string)d["problem"], d["answer"]));
            }
            return new Problem((string)obj["name"], (string)obj["description"], (string)obj["flag"], cases);
        }
    }
}

