using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TestDataGenerator.Core
{
    [DataContract]
    public class GenerationConfig
    {
        private int? _seed;

        [DataMember(Name="seed")]
        public int? Seed
        {
            get { return _seed; }
            set
            {
                _seed = value;
                if (_seed != null) _Random = new Random(_seed.Value);
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

        private Random _Random;
        public Random Random
        {
            get { return _Random; }
            set { _Random = value; }
        }

        public GenerationConfig()
        {
            _Random = new Random();
            _seed = _Random.Next(int.MinValue, int.MaxValue);
            _loadDefaultPatternFile = false;

            PatternFiles = new List<string>();
        }

    }
}