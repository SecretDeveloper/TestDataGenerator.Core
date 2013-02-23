using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace DataGenerator
{
    public class XmlSerializerHelper<T>
    {
        public Type _type;

        public XmlSerializerHelper()
        {
            _type = typeof(T);
        }


        public void Save(string path, object obj)
        {
            using (TextWriter textWriter = new StreamWriter(path))
            {
                XmlSerializer serializer = new XmlSerializer(_type);
                serializer.Serialize(textWriter, obj);
            }

        }

        public T Read(string path)
        {
            T result;
            using (TextReader textReader = new StreamReader(path))
            {
                XmlSerializer deserializer = new XmlSerializer(_type);
                result = (T)deserializer.Deserialize(textReader);
            }
            return result;

        }
    }


}
