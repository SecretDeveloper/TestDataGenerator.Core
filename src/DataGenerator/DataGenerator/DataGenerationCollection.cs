using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DataGenerator
{
    public class DataGenerationCollection
    {
        public List<IDataTypeGenerator> Items { get; set; }

        public DataGenerationCollection()
        {
            
            this.Items = new List<IDataTypeGenerator>();
        }

        public void Save(string fileName)
        {
            List<IGenerationOptions> options = new List<IGenerationOptions>();
            foreach(var gen in Items)
            {
                options.Add(gen.GenerationOptions);
            }

            //TODO : Ugly, messy - must be a nicer way to handle inherited classes unknown at runtime.
            TextWriter writer = new StreamWriter(fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(List<IGenerationOptions>), new Type[]
                                                                                               {
                                                                                                   typeof(AlphaNumericGenerationOptions), 
                                                                                                   typeof(AutoIncrementGenerationOptions), 
                                                                                                   typeof(NumberRangeGenerationOptions), 
                                                                                                   typeof(TextFixedGenerationOptions), 
                                                                                                   typeof(TextRandomGenerationOptions)
                                                                                               });
            serializer.Serialize(writer, options);
            writer.Close();
            

        }

        public void Load(string fileName)
        {
            List<IGenerationOptions> options = new List<IGenerationOptions>();

            TextReader reader = new StreamReader(fileName);
            XmlSerializer serializer = new XmlSerializer(typeof(List<IGenerationOptions>), new Type[]
                                                                                               {
                                                                                                   typeof(AlphaNumericGenerationOptions), 
                                                                                                   typeof(AutoIncrementGenerationOptions), 
                                                                                                   typeof(NumberRangeGenerationOptions), 
                                                                                                   typeof(TextFixedGenerationOptions), 
                                                                                                   typeof(TextRandomGenerationOptions)
                                                                                               });
            options = (List<IGenerationOptions>)serializer.Deserialize(reader);

            this.Items.Clear();
            foreach(var opt in options)
            {
                IDataTypeGenerator gen =  FactoryGeneratorFromOptions(opt);
                this.Items.Add(gen);
            }
        }

        private IDataTypeGenerator FactoryGeneratorFromOptions(IGenerationOptions option)
        {
            switch (option.GetType().Name)
            {
                case "AlphaNumericGenerationOptions":
                    return new AlphaNumericDataGenerator(option);
                case "AutoIncrementGenerationOptions":
                    return new AutoIncrementDataGenerator(option);
                case "NumberRangeGenerationOptions":
                    return new NumberRangeDataGenerator(option);
                case "TextFixedGenerationOptions":
                    return new TextFixedDataGenerator(option);
                case "TextRandomGenerationOptions":
                    return new TextRandomDataGenerator(option);
                default:
                    throw new ArgumentOutOfRangeException("Unknown type:" + option.GetType().Name);
            }
        }
    }
}
