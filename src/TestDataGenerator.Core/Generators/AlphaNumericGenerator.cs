using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TestDataGenerator.Core.Exceptions;

namespace TestDataGenerator.Core.Generators
{
    public static class AlphaNumericGenerator 
    {
        private static Dictionary<string, string> _ShortHands = GetShortHandMap();

        private const string _Config_Start = "<#";
        private const string _Config_End = "#>";

        private const string _Placeholder_Start = "<<";
        private const string _Placeholder_End = ">>";

        private const char _Section_Start = '(';
        private const char _Section_End = ')';

        private const char _Set_Start = '[';
        private const char _Set_End = ']';

        private const char _Quantifier_Start = '{';
        private const char _Quantifier_End = '}';

        private const char _Alternation = '|';
        private const char _Escape = '\\';
        private const char _Negation = '^';

        private const char _NamedPattern_Start = '@';
        private const char _NamedPattern_End = '@';

        private const string _Anagram = ":anagram:";

        private const int _ErrorSnippet_ContextLength = 50;
        
        /// <summary>
        /// The number of characters before and after the problem location to include in error messages.
        /// Default is 50.
        /// </summary>
        public static int ErrorContext
        {
            get { return _ErrorSnippet_ContextLength; } 
        }

        public static GenerationConfig GetConfiguration(string template, ref int index)
        {
            if (index > 0)
                return null; //throw new GenerationException("Configuration items must be at the start of the template (index 0)");
            
            if (!template.TrimStart().StartsWith(_Config_Start))
                return null;
            
            var configString = GetContent(template, ref index, _Config_Start, _Config_End).ToLowerInvariant().Replace('\'','"');

            GenerationConfig config;
            try
            {
                config = Utility.DeserializeJson<GenerationConfig>(configString);
            }
            catch (Exception ex)
            {
                var msg = BuildErrorSnippet(template, _Config_Start.Length);
                throw new GenerationException(string.Format("Unable to parse configuration string. Please check that all key names are within quotes.\n '{0}'\n{1}", msg, ex.Message));
            }
            return config;
        }

        /// <summary>
        /// Takes in a string that contains 0 or more &lt;&lt;placeholder&gt;&gt; values and replaces the placeholder item with the expression it defines.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static string GenerateFromTemplate(string template)
        {
            return GenerateFromTemplate(template, null, null);
        }

        /// <summary>
        /// Takes in a string that contains 0 or more &lt;&lt;placeholder&gt;&gt; values and replaces the placeholder item with the expression it defines.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GenerateFromTemplate(string template, GenerationConfig config)
        {
            return GenerateFromTemplate(template, null, config);
        }

        /// <summary>
        /// Takes in a string that contains 0 or more &lt;&lt;placeholder&gt;&gt; values and replaces the placeholder item with the expression it defines.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="namedPatterns"></param>
        /// <returns></returns>
        public static string GenerateFromTemplate(string template, NamedPatterns namedPatterns)
        {
            return GenerateFromTemplate(template, namedPatterns, null);
        }
        
        /// <summary>
        /// Takes in a string that contains 0 or more &lt;&lt;placeholder&gt;&gt; values and replaces the placeholder item with the expression it defines.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        public static string GenerateFromTemplate(string template, Random random)
        {
            var config = new GenerationConfig();
            config.Random = random;
            return GenerateFromTemplate(template, null, config);
        }

