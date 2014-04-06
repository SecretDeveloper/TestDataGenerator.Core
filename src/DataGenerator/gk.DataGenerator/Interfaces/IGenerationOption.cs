namespace gk.DataGenerator.Interfaces
{
    /// <summary>
    /// Encapsulates the option required to generate a data type.
    /// </summary>
    public abstract class IGenerationOption
    {
        /// <summary>
        /// Format of the string to be returned.
        /// </summary>
        public string Title { get; set; }

        protected IGenerationOption()
        {
            Title = string.Empty;
        }

    }
}
