using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gk.DataGenerator
{
    public class NamedPatterns
    {
        public string CollectionName { get; set; }

        public List<NamedPattern> Patterns { get; set; } 

        public NamedPatterns()
        {
            this.Patterns = new List<NamedPattern>();
        }

        public NamedPattern GetPattern(string name)
        {
            return Patterns.FirstOrDefault(n => n.Name.Equals(name));
        }
        
        public bool HasPattern(string name)
        {
            return Patterns.Any(n => n.Name.Equals(name));
        }

        public void AddPattern(string name, string pattern)
        {
            Patterns.Add(new NamedPattern(){Name = name, Pattern = pattern});
        }
    }

    public class NamedPattern
    {
        public string Name { get; set; }
        public string Pattern { get; set; }
    }
}
