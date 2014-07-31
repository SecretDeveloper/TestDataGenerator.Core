using System;

namespace gk.DataGenerator.Generators
{
    /// <summary>
    /// Used when a exception needs to be thrown by the test data generator.
    /// </summary>
    public class GenerationException : Exception
    {
        public GenerationException(string message)
            : base(message)
        {
        }
    }
}