        /// <summary>
        /// Takes in a string that contains 0 or more &lt;&lt;placeholder&gt;&gt; values and replaces the placeholder item with the expression it defines.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="namedPatterns"></param>
        /// <param name="generationConfig"></param>
        /// <returns></returns>
        public static string GenerateFromTemplate(string template, NamedPatterns namedPatterns, GenerationConfig generationConfig)
        {
            int index = 0;
            
            // CONFIG
            if (generationConfig == null) generationConfig = LoadAndRemoveConfigFromTemplate(ref template);
            if (generationConfig == null) generationConfig = new GenerationConfig();
            
            var random = generationConfig.Random;

            // PATTERNS
            var generationPatterns = LoadDefaultNamedPatterns(generationConfig);
            // Load provided ones as well if there are any.
            if (namedPatterns != null && namedPatterns.Patterns != null)
            {
                namedPatterns.Patterns.ForEach(generationPatterns.Patterns.Add);
            }
            // Load all from the PatternFiles in config
            AppendPatternsFromConfigToCollection(generationConfig, generationPatterns);
            
            var sb = new StringBuilder();

            while (index < template.Length)
            {
                // Find our next placeholder
                int start = FindPositionOfNext(template, index, _Placeholder_Start, _Placeholder_End);
                if (start == -1)
                {
                    sb.Append(template.Substring(index));  //add remaining string.
                    break; // all done!
                }

                bool isEscaped = IsEscaped(template, start); //start >= 1 && template[start - 1].Equals(_Escape);
                if (isEscaped) start = start - 1;
                sb.Append(template.Substring(index, start - index)); // Append everything up to start as it is.
                if (isEscaped) start = start + 1;
                start = start + _Placeholder_Start.Length; // move past '<<' to start of expression
                
                int end = FindPositionOfNext(template, start, _Placeholder_End, _Placeholder_Start); // find end of placeholder
                if (end == -1)
                {
                    throw new GenerationException("Unable to find closing placeholder after "+start);
                }

                var pattern = template.Substring(start, end - start); // grab our expression
                if (isEscaped)
                    sb.Append("<<"+pattern+">>");
                else
                    GenerateFromPattern(sb, pattern, generationPatterns, null, random); // generate value from expression
                index = end+2; // update our index.
            }

            return sb.ToString();
        }

        private static void AppendPatternsFromConfigToCollection(GenerationConfig config, NamedPatterns patternCollection)
        {
            if (config.PatternFiles == null) return;

            foreach (var patternFile in config.PatternFiles)
            {
                var correctedPath = FileReader.GetPatternFilePath(patternFile);

                try
                {
                    var patt = FileReader.LoadNamedPatterns(correctedPath);
                    foreach (var pattern in patt.Patterns)
                    {
                        patternCollection.Patterns.Add(pattern);
                    }
                }
                catch (Exception ex)
                {
                    throw new GenerationException("Error loading PatternFile:" + patternFile + "\n\n" + ex.Message);
                }
            }
        }
        
        private static GenerationConfig LoadAndRemoveConfigFromTemplate(ref string template)
        {
            int index = 0;
            // If we have no provided config attempt to get one from the template.
            GenerationConfig config = GetConfiguration(template, ref index);
            if (config != null)
            {
                // Remove all config sections from template.
                template = template.Substring(index);
            }
            return config;
        }

        private static NamedPatterns LoadDefaultNamedPatterns(GenerationConfig generationConfig)
        {
            var patterns = new NamedPatterns();
            if(generationConfig == null || generationConfig.LoadDefaultPatternFile == false)
                return patterns; // Dont attempt to load default pattern files with every execution -- too slow.

            var path = FileReader.GetPatternFilePath("default");
            if (File.Exists(path))
            {
                patterns = FileReader.LoadNamedPatterns(path, false);
            }

            return patterns;
        }

        public static string GenerateFromPattern(string pattern, NamedPatterns namedPatterns = null, GenerationConfig config = null, Random random = null)
        {
            var sb = new StringBuilder();
            GenerateFromPattern(sb, pattern, namedPatterns, config, random);
            return sb.ToString();
        }

        private static void GenerateFromPattern(StringBuilder sb, string pattern, NamedPatterns namedPatterns=null, GenerationConfig config=null, Random random=null)
        {
            if (pattern == null || string.IsNullOrEmpty(pattern))
                throw new GenerationException("Argument 'pattern' cannot be null.");
            
            if (namedPatterns == null)
            {
                namedPatterns = LoadDefaultNamedPatterns(config);
            }
            if (config != null && config.PatternFiles != null)
            {
                // Load all from the PatternFiles in config
                AppendPatternsFromConfigToCollection(config, namedPatterns);
            }
            if(config == null) config = new GenerationConfig();
            if (random == null) random = config.Random;
            
            ProcessPattern(sb, pattern, namedPatterns, random);
        }
        
