using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gk.DataGenerator;
using gk.DataGenerator.Generators;
using gk.DataGenerator.Interfaces;
using gk.DataGenerator.Options;
using gk.DataGenerator.Template.Interfaces;
using gk.DataGenerator.Template.StringBased;

namespace gk.DataGeneratorTests
{
    [TestClass]
    public class TemplateTests
    {
        [TestMethod]
        public void Template_String_CanProduceSingleSimpleRepresentation()
        {
            ITemplate template = new StringTemplate();
            template.Value = "this is a simple template {{placeholder}} ";

            IDataGenerator generator = new StringDataGenerator();

            IRepresentationProducer producer = new StringRepresentationProducer();
            IRepresentation representation = producer.ProduceSingle(template, generator);

            Assert.AreEqual(template.Value.Replace("{{placeholder}}", "ABC"), representation.GetString());
        }
    }
}
