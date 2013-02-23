using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using DataGenerator.Enum;

namespace DataGenerator
{
    public class TextRandomGenerationOptions:IGenerationOptions
    {
        #region IGenerationOptions Members

        public string FormatString
        {
            get; set;
        }

        #endregion

        public int MaximumNumberOfWordsToGenerate{ get; set; }
    }

    public class TextRandomDataGenerator:IDataTypeGenerator
    {
        #region IDataTypeGenerator Members

        public DataGenerator.Enum.DataTypes DataType
        {
            get { return DataTypes.TextRandom; }
        }

        public IGenerationOptions GenerationOptions
        {
            get; set;
        }

        public TextRandomDataGenerator()
        {
            var opt = new TextRandomGenerationOptions();
            opt.MaximumNumberOfWordsToGenerate = 50;
            GenerationOptions = opt;
        }

        public TextRandomDataGenerator(IGenerationOptions options)
        {
            GenerationOptions = options;
        }

        public string GenerateValue()
        {
            var rand = new Random(DateTime.Now.Millisecond);
            var lipsum = new NLipsum.Core.LipsumGenerator();
            var words = lipsum.GenerateWords(rand.Next(((TextRandomGenerationOptions)GenerationOptions).MaximumNumberOfWordsToGenerate));
            return string.Join(" ", words);
        }

        #endregion
    }
}
