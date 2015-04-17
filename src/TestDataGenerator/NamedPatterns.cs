using System.Collections.Generic;
using System.Linq;

namespace TestDataGenerator
{
    public class NamedPatterns
    {
        public List<NamedPattern> Patterns { get; set; } 

        public NamedPatterns()
        {
            Patterns = new List<NamedPattern>();
        }

        public NamedPattern GetPattern(string name)
        {
            return Patterns.FirstOrDefault(n => n.Name.Equals(name));
        }
        
        public bool HasPattern(string name)
        {
            return Patterns.Any(n => n.Name.Equals(name));
        }
    }

    public class NamedPattern
    {
        public string Name { get; set; }
        public string Pattern { get; set; }
    }
}
