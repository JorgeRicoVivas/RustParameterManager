using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SLightParameterManager {
        public class Group {
            public string Name { get; set; } = "";
            public SortedSet<string> Values { get; set; } = new SortedSet<string>();
            public List<Regex> Regexes { get; set; } = new List<Regex>();
        }

}
