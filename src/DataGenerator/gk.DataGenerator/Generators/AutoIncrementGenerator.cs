using System;
using System.Data;
using System.Globalization;
using gk.DataGenerator.Interfaces;
using gk.DataGenerator.Options;

namespace gk.DataGenerator.Generators
{
    public class AutoIncrementGenerator : IDataGenerator
    {
        public string GenerateValue(IGenerationOption option)
        {
            if(option == null)
                throw new ArgumentNullException("option");
            var opt = option as AutoIncrementGenerationOption;
            if(opt == null)
                throw new InvalidConstraintException("Cannot cast GenerateOptions to AutoIncrementGenerationOption. Unable to continue.");

            int val = opt.CurrentValue;
            opt.CurrentValue = opt.CurrentValue + opt.Increment;
            return val.ToString(CultureInfo.InvariantCulture);
        }

    }
}
