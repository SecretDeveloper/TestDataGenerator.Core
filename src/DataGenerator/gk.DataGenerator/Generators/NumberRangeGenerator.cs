using System;
using System.Data;
using gk.DataGenerator.Interfaces;
using gk.DataGenerator.Options;

namespace gk.DataGenerator.Generators
{
    public class NumberRangeGenerator : IDataGenerator
    {
        #region IDataGenerator Members

        public string GenerateValue(IGenerationOption option)
        {
            if(option == null)
                throw new NoNullAllowedException("GenerateOptions cannot be null.");
            var opt = option as NumberRangeGenerationOption;
            if(opt == null)
                throw new InvalidConstraintException("Cannot cast GenerateOptions to NumberRangeGenerationOption. Unable to continue.");

            var rand = new Random(DateTime.Now.Millisecond);
            return rand.Next(opt.Low, opt.High).ToString();
            
        }

        #endregion
    }
}
