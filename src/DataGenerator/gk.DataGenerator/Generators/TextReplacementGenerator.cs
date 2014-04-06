using System;
using gk.DataGenerator.Interfaces;

namespace gk.DataGenerator.Generators
{
    public class TextReplacementGenerator:IDataGenerator
    {
        public IGenerationOption GenerationOption { get; set; }
        public string GenerateValue(IGenerationOption option)
        {
            throw new NotImplementedException();
        }
    }
}
