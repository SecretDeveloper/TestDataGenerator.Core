using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TestDataGenerator.Core
{
    [DataContract]
    public class GenerationConfig
    {
        private string _seed;
        [DataMember(Name="seed")]
        public string Seed
        {
            get { return _seed; }
            set
            {
                _seed = value;
                if (_seed != null) _Random = new Random(GetHashCode(_seed));
            }
        }

        [DataMember(Name = "patternfiles")]
        public List<string> PatternFiles{get;set;}

        private bool _loadDefaultPatternFile;
        [DataMember(Name = "LoadDefaultPatternFile")]
        public bool LoadDefaultPatternFile
        {
            get { return _loadDefaultPatternFile; }
            set { _loadDefaultPatternFile = value; }
        }

        [DataMember(Name = "NamedPatterns")]
        public NamedPatterns NamedPatterns { get; set; }

        private Random _Random;
        public Random Random
        {
            get { return _Random; }
            set { _Random = value; }
        }

        public GenerationConfig()
        {
            LoadDefaultPatternFile = false;
            PatternFiles = new List<string>();
            NamedPatterns = new NamedPatterns();

            var rand = new Random();
            Seed = rand.Next(int.MinValue, int.MaxValue).ToString();
            
        }
        private int GetHashCode(string value)
        {
            int hash;
            if (int.TryParse(value, out hash)) return hash;
            return hash.GetHashCode();
        }

    }
}