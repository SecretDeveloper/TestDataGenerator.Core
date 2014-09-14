using System.IO;
using System.Text;

namespace gk.DataGenerator.Generators
{
    public static class Utility
    {
        public static T DeserializeJson<T>(string json) where T : class
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var ds = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GenerationConfig));
                return ds.ReadObject(ms) as T;
            }
        }

        public static string SerializeJson<T>(T obj) where T : class
        {
            using (var ms = new MemoryStream())
            {
                var ds = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(GenerationConfig));
                ds.WriteObject(ms, obj);
                ms.Position = 0;
                var sr = new StreamReader(ms);
                return sr.ReadToEnd();
            }
        }
    }
}