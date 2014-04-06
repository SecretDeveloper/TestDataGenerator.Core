using gk.DataGenerator.Interfaces;

namespace gk.DataGenerator.Options
{
    public class AutoIncrementGenerationOption:IGenerationOption
    {
        #region IGenerationOption Members

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

        public AutoIncrementGenerationOption()
        {
            StartValue = 1;
            Increment = 1;
        }
    }
}