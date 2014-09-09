using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Eloquent;
using gk.DataGenerator.Exceptions;

namespace gk.DataGenerator.Generators
{
    public static class AlphaNumericGenerator 
    {
        private static readonly Random Random;

        private const string _AllPossibleCharacters = _AllLetters + _AllNumbers + _AllNonAlphaNumericCharacters;
        private const string _AllLetters = _ShortHand_l + _ShortHand_L;
        private const string _AllNumbers = "0123456789";
        private const string _AllNonAlphaNumericCharacters = " .,;:\"'!&?£$€$%^<>{}[]()*\\+-=@#_|~/";

        private const string _ShortHand_l = "abcdefghijklmnopqrstuvwxyz"; // \l
        private const string _ShortHand_L = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; // \L
        private const string _ShortHand_V = "AEIOU"; // \V
        private const string _ShortHand_v = "aeiou"; // \v
        private const string _ShortHand_C = "BCDFGHJKLMNPQRSTVWXYZ"; // \C
        private const string _ShortHand_c = "bcdfghjklmnpqrstvwxyz"; // \c
        private const string _ShortHand_D = _AllLetters + _AllNonAlphaNumericCharacters; // \D
        private const string _ShortHand_d = _AllNumbers; // \d
        private const string _ShortHand_W = " .,;:\"'!&?£€$%^<>{}[]()*\\+-=@#|~/";  // \W
        private const string _ShortHand_w = _AllLetters; // \w
        private const string _ShortHand_s = " \t\n\r\f";  // \s
        private const string _ShortHand_S = _ShortHand_l + _ShortHand_L + _AllNumbers + _AllNonAlphaNumericCharacters;  // \S
        
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

        private static int _ErrorSnippet_ContextLength = 50;
        

        /// <summary>
        /// The number of characters before and after the problem location to include in error messages.
        /// Default is 50.
        /// </summary>
        public static int ErrorContext
        {
            get { return _ErrorSnippet_ContextLength; } 
        }


        static AlphaNumericGenerator()
        {
            Random = new Random(DateTime.Now.Millisecond);
        }


        /// <summary>
        /// Takes in a string that contains 0 or more &lt;&lt;placeholder&gt;&gt; values and replaces the placeholder item with the expression it defines.
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        public static string GenerateFromTemplate(string template)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "default.tdg-patterns";
            var namedParameters = FileReader.LoadNamedPatterns(path);
            return GenerateFromTemplate(template, namedParameters);
        }

