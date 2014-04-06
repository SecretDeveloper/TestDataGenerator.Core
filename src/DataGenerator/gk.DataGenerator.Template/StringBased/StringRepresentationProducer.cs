using System.Collections.Generic;
using gk.DataGenerator.Interfaces;
using gk.DataGenerator.Template.Interfaces;

namespace gk.DataGenerator.Template.StringBased
{
    public class StringRepresentationProducer:IRepresentationProducer
    {
        public IEnumerable<IRepresentation> ProduceAll(ITemplate template, IDataGenerator dataGenerator)
        {
            throw new System.NotImplementedException();
        }

        public IRepresentation ProduceSingle(ITemplate template, IDataGenerator generator)
        {
            throw new System.NotImplementedException();
        }
    }

    public class StringTemplate:ITemplate
    {
        public IRepresentation ProduceRepresentation(IDictionary<string, string> items)
        {
            throw new System.NotImplementedException();
        }

        public string Value { get; set; }
    }

    public class StringDataGenerator:IDataGenerator
    {
        public string GenerateValue(IGenerationOption option)
        {
            throw new System.NotImplementedException();
        }
    }
    
    public class StringRepresentation:IRepresentation
    {
        public string GetString()
        {
            throw new System.NotImplementedException();
        }
    }

}
