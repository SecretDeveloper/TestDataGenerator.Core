using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace gk.DataGenerator
{
    [DataContract]
    public class GenerationConfig
    {
        private int? mSeed;

        [DataMember(Name="seed")]
        public int? Seed
        {
            get { return mSeed; }
            set
            {
                mSeed = value;
                mRandom = new Random(mSeed.Value);
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
            this.mRandom = new Random(DateTime.Now.Millisecond);
            this.mRandom = new Random(mRandom.Next(int.MinValue, int.MaxValue));
            mSeed = mRandom.Next(int.MinValue, int.MaxValue);

            PatternFiles = new List<string>();
        }

    }
}