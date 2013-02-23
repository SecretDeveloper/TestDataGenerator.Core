using System;
using System.Data;
using System.Text;
using DataGenerator.Enum;

namespace DataGenerator
{
    public class AutoIncrementGenerationOptions:IGenerationOptions
    {
        #region IGenerationOptions Members

        public string FormatString
        {
            get; set;
        }

        #endregion

        private int _StartValue;
        public int StartValue
        {
            get { return _StartValue; }
            set
            {
                _StartValue = value; 
                CurrentValue = value;
            }
        }
        public int Increment { get; set; }
        public int CurrentValue { get; set; }

        public AutoIncrementGenerationOptions()
        {
            StartValue = 1;
            Increment = 1;
        }
    }

    public class AutoIncrementDataGenerator : IDataTypeGenerator
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

        public AutoIncrementDataGenerator()
        {
            this.GenerationOptions = new AutoIncrementGenerationOptions();
        }
        public AutoIncrementDataGenerator(IGenerationOptions options)
        {
            this.GenerationOptions = options;
        }

        public string GenerateValue()
        {
            if(GenerationOptions == null)
                throw new NoNullAllowedException("GenerateOptions cannot be null.");
            var opt = GenerationOptions as AutoIncrementGenerationOptions;
            if(opt == null)
                throw new InvalidConstraintException("Cannot cast GenerateOptions to AutoIncrementGenerationOptions. Unable to continue.");

            int val = opt.CurrentValue;
            opt.CurrentValue = opt.CurrentValue + opt.Increment;
            return val.ToString();
        }

        #endregion
    }
}
