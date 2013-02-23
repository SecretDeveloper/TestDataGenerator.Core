using System;
using System.Data;
using System.Text;
using DataGenerator.Enum;

namespace DataGenerator
{
    public class TextFixedGenerationOptions:IGenerationOptions
    {
        #region IGenerationOptions Members

        public string FormatString
        {
            get; set;
        }

        #endregion
        
        public int NumberofWordsToGenerate { get; set; }

        public TextFixedGenerationOptions()
        {
            NumberofWordsToGenerate = 10;
        }
    }

    public class TextFixedDataGenerator : IDataTypeGenerator
    {
        #region IDataTypeGenerator Members

        public DataTypes DataType
        {
            get
            {
                return DataTypes.TextFixed;
            }
        }

        public IGenerationOptions GenerationOptions
        {
            get; set;
        }

        public TextFixedDataGenerator()
        {
            this.GenerationOptions = new TextFixedGenerationOptions();
        }
        public TextFixedDataGenerator(IGenerationOptions options)
        {
            this.GenerationOptions = options;
        }

        public string GenerateValue()
        {
            if(GenerationOptions == null)
                throw new NoNullAllowedException("GenerateOptions cannot be null.");
            var opt = GenerationOptions as TextFixedGenerationOptions;
            if(opt == null)
                throw new InvalidConstraintException("Cannot cast GenerateOptions to TextFixedGenerationOptions. Unable to continue.");

            var lipsum = new NLipsum.Core.LipsumGenerator();
            var words = lipsum.GenerateWords(opt.NumberofWordsToGenerate);
            return string.Join(" ", words);
        }

        #endregion
    }
}
