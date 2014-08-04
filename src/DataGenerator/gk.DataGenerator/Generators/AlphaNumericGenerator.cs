using System;
using System.Globalization;
using System.Text;

namespace gk.DataGenerator.Generators
{
    public static class AlphaNumericGenerator 
    {
        private static readonly Random Random;

        private const string _AllAllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!£$%^&*_+;'#,./:@~?";

        private const string _AllLowerLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string _AllUpperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _AllLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _VowelUpper = "AEIOU";
        private const string _VowelLower = "aeiou";
        private const string _ConsonantLower = "bcdfghjklmnpqrstvwxyz";
        private const string _ConsonantUpper = "BCDFGHJKLMNPQRSTVWXYZ";
        private const string _Numbers0To9Characters = "0123456789";
        private const string _Numbers1To9Characters = "123456789";

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

                sb.Append(template.Substring(index, start - index)); // Append everything up to start as it is.
                start = start + 2; // move past '((' to start of expression

                int end = FindPositionOfNext(template, start, _Placeholder_End, _Placeholder_Start); // find end of placeholder
                if (end == -1)
                {
                    throw new GenerationException("Unable to find closing placeholder after "+start);
                }

                var pattern = template.Substring(start, end - start); // grab our expression
                if (pattern.IndexOf(_Alternation) > -1) // check for alternates.
                {
                    var exps = pattern.Split(_Alternation);
                    pattern = exps[Random.Next(0, exps.Length)];
                }

                sb.Append(GenerateFromPattern(pattern)); // generate value from expression
                index = end+2; // update our index.
            }

