using System;
using System.Data;
using System.Text;
using DataGenerator.Enum;

namespace DataGenerator
{
    public class NumberRangeGenerationOptions:IGenerationOptions
    {
        #region IGenerationOptions Members

        public string FormatString
        {
            get; set;
        }

        #endregion
        
        /// <summary>
        /// The highest number to allow.
        /// </summary>
        public int High { get; set; }
        /// <summary>
        /// The lowest number to allow.
        /// </summary>
        public int Low { get; set; }

        public NumberRangeGenerationOptions()
        {
            High = 1000;
            Low = 1;
        }
    }

    public class NumberRangeDataGenerator : IDataTypeGenerator
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

        public NumberRangeDataGenerator()
        {
            this.GenerationOptions = new NumberRangeGenerationOptions();
        }
        public NumberRangeDataGenerator(IGenerationOptions options)
        {
            this.GenerationOptions = options;
        }

        public string GenerateValue()
        {
            if(GenerationOptions == null)
                throw new NoNullAllowedException("GenerateOptions cannot be null.");
            var opt = GenerationOptions as NumberRangeGenerationOptions;
            if(opt == null)
                throw new InvalidConstraintException("Cannot cast GenerateOptions to NumberRangeGenerationOptions. Unable to continue.");

            var rand = new Random(DateTime.Now.Millisecond);
            return rand.Next(opt.Low, opt.High).ToString();
            
        }

        #endregion
    }
}
