using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataGenerator
{
    /// <summary>
    /// Base interface for all datatype generators
    /// </summary>
    public interface IDataTypeGenerator
    {
        /// <summary>
        /// The Type of data generated
        /// </summary>
        Enum.DataTypes DataType { get;}

        /// <summary>
        /// The options that the generator should use to create new values.
        /// </summary>
        IGenerationOptions GenerationOptions { get; set; }

        /// <summary>
        /// Generate the next value
        /// </summary>
        /// <returns></returns>
        string GenerateValue();
    }
}