        private static int FindPositionOfNext(string template, int index, string toFind, string notBefore)
        {
            bool found = false;
            var ndx = index;
            var notBeforeNdx = index;
            while (!found)
            {
                ndx = template.IndexOf(toFind, ndx, StringComparison.Ordinal);
                if (ndx == -1)break;

                notBeforeNdx = template.IndexOf(notBefore, notBeforeNdx, StringComparison.Ordinal);
                // check if escaped
                /*if (IsEscaped(template, ndx))
                {
                    ndx++; //we found an escaped item, go forward and search again.
                    notBeforeNdx++;
                    continue;
                }
                */
                if (notBeforeNdx > -1 && notBeforeNdx < ndx)
                {
                    BuildErrorSnippet(template, ndx);
                    string msg = @"Found unexpected token '" + notBefore + "' at index '" + notBeforeNdx + "' when seeking '" + toFind + "' starting at index '" + index + "'.";
                    msg = msg + Environment.NewLine + BuildErrorSnippet(template, notBeforeNdx);
                    throw new GenerationException(msg);
                }
                found = true;
            }
            return ndx;
        }

        public static string BuildErrorSnippet(string template, int ndx)
        {
            var templateLength = template.Length;
            var contextBefore = _ErrorSnippet_ContextLength;
            var contextAfter = _ErrorSnippet_ContextLength;

            if (contextBefore > ndx) contextBefore = ndx; // ^__contextBefore_ndx_contextAfter____$
            if (contextAfter + ndx > templateLength) contextAfter = templateLength - ndx-1;

            var line = template.Substring(ndx-contextBefore, contextBefore);
            line += template.Substring(ndx, contextAfter + 1);
            line = line.Replace('\n', ' ').Replace('\r', ' ');
            var indicator = new string(' ', contextBefore) + "^" + new String(' ', (contextAfter<0?0:contextAfter));
            
            return line + Environment.NewLine + indicator;
        }

        private static bool IsEscaped(string template, int ndx)
        {
            int slashes = 0;
            var c = ndx-1;
            while (c >= 0)
            {
                if (template[c] != _Escape) break;

                slashes++;
                c--;
            }
            return (slashes != 0) && slashes%2 != 0;
        }

        /// <summary>
        /// Calculates the content from a repeated symbol when the following form is encountered 's{repeat}' where s is a symbol.
        /// The calculated value is append to sb.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="characters"></param>
        /// <param name="index"></param>
        /// <param name="isEscaped">
        /// True if the the previous character was an escape char.</param>
        /// <param name="random"></param>
        /// <returns></returns>
        private static void AppendRepeatedSymbol(StringBuilder sb, string characters, ref int index, bool isEscaped, Random random)
        {
            var symbol = characters[index++];
            var repeatExpression = GetContent(characters, ref index, _Quantifier_Start.ToString(CultureInfo.InvariantCulture), _Quantifier_End.ToString(CultureInfo.InvariantCulture));
            var repeat = GetRepeatValueFromRepeatExpression(repeatExpression, random);
            
            for (int x = 0; x < repeat; x++)
            {
                AppendCharacterDerivedFromSymbol(sb, symbol, isEscaped, random);
            }
        }

        /// <summary>
        /// Calculates the content from a repeated expression when the following form is enountered '[exp]{repeat}'.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="characters"></param>
        /// <param name="index"></param>
        /// <param name="namedPatterns"></param>
        /// <param name="random"></param>
        private static void AppendContentFromSectionExpression(StringBuilder sb, string characters, ref int index, NamedPatterns namedPatterns, Random random)
        {
            var contentOptions = GetContentOptions(characters, ref index, _Section_Start.ToString(CultureInfo.InvariantCulture), _Section_End.ToString(CultureInfo.InvariantCulture), random);

            var exp = contentOptions.Content;
            if (contentOptions.ContainsAlternation) // contains alternations
            {
                GenerateFromAlternatedPattern(sb, namedPatterns, exp, contentOptions, random);
                return;
            }
            
            for (int x = 0; x < contentOptions.Repeat; x++)
            {
                ProcessPattern(sb, exp, namedPatterns, random);
            }
        }

