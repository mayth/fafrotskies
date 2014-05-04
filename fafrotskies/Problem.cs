using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization;

namespace Fafrotskies
{
    public class Problem
    {
        public static readonly int DefaultLimit = 10;

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

        private readonly List<Stage> stages;
        public IEnumerable<Stage> Stages
        {
            get { return stages; }
        }

        private readonly CaseGeneratorType caseGeneratorType;
        public CaseGeneratorType CaseGeneratorType
        {
            get { return caseGeneratorType; }
        }

        private readonly List<Case> cases;
        public IEnumerable<Case> Cases
        {
            get
            {
                switch (CaseGeneratorType)
                {
                    case CaseGeneratorType.List:
                        return cases;
                    case CaseGeneratorType.ExternalCommand:
                        throw new InvalidOperationException("Cases list is not available for problem with external generator.");
                    default:
                        throw new InvalidOperationException("Unknown generator type.");
                }
            }
        }

        private readonly string generatorCommand;
        public string GeneratorCommand
        {
            get
            {
                if (CaseGeneratorType != CaseGeneratorType.ExternalCommand)
                    throw new InvalidOperationException();
                return generatorCommand;
            }
        }

        private Problem(string name, string description, IEnumerable<Stage> stages)
        {
            this.name = name;
            this.description = description;
            this.stages = new List<Stage>(stages);
        }

        private Problem(string name, string description, IEnumerable<Stage> stages, IList<Case> cases)
            : this(name, description, stages)
        {
            this.caseGeneratorType = CaseGeneratorType.List;
            this.cases = new List<Case>(cases);
        }

        private Problem(string name, string description, IEnumerable<Stage> stages, string generatorCommand)
            : this(name, description, stages)
        {
            this.caseGeneratorType = CaseGeneratorType.ExternalCommand;
            this.generatorCommand = generatorCommand;
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

            // read limit
            object limitObj;
            int limit;
            if (obj.TryGetValue("limit", out limitObj))
                limit = int.Parse((string)limitObj);
            else
                limit = DefaultLimit;

            var stages = new List<Stage>();
            var stagesObj = obj["stages"] as List<object>;
            if (stagesObj != null)
            {
                foreach (var st in stagesObj)
                {
                    var dict = st as Dictionary<object, object>;
                    if (dict == null)
                        throw new Exception("stage specification invalid (it must be a hash)");

                    int num = int.Parse((string)dict["num"]);

                    object enterMessageObj;
                    string enterMessage = null;
                    if (dict.TryGetValue("enterMessage", out enterMessageObj))
                        enterMessage = (string)enterMessageObj;

                    object clearMessageObj;
                    string clearMessage = null;
                    if (dict.TryGetValue("clearMessage", out clearMessageObj))
                        clearMessage = (string)clearMessage;

                    stages.Add(new Stage(num, enterMessage, clearMessage));
                }
            }

            // try parse `cases` as List
            var yamlCases = obj["cases"] as List<object>;
            if (yamlCases == null)
            {
                // try parse `cases` as Hash
                var yamlCasesHash = obj["cases"] as Dictionary<object, object>;

                if (yamlCasesHash == null)
                    throw new InvalidOperationException("The given yaml's `cases` is not list.");

                // if successfully parsed as Hash, it has external generator.
                return new Problem(
                    (string)obj["name"],
                    (string)obj["description"],
                    stages,
                    (string)yamlCasesHash["cmd"]
                );
            }
            // if successfully parsed as List, it has list of cases.
            var cases = new List<Case>();
            foreach (var dict in yamlCases)
            {
                var d = dict as Dictionary<object, object>;
                if (d == null)
                {
                    throw new InvalidOperationException("Failed to deserialize 'case'.");
                }

                object caseLimitObj;
                int caseLimit;
                if (d.TryGetValue("limit", out caseLimitObj))
                    caseLimit = int.Parse((string)caseLimitObj);
                else
                    caseLimit = limit;

                cases.Add(Case.Create(((string)d["problem"]).Trim(), d["answer"], caseLimit));
            }
            return new Problem((string)obj["name"], (string)obj["description"], stages, cases);
        }
    }
}

