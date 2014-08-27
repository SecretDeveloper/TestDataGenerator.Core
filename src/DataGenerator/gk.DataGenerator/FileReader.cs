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
                while (!fs.EndOfStream)
                {
                    var line = fs.ReadLine().Trim();
                    if (line.StartsWith("#")) continue;
                    if (!line.Contains('=')) continue;
                    var keyValue = line.Split('=');
                    result.Add(keyValue[0], keyValue[1]);
                }
            }
            return result;
        }
    }
}