            return sb.ToString();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pattern">
        /// The format of the text produced. 15 random characters is the default.
        /// . - An uppercase or lowercase letter or number.
        /// w - Any Letter.  	
        /// L - An uppercase Letter.  	
        /// l - A lowercase letter. 	
        /// V - An uppercase Vowel.
        /// v - A lowercase vowel.
        /// C - An uppercase Consonant. 	
        /// c - A lowercase consonant. 	
        /// n - Any number, 1-9.
        /// d - Any number, 0-9.
        /// -------
        /// Patterns can be produced:
        /// "\.{10}" will produce a random string 10 characters long.
        /// </param>
        /// <returns></returns>
        public static string GenerateFromPattern(string pattern)
        {
            if(pattern == null)
                throw new GenerationException("Argument 'pattern' cannot be null.");

            var sb = new StringBuilder();
            bool isEscaped = false;
            
            int i = 0; 
            while(i < pattern.Length)
            {
                char ch = pattern[i];

                // check for escape chars for next part
                if (ch == _Escape)
                {
                    if (isEscaped)
                    {
                        sb.Append(@"\");
                        isEscaped = false;
                        i++;
                        continue;
                    }
                    isEscaped = true;
                    i++;
                    continue;
                }

                // check are we entering a repeat pattern section that may include a quantifier
                // Format = "(LL){n,m}" = repeat xx pattern 4 times.
                if (!isEscaped && ch == _Section_Start)
                {
                    AppendContentFromSectionExpression(sb, pattern, ref i);
                    continue; // skip to next character - index has already been forwarded to new position
                }

                // check are we entering a set pattern that may include a quantifier
                // Format = "[Vv]{4}" = generate 4 random ordered characters comprising of either V or v characters
                if (!isEscaped && ch == _Set_Start)
                {
                    AppendContentFromSetExpression(sb, pattern, ref i);
                    continue; // skip to next character - index has already been forwarded to new position
                }

                // check are we entering a repeat symbol section
                // Format = "L{4}" = repeat L symbol 4 times.
                bool repeatSymbol = i < pattern.Length -1 && pattern[i + 1] == _Quantifier_Start;
                if (isEscaped && repeatSymbol)
                {
                    AppendRepeatedSymbol(sb, pattern, ref i, isEscaped);
                    continue; // skip to next character - index has already been forwarded to new position
                }
                
                AppendCharacterDerivedFromSymbol(sb, ch, isEscaped);
                isEscaped = false;
                i++;
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
            var notBeforeNdx = template.IndexOf(notBefore, index, StringComparison.Ordinal);
            if (notBeforeNdx == -1)
                return template.IndexOf(toFind, index, StringComparison.Ordinal);

            var ndx = template.IndexOf(toFind, index, StringComparison.Ordinal);
            if (notBeforeNdx < ndx)
                throw new GenerationException("Found start of new section '" + notBefore + "' at index '" + notBeforeNdx + "' when expecting to find '" + toFind + "' first.");
            return ndx;
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
            string repeatExpression = GetSurroundedContent(characters, ref index, _Quantifier_Start, _Quantifier_End);
            int repeat = GetRepeatValueFromRepeatExpression(repeatExpression);

            if (isEscaped)
            {
                for (int x = 0; x < repeat; x++)
                {
                    AppendCharacterDerivedFromSymbol(sb, symbol, isEscaped);
                }
            }
            else
            {
                for (int x = 0; x < repeat; x++)
                {
                    sb.Append(symbol);
                }
            }
        }

        /// <summary>
        /// Calculates the content from a repeated expression when the following form is enountered '[exp]{repeat}'.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="characters"></param>
        /// <param name="index"></param>
        private static void AppendContentFromSectionExpression(StringBuilder sb, string characters, ref int index)
        {
            var tuple = GetPatternAndRepeatValueFromExpression(characters,_Section_Start, _Section_End, ref index);

            var exp = tuple.Item2;
            if (exp.IndexOf(_Alternation)>-1)
            {
                // alternates in expression 'LL|ll|vv'
                var alternates = exp.Split(_Alternation);
                exp = alternates[Random.Next(0, alternates.Length)];
                sb.Append(GenerateFromPattern(exp));
                return;
            }

            bool isEscaped = false;
            for (int x = 0; x < tuple.Item1; x++)
            {
                foreach (var chx in exp)
                {
                    if (chx == _Escape)
                    {
                        if (isEscaped)
                        {
                            isEscaped = false;
                            sb.Append(chx); // append escaped character.
                            continue;
                        }
                        isEscaped = true;
                        continue;
                    }

                    AppendCharacterDerivedFromSymbol(sb, chx, isEscaped);
                    isEscaped = false;
                }
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
            var tuple = GetPatternAndRepeatValueFromExpression(characters, _Set_Start, _Set_End, ref index);
            var possibles = tuple.Item2.Split();
            for (int i = 0; i < tuple.Item1; i++)
            {
                sb.Append(possibles[Random.Next(0, possibles.Length)]);
            }
        }


        /// <summary>
        /// Returns a tuple containing an integer representing the number of repeats and a string representing the pattern.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static Tuple<int,string> GetPatternAndRepeatValueFromExpression(string characters, char startChar, char endChar, ref int index)
        {
            string pattern = GetSurroundedContent(characters, ref index, startChar, endChar);
            string repeatExpression = GetSurroundedContent(characters, ref index, _Quantifier_Start, _Quantifier_End);

            int repeat;
            repeat = GetRepeatValueFromRepeatExpression(repeatExpression);

            return new Tuple<int, string>(repeat, pattern);
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
                int min = -1, max = -1;

                if (vals.Length < 2 || !int.TryParse(vals[0], out min) || !int.TryParse(vals[1], out max) || min > max || min < 0)
                    throw new GenerationException("Invalid repeat section, random length parameters must be in the format {min,max} where min and max are greater than zero and min is less than max.");

                repeat = Random.Next(min, max + 1);
            }
            else if (!int.TryParse(repeatExpression, out repeat)) repeat = -1;

            if (repeat < 0)
                throw new GenerationException("Invalid repeat section, repeat value must not be less than zero.");
            return repeat;
        }


        private static string GetSurroundedContent(string characters, ref int index, char sectionStartChar, char sectionEndChar)
        {
            if (index == characters.Length)
                return ""; // throw new GenerationException("Expected '" + sectionStartChar + "' at " + index + " but reached end of pattern instead.");
            if (characters[index].Equals(sectionStartChar) == false)
                return ""; // return blank string if expected character is not found.

            int patternStart = index + 1;

            var sectionDepth = 1; // start off inside current section
            var patternEnd = patternStart;
            while (patternEnd < characters.Length)
            {
                if (characters[patternEnd] == sectionStartChar) sectionDepth++;

                if (characters[patternEnd] == sectionEndChar)
                {
                    sectionDepth--;
                    if (sectionDepth == 0) break;
                }
                patternEnd++;
            }
            if (sectionDepth > 0) // make sure we found closing char
                throw new GenerationException("Expected '" + sectionEndChar + "' but it was not found.");

            int patternLength = patternEnd - patternStart;
            if(patternLength <= 0)
                throw new GenerationException("Expected '"+ sectionEndChar +"' but it was not found.");

            index = index + patternLength + 2; // update index position.
            return characters.Substring(patternStart, patternLength);
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
                    AppendRandomCharacterFromString(sb, _AllAllowedCharacters);
                    break;
                case 'w':
                    AppendRandomCharacterFromString(sb, _AllLetters);
                    break;
                case 'L':
                    AppendRandomCharacterFromString(sb, _AllUpperLetters);
                    break;
                case 'l':
                    AppendRandomCharacterFromString(sb, _AllLowerLetters);
                    break;
                case 'V':
                    AppendRandomCharacterFromString(sb, _VowelUpper);
                    break;
                case 'v':
                    AppendRandomCharacterFromString(sb, _VowelLower);
                    break;
                case 'C':
                    AppendRandomCharacterFromString(sb, _ConsonantUpper);
                    break;
                case 'c':
                    AppendRandomCharacterFromString(sb, _ConsonantLower);
                    break;
                case 'd':
                    AppendRandomCharacterFromString(sb, _Numbers0To9Characters);
                    break;
                case 'n':
                    AppendRandomCharacterFromString(sb, _Numbers1To9Characters);
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