        /// <summary>
        /// Takes in a string that contains 0 or more &lt;&lt;placeholder&gt;&gt; values and replaces the placeholder item with the expression it defines.
        /// </summary>
        /// <param name="template"></param>
        /// <param name="namedPatterns"></param>
        /// <returns></returns>
        public static string GenerateFromTemplate(string template, NamedPatterns namedPatterns)
        {
            var sb = new StringBuilder();

            int index = 0;
            while (index < template.Length)
            {
                // Find our next placeholder
                int start = FindPositionOfNext(template, index, _Placeholder_Start, _Placeholder_End);
                if (start == -1)
                {
                    sb.Append(template.Substring(index));  //add remaining string.
                    break; // all done!
                }

                bool isEscaped = start > 2 && template[start - 1].Equals(_Escape) && template[start - 2].Equals(_Escape);
                if (isEscaped) start = start - 2;
                sb.Append(template.Substring(index, start - index)); // Append everything up to start as it is.
                if (isEscaped) start = start + 2;
                start = start + 2; // move past '<<' to start of expression
                
                int end = FindPositionOfNext(template, start, _Placeholder_End, _Placeholder_Start); // find end of placeholder
                if (end == -1)
                {
                    throw new GenerationException("Unable to find closing placeholder after "+start);
                }

                var pattern = template.Substring(start, end - start); // grab our expression
                if (isEscaped)
                    sb.Append("<<"+pattern+">>");
                else
                    sb.Append(GenerateFromPattern(pattern, namedPatterns)); // generate value from expression
                index = end+2; // update our index.
            }

            return sb.ToString();
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern">
        ///     The format of the text produced. 15 random characters is the default.
        ///     . - An uppercase or lowercase letter or number.
        ///     w - Any Letter.  	
        ///     L - An uppercase Letter.  	
        ///     l - A lowercase letter. 	
        ///     V - An uppercase Vowel.
        ///     v - A lowercase vowel.
        ///     C - An uppercase Consonant. 	
        ///     c - A lowercase consonant. 	
        ///     n - Any number, 1-9.
        ///     d - Any number, 0-9.
        ///     -------
        ///     Patterns can be produced:
        ///     "\.{10}" will produce a random string 10 characters long.
        /// </param>
        /// <returns></returns>
        public static string GenerateFromPattern(string pattern)
        {
            var path = AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "default.tdg-patterns";
            var namedParameters = FileReader.LoadNamedPatterns(path);
            return GenerateFromPattern(pattern, namedParameters);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern">
        ///     The format of the text produced. 15 random characters is the default.
        ///     . - An uppercase or lowercase letter or number.
        ///     w - Any Letter.  	
        ///     L - An uppercase Letter.  	
        ///     l - A lowercase letter. 	
        ///     V - An uppercase Vowel.
        ///     v - A lowercase vowel.
        ///     C - An uppercase Consonant. 	
        ///     c - A lowercase consonant. 	
        ///     n - Any number, 1-9.
        ///     d - Any number, 0-9.
        ///     -------
        ///     Patterns can be produced:
        ///     "\.{10}" will produce a random string 10 characters long.
        /// </param>
        /// <param name="namedPatterns"></param>
        /// <returns></returns>
        public static string GenerateFromPattern(string pattern, NamedPatterns namedPatterns)
        {
            if(pattern == null)
                throw new GenerationException("Argument 'pattern' cannot be null.");

            var sb = new StringBuilder();
            bool isEscaped = false;
            
            int index = 0; 
            while(index < pattern.Length)
            {
                char ch = pattern[index];

                // check for escape chars for next part
                if (ch == _Escape)
                {
                    if (isEscaped)
                    {
                        sb.Append(@"\");
                        isEscaped = false;
                        index++;
                        continue;
                    }
                    isEscaped = true;
                    index++;
                    continue;
                }

                // check are we entering a repeat pattern section that may include a quantifier
                // Format = "(LL){n,m}" = repeat xx pattern 4 times.
                if (!isEscaped && ch == _Section_Start)
                {
                    AppendContentFromSectionExpression(sb, pattern, ref index, namedPatterns);
                    continue; // skip to next character - index has already been forwarded to new position
                }

                // check are we entering a set pattern that may include a quantifier
                // Format = "[Vv]{4}" = generate 4 random ordered characters comprising of either V or v characters
                if (!isEscaped && ch == _Set_Start)
                {
                    AppendContentFromSetExpression(sb, pattern, ref index);
                    continue; // skip to next character - index has already been forwarded to new position
                }

                // check are we entering a repeat symbol section
                // Format = "L{4}" = repeat L symbol 4 times.
                bool repeatSymbol = index < pattern.Length -1 && pattern[index + 1] == _Quantifier_Start;
                if (repeatSymbol)
                {
                    AppendRepeatedSymbol(sb, pattern, ref index, isEscaped);
                    isEscaped = false;
                    continue; // skip to next character - index has already been forwarded to new position
                }

                // check if we have encountered a named pattern.
                if (!isEscaped && ch == _NamedPattern_Start)
                {
                    AppendContentFromNamedPattern(sb, pattern, ref index, namedPatterns);
                    continue;
                }
                
                AppendCharacterDerivedFromSymbol(sb, ch, isEscaped);
                isEscaped = false;
                index++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="index"></param>
        /// <param name="toFind"></param>
        /// <param name="notBefore"></param>
        /// <returns></returns>
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
                if (IsEscaped(template, ndx))
                {
                    ndx++; //we found an escaped item, go forward and search again.
                    notBeforeNdx++;
                    continue;
                }

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
            var indicator = new string(' ', contextBefore) + "^" + new String(' ', contextAfter);
            
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
        /// <returns></returns>
        private static void AppendRepeatedSymbol(StringBuilder sb, string characters, ref int index, bool isEscaped)
        {
            var symbol = characters[index++];
            var repeatExpression = GetContent(characters, ref index, _Quantifier_Start, _Quantifier_End);
            var repeat = GetRepeatValueFromRepeatExpression(repeatExpression);
            
            for (int x = 0; x < repeat; x++)
            {
                AppendCharacterDerivedFromSymbol(sb, symbol, isEscaped);
            }
        }

        /// <summary>
        /// Calculates the content from a repeated expression when the following form is enountered '[exp]{repeat}'.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="characters"></param>
        /// <param name="index"></param>
        /// <param name="namedPatterns"></param>
        private static void AppendContentFromSectionExpression(StringBuilder sb, string characters, ref int index, NamedPatterns namedPatterns)
        {
            var contentOptions = GetContentOptions(characters, ref index, _Section_Start, _Section_End);

            var exp = contentOptions.Content;
            if (contentOptions.ContainsAlternation) // containse alternations
            {
                GenerateFromAlternatedPattern(sb, namedPatterns, exp, contentOptions);
                return;
            }

            if (contentOptions.IsNegated) // containse alternations
            {
                GenerateFromNegatedSet(sb, contentOptions);
                return;
            }

            bool isEscaped = false;
            for (int x = 0; x < contentOptions.Repeat; x++)
            {
                var curNdx = 0;
                while(curNdx < exp.Length)
                {
                    var chx = exp[curNdx];
                    if (chx == _Escape)
                    {
                        if (isEscaped)
                        {
                            isEscaped = false;
                            sb.Append(chx); // append escaped character.
                            curNdx++;
                            continue;
                        }
                        isEscaped = true;
                        curNdx++;
                        continue;
                    }
                    

                    if (!isEscaped && chx == _Section_Start)
                    {
                        AppendContentFromSectionExpression(sb, exp, ref curNdx, namedPatterns);
                        continue; // skip to next character - index has already been forwarded to new position
                    }

                    // check are we entering a set pattern that may include a quantifier
                    // Format = "[Vv]{4}" = generate 4 random ordered characters comprising of either V or v characters
                    if (!isEscaped && chx == _Set_Start)
                    {
                        AppendContentFromSetExpression(sb, exp, ref curNdx);
                        continue; // skip to next character - index has already been forwarded to new position
                    }

                    if (!isEscaped && chx == _NamedPattern_Start)
                    {
                        AppendContentFromNamedPattern(sb, exp, ref curNdx, namedPatterns);
                        continue;
                    }

                    // check are we entering a repeat symbol section
                    // Format = "L{4}" = repeat L symbol 4 times.
                    bool repeatSymbol = curNdx < exp.Length - 1 && exp[curNdx + 1] == _Quantifier_Start;
                    if (repeatSymbol)
                    {
                        AppendRepeatedSymbol(sb, exp, ref curNdx, isEscaped);
                        isEscaped = false;
                        continue; // skip to next character - index has already been forwarded to new position
                    }

                    AppendCharacterDerivedFromSymbol(sb, chx, isEscaped);
                    curNdx++; // increment to move to next character.
                    isEscaped = false;
                }
            }
        }
        
        private static void GenerateFromAlternatedPattern(StringBuilder sb, NamedPatterns namedPatterns, string exp, ContentOptions contentOptions)
        {
            var alternates = exp.Split(_Alternation);
            for (int x = 0; x < contentOptions.Repeat; x++)
            {
                exp = alternates[Random.Next(0, alternates.Length)];
                sb.Append(GenerateFromPattern(exp, namedPatterns));
            }
        }

        private static void AppendContentFromNamedPattern(StringBuilder sb, string characters, ref int index, NamedPatterns namedPatterns)
        {
            var ndx = index;
            var tuple = GetContentOptions(characters, ref index, _NamedPattern_Start, _NamedPattern_End);
            
            if (namedPatterns.HasPattern(tuple.Content)) // $namedPattern;
            {
                sb.Append(GenerateFromPattern(namedPatterns.GetPattern(tuple.Content).Pattern, namedPatterns));
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
        private static void AppendContentFromSetExpression(StringBuilder sb, string characters, ref int index)
        {
            var contentOptions = GetContentOptions(characters, ref index, _Set_Start, _Set_End);
            
            if (contentOptions.Content.Contains("-")) // Ranged - [0-7] or [a-z] or [1-9A-Za-z] for fun.
            {
                MatchCollection ranges = new Regex(@"\D-\D|\d+\.?\d*-\d+\.?\d*").Matches(contentOptions.Content);
                if (contentOptions.IsNegated)
                {
                    var possibles = _AllPossibleCharacters;
                    foreach (var range in ranges)
                    {
                        //TODO - Cleanup - Only here to throw an exception for invalid ranges
                        GetRandomCharacterFromRange(range.ToString(), contentOptions.IsNegated);

                        var regex = new Regex("[" + range.ToString() + "]");
                        possibles = regex.Replace(possibles, "");
                    }

                    for (int i = 0; i < contentOptions.Repeat; i++)
                    {
                        sb.Append(possibles[Random.Next(0, possibles.Length)]);
                    }
                }
                else
                {
                    for (int i = 0; i < contentOptions.Repeat; i++)
                    {
                        var range = ranges[Random.Next(0, ranges.Count)];
                        sb.Append(GetRandomCharacterFromRange(range.ToString(), contentOptions.IsNegated));
                    }
                }
            }
            else
            {
                var possibles = contentOptions.Content;
                if (contentOptions.IsNegated)
                    possibles = _AllPossibleCharacters.RegexReplace("[" + possibles + "]", "");

                for (int i = 0; i < contentOptions.Repeat; i++)
                {
                    sb.Append(possibles[Random.Next(0, possibles.Length)]);
                }
            }
        }


        private static void GenerateFromNegatedSet(StringBuilder sb, ContentOptions contentOptions)
        {
            var alternates = contentOptions.Content.Replace("^", "");
            var possibles = _AllPossibleCharacters.RegexReplace("[" + alternates + "]", "");
            for (int x = 0; x < contentOptions.Repeat; x++)
            {
                var possible = possibles[Random.Next(0, possibles.Length)];
                sb.Append(possible);
            }
        }

        /// <summary>
        /// Recieves a "A-Z" type string and returns the appropriate list of characters.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="isNegated"></param>
        /// <returns></returns>
        private static string GetRandomCharacterFromRange(string range, bool isNegated)
        {
            string ret = "";
            string possibles;
            var items = range.Split('-');

            var start = _ShortHand_l.IndexOf(items[0].ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
            if (start > -1)
            {
                var end = _ShortHand_l.IndexOf(items[1].ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
                possibles = _ShortHand_l.Substring(start, end - start+1);
                if (isNegated) possibles = _AllPossibleCharacters.RegexReplace("[" + possibles + "]", "");
                ret = possibles[Random.Next(0, possibles.Length)].ToString(CultureInfo.InvariantCulture);
                return ret;
            }

            start = _ShortHand_L.IndexOf(items[0].ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
            if (start > -1)
            {
                var end = _ShortHand_L.IndexOf(items[1].ToString(CultureInfo.InvariantCulture), StringComparison.Ordinal);
                possibles = _ShortHand_L.Substring(start, end - start+1);
                if (isNegated) possibles = _AllPossibleCharacters.RegexReplace("[" + possibles + "]", "");
                ret = possibles[Random.Next(0, possibles.Length)].ToString(CultureInfo.InvariantCulture);
                return ret;
            }
            
            // NUMERIC RANGES
            double min;
            if (double.TryParse(items[0], out min))
            {
                double max;
                if (double.TryParse(items[1], out max))
                {
                    int scale = 0;
                    if (items[0].Contains(".")) 
                        scale = items[0].Split('.')[1].Length;

                    if (isNegated)
                    {
                        if (scale > 0 || min < 0 || min > 9 || max < 0 || max > 9)
                            throw new GenerationException("Negated numeric sets are restricted to integers between 1 and 9.");
                        possibles = ExpandNegatedMinMax(min, max);
                        possibles = "0123456789".RegexReplace("[" + possibles + "]", "");
                        ret = possibles[Random.Next(0, possibles.Length)].ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        var t = Random.NextDouble();
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
        /// <returns></returns>
        private static int GetRepeatValueFromRepeatExpression(string repeatExpression)
        {
            if (string.IsNullOrWhiteSpace(repeatExpression)) return 1; 

            int repeat;
            if (repeatExpression.Contains(","))
            {
                // {min,max} has been provided - parse and get value.
                var vals = repeatExpression.Split(',');
                int min, max;

                if (vals.Length < 2 || !int.TryParse(vals[0], out min) || !int.TryParse(vals[1], out max) || min > max || min < 0)
                    throw new GenerationException("Invalid repeat section, random length parameters must be in the format {min,max} where min and max are greater than zero and min is less than max.");

                repeat = Random.Next(min, max + 1);
            }
            else if (!int.TryParse(repeatExpression, out repeat)) repeat = -1;

            if (repeat < 0)
                throw new GenerationException("Invalid repeat section, repeat value must not be less than zero.");
            return repeat;
        }


        private static ContentOptions GetContentOptions(string characters, ref int index, char openingContainerChar, char closingContainerChar)
        {
            var result = new ContentOptions();
            
            result.Content = GetContent(characters, ref index, openingContainerChar, closingContainerChar);
            if (result.Content[0].Equals(_Negation))
            {
                result.IsNegated = true;
                result.Content = result.Content.Replace("^", "");
            }

            if (characters.Length > index && characters[index].Equals(_Quantifier_Start))
            {
                var repeatExpression = GetContent(characters, ref index, _Quantifier_Start, _Quantifier_End);
                result.Repeat = GetRepeatValueFromRepeatExpression(repeatExpression);
            }

            result.ContainsAlternation = ContainsAlternations(result.Content);

            return result;
        }

        private static string GetContent(string characters, ref int index, char openingContainerChar, char closingContainerChar)
        {
            if (index == characters.Length)
                return "";
            if (characters[index].Equals(openingContainerChar) == false)
                return "";

            int patternStart = index + 1;
            var patternEnd = patternStart;
            var sectionDepth = openingContainerChar.Equals(closingContainerChar) ? 0 : 1; // start off inside current section
            while (patternEnd < characters.Length)
            {
                if (characters[patternEnd] == openingContainerChar) sectionDepth++;
                //check for Alternations in base group.

                if (characters[patternEnd] == closingContainerChar)
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
            
            if (patternLength <= 0)
            {
                var msg = "Expected '" + closingContainerChar + "' but it was not found." + Environment.NewLine;
                msg += BuildErrorSnippet(characters, patternStart);
                throw new GenerationException(msg);
            }

            index = index + patternLength + 2; // update index position.
            return characters.Substring(patternStart, patternLength);
        }

        private static bool ContainsAlternations(string characters)
        {
            var patternEnd = 0;
            var sectionDepth = 0;

            while (patternEnd < characters.Length)
            {
                if (characters[patternEnd].Equals(_Section_Start)) sectionDepth++;
                //check for Alternations in base group.
                if (sectionDepth == 0 && characters[patternEnd].Equals(_Alternation)) return true;

                if (characters[patternEnd].Equals(_Section_End))
                {
                    sectionDepth--;
                    if (sectionDepth == 0) break;
                }
                patternEnd++;
            }
            return false;
        }

        private static void AppendCharacterDerivedFromSymbol(StringBuilder sb, char symbol, bool isEscaped)
        {
            if (!isEscaped)
            {
                sb.Append(symbol); // not a symbol - append as is.
                return;
            }

            switch (symbol)
            {
                case '.':
                    AppendRandomCharacterFromString(sb, _AllPossibleCharacters);
                    break;
                case 'W':
                    AppendRandomCharacterFromString(sb, _ShortHand_W);
                    break;
                case 'w':
                    AppendRandomCharacterFromString(sb, _ShortHand_w);
                    break;
                case 'L':
                    AppendRandomCharacterFromString(sb, _ShortHand_L);
                    break;
                case 'l':
                    AppendRandomCharacterFromString(sb, _ShortHand_l);
                    break;
                case 'V':
                    AppendRandomCharacterFromString(sb, _ShortHand_V);
                    break;
                case 'v':
                    AppendRandomCharacterFromString(sb, _ShortHand_v);
                    break;
                case 'C':
                    AppendRandomCharacterFromString(sb, _ShortHand_C);
                    break;
                case 'c':
                    AppendRandomCharacterFromString(sb, _ShortHand_c);
                    break;
                case 'D':
                    AppendRandomCharacterFromString(sb, _ShortHand_D);
                    break;
                case 'd':
                    AppendRandomCharacterFromString(sb, _ShortHand_d);
                    break;
                case 's':
                    AppendRandomCharacterFromString(sb, _ShortHand_s);
                    break;
                case 'S':
                    AppendRandomCharacterFromString(sb, _ShortHand_S);
                    break;
                case 'n':
                    sb.Append(Environment.NewLine);
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

        private static void AppendRandomCharacterFromString(StringBuilder sb, string allowedCharacters)
        {
            sb.Append(allowedCharacters[Random.Next(allowedCharacters.Length)]);
        }
    }
}
