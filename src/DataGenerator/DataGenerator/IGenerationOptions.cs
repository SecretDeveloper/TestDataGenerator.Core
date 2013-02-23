using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataGenerator
{
    /// <summary>
    /// Encapsulates the options required to generate a data type.
    /// </summary>
    public abstract class IGenerationOptions
    {
        /// <summary>
        /// Format of the string to be returned.
        /// </summary>
        public string Title { get; set; }

        protected IGenerationOptions()
        {
            Title = string.Empty;
        }

    }
}