        private static void ProcessPattern(StringBuilder sb, string exp, NamedPatterns namedPatterns, Random random)
        {
            bool isEscaped = false;
            var curNdx = 0;
            while (curNdx < exp.Length)
            {
                var chx = exp[curNdx];
                if (chx == _Escape)
                {
                    if (isEscaped)
                    {
                        // "\\" handling
                        isEscaped = false;
                    }
                    else
                    {
                        // "...\..." detected, entering escaped symbol handling on next pass.
                        isEscaped = true;
                        curNdx++;
                        continue;    
                    }
                }

                if (!isEscaped && chx == _Section_Start)
                {
                    AppendContentFromSectionExpression(sb, exp, ref curNdx, namedPatterns, random);
                    continue; // skip to next character - index has already been forwarded to new position
                }

                // check are we entering a set pattern that may include a quantifier
                // Format = "[Vv]{4}" = generate 4 random ordered characters comprising of either V or v characters
                if (!isEscaped && chx == _Set_Start)
                {
                    AppendContentFromSetExpression(sb, exp, ref curNdx, random);
                    continue; // skip to next character - index has already been forwarded to new position
                }

                if (!isEscaped && chx == _NamedPattern_Start)
                {
                    AppendContentFromNamedPattern(sb, exp, ref curNdx, namedPatterns, random);
                    continue; 
                }

                // check are we entering a repeat symbol section
                // Format = "L{4}" = repeat L symbol 4 times.
                // Format = "\\{4}" = repeat \ symbol 4 times.
                bool repeatSymbol = curNdx < exp.Length - 1 && exp[curNdx + 1] == _Quantifier_Start;
                if (repeatSymbol)
                {
                    AppendRepeatedSymbol(sb, exp, ref curNdx, isEscaped, random);
                    isEscaped = false;
                    continue; // skip to next character - index has already been forwarded to new position
                }

                AppendCharacterDerivedFromSymbol(sb, chx, isEscaped, random);
                curNdx++; // increment to move to next character.
                isEscaped = false;
            }
        }

        private static void GenerateFromAlternatedPattern(StringBuilder sb, NamedPatterns namedPatterns, string exp, ContentOptions contentOptions, Random random)
        {
            var alternates = exp.Split(_Alternation);
            for (int x = 0; x < contentOptions.Repeat; x++)
            {
                exp = alternates[random.Next(0, alternates.Length)];
                GenerateFromPattern(sb, exp, namedPatterns, null, random);
            }
        }

        private static void GenerateFromAnagramPattern(StringBuilder sb, string exp, ContentOptions contentOptions, Random random)
        {
            for (int x = 0; x < contentOptions.Repeat; x++)
            {
                var arr = exp.ToCharArray();
                arr = arr.OrderBy(r => random.Next()).ToArray();
                foreach(var ch in arr)
                    AppendCharacterDerivedFromSymbol(sb, ch, false, random);
            }
        }

        private static void AppendContentFromNamedPattern(StringBuilder sb, string characters, ref int index, NamedPatterns namedPatterns, Random random)
        {
            var ndx = index;
            var tuple = GetContentOptions(characters, ref index, _NamedPattern_Start.ToString(CultureInfo.InvariantCulture), _NamedPattern_End.ToString(CultureInfo.InvariantCulture), random);
            
            if (namedPatterns.HasPattern(tuple.Content)) // $namedPattern;
            {
                GenerateFromPattern(sb, namedPatterns.GetPattern(tuple.Content).Pattern, namedPatterns, null, random);
            }
            else
            {
                var msg = BuildErrorSnippet(characters, ndx);
                msg = "Found named pattern placeholder '" + tuple.Content + "' but was not provided with a corresponding pattern Check Named Patterns file." + Environment.NewLine + msg;
                throw new GenerationException(msg);
            }
        }

