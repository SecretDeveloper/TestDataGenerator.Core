using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using gk.DataGenerator.Exceptions;

namespace gk.DataGenerator
{
    public static class FileReader
    {
        private const string _PatternFolder_Name = "tdg-patterns";
        private const string _PatternFile_Extension = "tdg-patterns";

        public static NamedPatterns LoadNamedPatterns(string path)
        {
            return LoadNamedPatterns(path, true);
        }

        [ExcludeFromCodeCoverage]
        public static NamedPatterns LoadNamedPatterns(string path, bool throwException)
        {
            var result = new NamedPatterns();
            try
            {
                using (var reader = XmlReader.Create(path))
                {
                    var ser = new XmlSerializer(typeof (NamedPatterns));
                    result = ser.Deserialize(reader) as NamedPatterns;
                }
            }
            catch
            {
                if (throwException) throw;
            }
            return result;
        }

        public static string GetPatternFilePath(string filePath)
        {
            if (!Path.HasExtension(filePath)) filePath = Path.ChangeExtension(filePath, _PatternFile_Extension);

            var paths = new List<string>();
            paths.Add(filePath);

            var rooted = Path.IsPathRooted(filePath);
            if (!rooted)
            {
                // attempt to root relative path files within the current execution directory.
                paths.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath));

                // check if it is within the _PatternFolder_Name folder
                paths.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _PatternFolder_Name, filePath));
            }

            foreach (var path in paths)
            {
                if (File.Exists(path)) return path;
            }

            var msg = String.Format("Unable to find pattern file '{0}'. Searched in the following locations:\n{1}", filePath, String.Join("\n", paths));
            throw new GenerationException(msg);
        }
    }
}
