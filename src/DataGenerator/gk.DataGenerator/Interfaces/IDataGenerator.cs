
namespace gk.DataGenerator.Interfaces
{
    /// <summary>
    /// Base interface for all datatype generators
    /// </summary>
    public interface IDataGenerator
    {
        /// <summary>
        /// Generate the next value
        /// </summary>
        /// <returns></returns>
        string GenerateValue(IGenerationOption option);
    }
}
