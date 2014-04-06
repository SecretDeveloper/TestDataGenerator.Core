using gk.DataGenerator.Interfaces;

namespace gk.DataGenerator.Options
{
    public class NumberRangeGenerationOption:IGenerationOption
    {
        #region IGenerationOption Members

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

        public NumberRangeGenerationOption()
        {
            High = 1000;
            Low = 1;
        }
    }
}