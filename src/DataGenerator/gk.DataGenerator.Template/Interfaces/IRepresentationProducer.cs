using System.Collections.Generic;
using gk.DataGenerator.Interfaces;

namespace gk.DataGenerator.Template.Interfaces
{

    /// <summary>
    /// Controller for producing multiple IRepresentations
    /// </summary>
    public interface IRepresentationProducer
    {
        /// <summary>
        /// Produces a a collection of string representation of the source template populated with the provided items.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRepresentation> ProduceAll(ITemplate template, IDataGenerator dataGenerator);

        IRepresentation ProduceSingle(ITemplate template, IDataGenerator generator);
    }

    
    /// <summary>
    /// A template representing the structure data should be mapped into to produce an IRepresentation
    /// </summary>
    public interface ITemplate
    {
        IRepresentation ProduceRepresentation(IDictionary<string, string> items);
        string Value { get; set; }
    }

    /// <summary>
    /// The outcome when data has been applied to a template.
    /// </summary>
    public interface IRepresentation
    {
        string GetString();
    }
}
