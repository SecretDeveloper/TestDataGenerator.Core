using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gk.DataGenerator.Generators;

namespace gk.DataGenerator
{
    public static class FileReader
    {
        public static Dictionary<string, string> GetNamedPattern(string path)
        {
            var result = new Dictionary<string, string>();
            using (var fs = new StreamReader(path))
            {
                var json = fs.ReadToEnd();
                result = (new ServiceStack.Text.JsonSerializer<Dictionary<string, string>>()).DeserializeFromString(json);
            }
            return result;
        }

        public static void SerializeDictionary(Dictionary<string, string> dictionary, string path)
        {
            using (var fs = new StreamWriter(path))
            {
                var json = new ServiceStack.Text.JsonSerializer<Dictionary<string, string>>().SerializeToString(dictionary);
                fs.Write(json);
            }
        }
    }
}
