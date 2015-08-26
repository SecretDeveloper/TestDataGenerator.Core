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
                if (_seed != null) mRandom = new Random(_seed.Value);
            }
        }

        [DataMember(Name = "patternfiles")]
        public List<string> PatternFiles{get;set;}
        
        private Random mRandom;
        public Random Random
        {
            get { return mRandom; }
            set { mRandom = value; }
        }

        public GenerationConfig()
        {
            this.mRandom = new Random();
            _seed = mRandom.Next(int.MinValue, int.MaxValue);

            PatternFiles = new List<string>();
        }

    }
}