        /// <summary>
        /// Calculates the content from a set expression when the following form is enountered '[exp]{repeat}'.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="characters"></param>
        /// <param name="index"></param>
        /// <param name="random"></param>
        private static void AppendContentFromSetExpression(StringBuilder sb, string characters, ref int index, Random random)
        {
            var contentOptions = GetContentOptions(characters, ref index, _Set_Start.ToString(CultureInfo.InvariantCulture), _Set_End.ToString(CultureInfo.InvariantCulture), random);

            if (contentOptions.IsAnagram)
            {
                GenerateFromAnagramPattern(sb, contentOptions.Content, contentOptions, random);
                return;
            }

            if (contentOptions.Content.Contains("-")) // Ranged - [0-7] or [a-z] or [1-9A-Za-z] for fun.
            {
                MatchCollection ranges = new Regex(@"[A-Za-z]-[A-Za-z]|\d+\.?\d*-\d+\.?\d*|.").Matches(contentOptions.Content);
                if (contentOptions.IsNegated)
                {
                    var possibles = _ShortHands["."];
                    foreach (var range in ranges)
                    {
                        if (range.ToString().Contains("-"))
                        {
                            //TODO - Cleanup - Only here to throw an exception for invalid ranges
                            GetRandomCharacterFromRange(range.ToString(), contentOptions.IsNegated, random);
                            var regex = new Regex("[" + range + "]");
                            possibles = regex.Replace(possibles, "");
                        }
                        else
                        {
                            // single character item to negate, just remove it from the list of possibles.
                            possibles = possibles.Replace(range.ToString(), "");
                        }
                    }

                    for (int i = 0; i < contentOptions.Repeat; i++)
                    {
                        sb.Append(possibles[random.Next(0, possibles.Length)]);
                    }
                }
                else
                {
                    for (int i = 0; i < contentOptions.Repeat; i++)
                    {
                        var range = ranges[random.Next(0, ranges.Count)].ToString();
                        sb.Append(range.Contains("-") ? GetRandomCharacterFromRange(range, contentOptions.IsNegated, random) : range);
                    }
                }
            }
            else
            {
                var possibles = contentOptions.Content;
                if (contentOptions.IsNegated)
                    possibles = Regex.Replace(_ShortHands["."], "[" + possibles + "]", "");

                for (int i = 0; i < contentOptions.Repeat; i++)
                {
                    sb.Append(possibles[random.Next(0, possibles.Length)]);
                }
            }
        }
        
        /// <summary>
        /// Recieves a "A-Z" type string and returns the appropriate list of characters.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="isNegated"></param>
        /// <param name="random"></param>
        /// <returns></returns>
        private static string GetRandomCharacterFromRange(string range, bool isNegated, Random random)
        {
            string ret = "";
            string possibles=_ShortHands["."];
            var items = range.Split('-');
            double i = 0;
            if (!Double.TryParse(items[0], out i))
                ret = GetRandomAlphaItemFromRange(isNegated, random, items, possibles);
            else
                ret = GetRandomNumericItemFromRange(isNegated, random, items, possibles);

            return ret;
        }

