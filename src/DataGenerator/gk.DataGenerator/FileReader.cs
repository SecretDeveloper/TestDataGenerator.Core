using System.Xml;
using System.Xml.Serialization;

namespace gk.DataGenerator
{
    public static class FileReader
    {
        public static NamedPatterns LoadNamedPatterns(string path)
        {
            var result = new NamedPatterns();
            using (var reader = XmlReader.Create(path))
            {
                var ser = new XmlSerializer(typeof(NamedPatterns));
                result = ser.Deserialize(reader) as NamedPatterns;
            }
            return result;
        }

        public static void SerializeDictionary(NamedPatterns namedPatterns, string path)
        {
            using (var fs = XmlWriter.Create(path, new XmlWriterSettings(){Indent = true,OmitXmlDeclaration = true}))
            {
                var ns = new XmlSerializerNamespaces();
                ns.Add("", "");

                var ser = new XmlSerializer(typeof(NamedPatterns));
                ser.Serialize(fs, namedPatterns, ns);

            }
        }
    }
}
