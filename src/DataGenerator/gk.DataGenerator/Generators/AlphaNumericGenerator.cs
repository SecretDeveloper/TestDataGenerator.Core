using System;
using System.Globalization;
using System.Text;

namespace gk.DataGenerator.Generators
{
    public static class AlphaNumericGenerator 
    {
        private static readonly Random Random;

        private const string AllAllowedCharacters = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!£$%^&*()-=_+;'#:@~,./<>?\| ";
        private const string AllLowerLetters = "abcdefghijklmnopqrstuvwxyz";
        private const string AllUpperLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string AllLetters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string VowelUpper = "AEIOU";
        private const string VowelLower = "aeiou";
        private const string ConsonantLower = "bcdfghjklmnpqrstvwxyz";
        private const string ConsonantUpper = "BCDFGHJKLMNPQRSTVWXYZ";
        private const string Numbers0To9Characters = "0123456789";
        private const string Numbers1To9Characters = "123456789";

        private const string Placeholder_Start = "((";
        private const string Placeholder_End = "))";


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
                int start = FindPositionOfNext(template, index, Placeholder_Start, Placeholder_End);
                if (start == -1)
                {
                    sb.Append(template.Substring(index));  //add remaining string.
                    break; // all done!
                }

                sb.Append(template.Substring(index, start-index)); // Append everything up to start as it is.
                start = start + 2; // move past '((' to start of expression

                int end = FindPositionOfNext(template, start, Placeholder_End, Placeholder_Start); // find end of placeholder
                if (end == -1)
                {
                    throw new GenerationException("Unable to find closing placeholder after "+start);
                }

                var pattern = template.Substring(start, end-start); // grab our expression
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
        /// * - An uppercase or lowercase letter or number.
        /// L - An uppercase Letter.  	
        /// l - A lowercase letter. 	
        /// V - An uppercase Vowel.
        /// v - A lowercase vowel.
        /// C - An uppercase Consonant. 	
        /// c - A lowercase consonant. 	
        /// X - Any number, 1-9.
        /// x - Any number, 0-9.
        /// -------
        /// Patterns can be produced:
        /// "[*]{10}" will produce a random string 10 characters long.
        /// </param>
        /// <returns></returns>
        public static string GenerateFromPattern(string pattern = "[*]{15}")
        {
            if(pattern == null)
                throw new GenerationException("Argument 'pattern' cannot be null.");

            var sb = new StringBuilder();
            bool skipNext = false;
            
            int i = 0; 
            while(i < pattern.Length)
            {
                char ch = pattern[i];

                // check for escape chars for next part
                if (ch == '\\')
                {
                    skipNext = true;
                    i++;
                    continue;
                }

                // check are we entering a repeat pattern section
                // Format = "LL[xx]{4}" = repeat xx pattern 4 times.
                if (ch == '[')
                {
                    AppendContentFromRepeatExpression(sb, pattern, ref i);
                    continue; // skip to next character - index has already been forwarded to new position
                }

                // check are we entering a repeat symbol section
                // Format = "L{4}" = repeat L symbol 4 times.
                bool repeatSymbol = i < pattern.Length -1 && pattern[i + 1] == '{';
                if (repeatSymbol)
                {
                    AppendRepeatedSymbol(sb, pattern, ref i, skipNext);
                    continue; // skip to next character - index has already been forwarded to new position
                }

                // if we are escaping this char then just stick it in as is.
                if (skipNext)
                {
                    skipNext = false;
                    sb.Append(ch);
                    i++;
                    continue;
                }
                
                AppendCharacterDerivedFromSymbol(sb, ch);
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
        /// True if the previous character was escaped and should be added 'as-is'.
        /// False if previous character should be treated as a symbol.</param>
        /// <returns></returns>
        private static void AppendRepeatedSymbol(StringBuilder sb, string characters, ref int index, bool isEscaped)
        {
            var symbol = characters[index++];
            string rs = GetSurroundedContent(characters, ref index, '{', '}');
            int repeat = GetRepeatValueFromRepeatExpression(rs);

            if (!isEscaped)
            {
                for (int x = 0; x < repeat; x++)
                {
                    AppendCharacterDerivedFromSymbol(sb, symbol);
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
        private static void AppendContentFromRepeatExpression(StringBuilder sb, string characters, ref int index)
        {
            var tuple = GetPatternAndRepeatValueFromExpression(characters, ref index);

            bool skipNext = false;
            for (int x = 0; x < tuple.Item1; x++)
            {
                foreach (var chx in tuple.Item2)
                {
                    if (skipNext)
                    {
                        skipNext = false;
                        sb.Append(chx); // append escaped character.
                        continue;
                    }
                    if (chx == '\\')
                    {
                        skipNext = true;
                        continue;
                    }

                    AppendCharacterDerivedFromSymbol(sb, chx);
                }
            }
        }

        /// <summary>
        /// Returns a tuple containing an integer representing the number of repeats and a string representing the pattern.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static Tuple<int,string> GetPatternAndRepeatValueFromExpression(string characters, ref int index)
        {
            string pattern = GetSurroundedContent(characters, ref index, '[', ']');
            string repeatExpression = GetSurroundedContent(characters, ref index, '{', '}');

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
                throw new GenerationException("Expected '" + sectionStartChar + "' at " + index +" but reached end of pattern instead.");
            if (characters[index].Equals(sectionStartChar) == false)
                throw new GenerationException("Expected '" + sectionStartChar + "' at " + index + " but it was not found.");
            
            int patternStart = index+1;

            int patternLength = (characters.IndexOf(sectionEndChar, index)) - patternStart;
            if(patternLength <= 0)
                throw new GenerationException("Expected '"+ sectionEndChar +"' but it was not found.");

            index = index + patternLength + 2; // update index position.
            return characters.Substring(patternStart, patternLength);
        }

        private static void AppendCharacterDerivedFromSymbol(StringBuilder sb, char symbol)
        {
            switch (symbol)
            {
                case '*':
                    AppendRandomCharacterFromString(sb, AllLetters);
                    break;
                case 'L':
                    AppendRandomCharacterFromString(sb, AllUpperLetters);
                    break;
                case 'l':
                    AppendRandomCharacterFromString(sb, AllLowerLetters);
                    break;
                case 'V':
                    AppendRandomCharacterFromString(sb, VowelUpper);
                    break;
                case 'v':
                    AppendRandomCharacterFromString(sb, VowelLower);
                    break;
                case 'C':
                    AppendRandomCharacterFromString(sb, ConsonantUpper);
                    break;
                case 'c':
                    AppendRandomCharacterFromString(sb, ConsonantLower);
                    break;
                case 'X':
                    AppendRandomCharacterFromString(sb, Numbers0To9Characters);
                    break;
                case 'x':
                    AppendRandomCharacterFromString(sb, Numbers1To9Characters);
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