        private static string GetRandomAlphaItemFromRange(bool isNegated, Random random, string[] items, string possibles)
        {
            string ret = "";
            if (_ShortHands["l"].Contains(items[0])) possibles = _ShortHands["l"];
            if (_ShortHands["L"].Contains(items[0])) possibles = _ShortHands["L"];

            var start = possibles.IndexOf(items[0].ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
            if (start > -1)
            {
                var end = possibles.IndexOf(items[1].ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
                possibles = possibles.Substring(start, end - start + 1);
                if (isNegated) possibles = Regex.Replace(_ShortHands["."],"[" + possibles + "]", "");
                ret = possibles[random.Next(0, possibles.Length)].ToString(CultureInfo.InvariantCulture);
            }
            return ret;
        }

        private static string GetRandomNumericItemFromRange(bool isNegated, Random random, string[] items, string possibles)
        {
            string ret = "";

            double min;
            if (double.TryParse(items[0], NumberStyles.Number, CultureInfo.InvariantCulture, out min))
            {
                double max;
                if (double.TryParse(items[1], NumberStyles.Number, CultureInfo.InvariantCulture, out max))
                {
                    int scale = 0;
                    if (items[0].Contains("."))
                        scale = items[0].Split('.')[1].Length;

                    if (isNegated)
                    {
                        if (scale > 0 || min < 0 || min > 9 || max < 0 || max > 9)
                            throw new GenerationException("Negated numeric sets are restricted to integers between 1 and 9.");

                        var negs = ExpandNegatedMinMax(min, max);
                        possibles = Regex.Replace(possibles, "[" + negs + "]", "");
                        if (possibles.Length == 0) return ""; // No allowable values remain - return empty string
                        ret = possibles[random.Next(0, possibles.Length)].ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        var t = random.NextDouble();
                        min = min + (t*(max - min));
                        ret = min.ToString(GenerateFloatingFormatWithScale(scale), CultureInfo.InvariantCulture);
                    }
                }
            }
            return ret;
        }

        private static string ExpandNegatedMinMax(double min, double max)
        {
            string ret = "";
            for (double i = min; i <= max; i++)
            {
                ret += i.ToString(CultureInfo.InvariantCulture);
            }
            return ret;
        }

        private static string GenerateFloatingFormatWithScale(int scale)
        {
            var t = "0.";
            for (int i = 0; i < scale; i++)
            {
                t += "#";
            }
            return t;
        }

        /// <summary>
        /// Dervives the correct repeat value from the provided expression.
        /// </summary>
        /// <param name="repeatExpression">String in the form of '{n}' or '{n,m}' where n and m are integers</param>
        /// <param name="random"></param>
        /// <returns></returns>
        private static int GetRepeatValueFromRepeatExpression(string repeatExpression, Random random)
        {
            if (string.IsNullOrWhiteSpace(repeatExpression)) return 1; 

            int repeat;
            if (repeatExpression.Contains(","))
            {
                // {min,max} has been provided - parse and get value.
                var vals = repeatExpression.Split(',');
                int min, max;

                if (vals.Length < 2 || !int.TryParse(vals[0], out min) || !int.TryParse(vals[1], out max) || min > max || min < 0)
                {
                    var msg = "Invalid repeat section, random length parameters must be in the format {min,max} where min and max are greater than zero and min is less than max.\n";
                    msg += BuildErrorSnippet(repeatExpression, 0);
                    throw new GenerationException(msg);
                }

                repeat = random.Next(min, max + 1);
            }
            else if (!int.TryParse(repeatExpression, out repeat)) repeat = -1;

            if (repeat < 0)
                throw new GenerationException("Invalid repeat section, repeat value must not be less than zero. '"+repeatExpression+"'");
            return repeat;
        }
        
        private static ContentOptions GetContentOptions(string characters, ref int index, string openingContainerChar, string closingContainerChar, Random random)
        {
            var result = new ContentOptions();
            
            result.Content = GetContent(characters, ref index, openingContainerChar, closingContainerChar);
            
            result.ContainsAlternation = ContainsAlternations(result.Content);

            if (result.Content[0].Equals(_Negation))
            {
                result.IsNegated = true;
                result.Content = result.Content.Replace("^", "");
            }

            if (characters.Length <= index || !characters[index].Equals(_Quantifier_Start)) return result;

            result.QuantifierContent = GetContent(characters, ref index, _Quantifier_Start.ToString(CultureInfo.InvariantCulture), _Quantifier_End.ToString(CultureInfo.InvariantCulture));
            
            // check for special functions
            if (result.QuantifierContent.Contains(":"))
            {
                result.IsAnagram = ContainsAnagram(result.QuantifierContent);
            }
            else
            {
                result.Repeat = GetRepeatValueFromRepeatExpression(result.QuantifierContent, random);    
            }

            return result;
        }

        private static bool ContainsAnagram(string content)
        {
            return content.ToLower().Equals(_Anagram);
        }

        private static string GetContent(string characters, ref int index, string openingContainerChar, string closingContainerChar)
        {
            if(index + openingContainerChar.Length >= characters.Length)
                return "";
            
            int patternStart = index + openingContainerChar.Length;
            var patternEnd = patternStart;
            var sectionDepth = openingContainerChar.Equals(closingContainerChar) ? 0 : 1; // start off inside current section
            while (patternEnd < characters.Length)
            {
                if (characters.Substring(patternEnd, openingContainerChar.Length).Equals(openingContainerChar)) sectionDepth++;
                //check for Alternations in base group.

                if (characters.Substring(patternEnd, closingContainerChar.Length).Equals(closingContainerChar))
                {
                    sectionDepth--;
                    if (sectionDepth == 0) break;
                }
                patternEnd++;
            }

            if (sectionDepth > 0) // make sure we found closing char
            {
                var msg = "Expected '" + closingContainerChar + "' but it was not found." + Environment.NewLine;
                msg += BuildErrorSnippet(characters, patternStart);
                throw new GenerationException(msg);
            }

            int patternLength = patternEnd - patternStart;
            
            index = index + patternLength + openingContainerChar.Length + closingContainerChar.Length; // update index position.
            return characters.Substring(patternStart, patternLength);
        }

        private static bool ContainsAlternations(string characters)
        {
            var patternEnd = 0;
            var sectionDepth = 0;

            next:
            while (patternEnd < characters.Length)
            {
                if (characters[patternEnd].Equals(_Section_Start)) sectionDepth++;
                //check for Alternations in base group.
                if (sectionDepth == 0 && characters[patternEnd].Equals(_Alternation)) return true;

                if (characters[patternEnd].Equals(_Section_End))
                {
                    sectionDepth--;
                    if (sectionDepth == 0) goto next;
                }
                patternEnd++;
            }
            return false;
        }

        private static void AppendCharacterDerivedFromSymbol(StringBuilder sb, char symbol, bool isEscaped, Random random)
        {
            if (!isEscaped)
            {
                sb.Append(symbol); // not a symbol - append as is.
                return;
            }

            if (_ShortHands.ContainsKey(symbol.ToString()))
            {
                AppendRandomCharacterFromString(sb, _ShortHands[symbol.ToString()], random);
            }
            else
            {
                switch (symbol)
                {
                    case 'n':
                        sb.Append("\n");
                        break;
                    case 'r':
                        sb.Append("\r");
                        break;
                    case 't':
                        sb.Append("\t");
                        break;
                    default:
                        // Just append the character as it is not a symbol.
                        sb.Append(symbol);
                        break;
                }
            }
        }

        private static Dictionary<string, string> GetShortHandMap()
        {
            var shorthands = new Dictionary<string, string>();
            shorthands["l"] = "abcdefghijklmnopqrstuvwxyz";
            shorthands["L"] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            shorthands["V"] = "AEIOU";
            shorthands["v"] = "aeiou";
            shorthands["C"] = "BCDFGHJKLMNPQRSTVWXYZ";
            shorthands["c"] = "bcdfghjklmnpqrstvwxyz";
            shorthands["s"] = " \f\n\r\t\v​";
            shorthands["d"] = "0123456789";

            var nonAlphaNonWhiteSpace = ".,;:\"'!&?£$€$%^<>{}[]()*\\+-=@#|~/";
            shorthands["W"] = shorthands["s"] + nonAlphaNonWhiteSpace + " "; // [^A-Za-z0-9_].

            shorthands["a"] = shorthands["l"] + shorthands["L"];
            shorthands["D"] = shorthands["l"] + shorthands["L"] + shorthands["W"];
            shorthands["w"] = shorthands["l"] + shorthands["L"] + shorthands["d"] + "_"; // [A-Za-z0-9_].
            shorthands["S"] = shorthands["l"] + shorthands["L"] + shorthands["d"] + "_" + nonAlphaNonWhiteSpace;  // [^ \f\n\r\t\v​\u00a0\u1680​\u180e\u2000-\u200a​\u2028\u2029​\u202f\u205f​\u3000\ufeff]
            shorthands["."] = shorthands["a"] + shorthands["d"] + shorthands["W"];

            return shorthands;
        }

        private static void AppendRandomCharacterFromString(StringBuilder sb, string allowedCharacters, Random random)
        {
            sb.Append(allowedCharacters[random.Next(allowedCharacters.Length)]);
        }
    }
}
