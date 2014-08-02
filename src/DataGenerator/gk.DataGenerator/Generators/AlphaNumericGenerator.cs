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
                int start = GetNext(template, index, Placeholder_Start, Placeholder_End);
                if (start == -1)
                {
                    sb.Append(template.Substring(index));  //add remaining string.
                    break; // all done!
                }

                sb.Append(template.Substring(index, start-index)); // Append everything up to start as it is.
                start = start + 2; // move past '((' to start of expression

                int end = GetNext(template, start, Placeholder_End, Placeholder_Start); // find end of placeholder
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

        private static int GetNext(string template, int index, string toFind, string notBefore)
        {
            var notBeforeNdx = template.IndexOf(notBefore, index, StringComparison.Ordinal);
            if(notBeforeNdx == -1)
                return template.IndexOf(toFind, index, StringComparison.Ordinal);
            
            var ndx = template.IndexOf(toFind, index, StringComparison.Ordinal);
            if(notBeforeNdx < ndx)
                throw new GenerationException("Found start of new section '"+notBefore+"' at index '" + notBeforeNdx + "' when expecting to find '"+toFind+"' first.");
            return ndx;
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

            var characters = pattern.ToCharArray();
            int i = 0; 
            while(i < characters.Length)
            {
                char ch = characters[i];

                // check for escape chars for next part
                if (ch == '\\')
                {
                    skipNext = true;
                    i++;
                    continue;
                }
                // if we are escaping this char then just stick it in as is.
                if (skipNext)
                {
                    skipNext = false;
                    sb.Append(ch);
                    i++;
                    continue;
                }

                // check are we entering a repeat pattern section
                // Format = "LL[xx]{4}" = repeat xx pattern 4 times.
                if (ch == '[')
                {
                    GetRepeatedPattern(sb, pattern, ref i);
                    continue; // skip to next character - i has already been forwarded to new position
                }

                // check are we entering a repeat symbol section
                // Format = "L{4}" = repeat L symbol 4 times.
                bool repeatSymbol = i < characters.Length -1 && characters[i + 1] == '{';
                if (repeatSymbol)
                {
                    GetRepeatedSymbol(sb, pattern, ref i);
                    continue; // skip to next character - i has already been forwarded to new position
                }

                GenerateStringFromSymbol(sb, ch);
                i++;
            }
            return sb.ToString();
        }

        
        private static string GetRepeatedSymbol(StringBuilder sb, string characters, ref int i)
        {
            var symbol = characters[i++];
            string rs = GetRepeatedPartSection(characters, ref i, '{', '}');
            int repeat = GetRepeatValue(rs);

        
            //ok so we have our pattern, lets repeat it
            for (int x = 0; x < repeat; x++)
            {
                GenerateStringFromSymbol(sb, symbol);
            }

            // i = currentposition + '{}' + string in between + move forward 1
            //i = i + 2 + (end - start) +1 ;  // update index
            return sb.ToString();
        }


        private static void GetRepeatedPattern(StringBuilder sb, string characters, ref int i)
        {
            var tuple = GetRepeatingPatternTuple(characters, ref i);

            for (int x = 0; x < tuple.Item1; x++)
            {
                foreach (var chx in tuple.Item2)
                {
                    GenerateStringFromSymbol(sb, chx);
                }
            }
        }

        /// <summary>
        /// Returns a tuple containing an integer representing the number of repeats and a string representing the pattern.
        /// </summary>
        /// <param name="characters"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private static Tuple<int,string> GetRepeatingPatternTuple(string characters, ref int i)
        {
            string pattern = GetRepeatedPartSection(characters, ref i, '[', ']');
            string rs = GetRepeatedPartSection(characters, ref i, '{', '}');

            int repeat;
            repeat = GetRepeatValue(rs);

            return new Tuple<int, string>(repeat, pattern);
        }

        private static int GetRepeatValue(string rs)
        {
            int repeat;
            if (rs.Contains(","))
            {
                // {min,max} has been provided - parse and get value.
                var vals = rs.Split(',');
                int min = -1, max = -1;

                if (vals.Length < 2 || !int.TryParse(vals[0], out min) || !int.TryParse(vals[1], out max) || min > max || min < 0)
                    throw new GenerationException("Invalid repeat section, random length parameters must be in the format {min,max} where min and max are greater than zero and min is less than max.");

                repeat = Random.Next(min, max + 1);
            }
            else if (!int.TryParse(rs, out repeat)) repeat = -1;

            if (repeat < 0)
                throw new GenerationException("Invalid repeat section, repeat value must not be less than zero.");
            return repeat;
        }


        private static string GetRepeatedPartSection(string characters, ref int i, char sectionStartChar, char sectionEndChar)
        {
            if (i == characters.Length)
                throw new GenerationException("Expected '" + sectionStartChar + "' at " + i +" but reached end of pattern instead.");
            if (characters[i].Equals(sectionStartChar) == false)
                throw new GenerationException("Expected '" + sectionStartChar + "' at " + i + " but it was not found.");
            
            int patternStart = i+1;

            int patternLength = (characters.IndexOf(sectionEndChar, i)) - patternStart;
            if(patternLength <= 0)
                throw new GenerationException("Expected '"+ sectionEndChar +"' but it was not found.");

            i = i + patternLength + 2; // update index position.
            return characters.Substring(patternStart, patternLength);
        }

        private static void GenerateStringFromSymbol(StringBuilder sb, char symbol)
        {
            if(AllAllowedCharacters.IndexOf(symbol) == -1)
                throw new GenerationException("Invalid symbol '" + symbol + "' encountered.");
            
            switch (symbol)
            {
                case '*':
                    GenerateRandomString(sb, AllLetters);
                    break;
                case 'L':
                    GenerateRandomString(sb, AllUpperLetters);
                    break;
                case 'l':
                    GenerateRandomString(sb, AllLowerLetters);
                    break;
                case 'V':
                    GenerateRandomString(sb, VowelUpper);
                    break;
                case 'v':
                    GenerateRandomString(sb, VowelLower);
                    break;
                case 'C':
                    GenerateRandomString(sb, ConsonantUpper);
                    break;
                case 'c':
                    GenerateRandomString(sb, ConsonantLower);
                    break;
                case 'X':
                    GenerateRandomString(sb, Numbers0To9Characters);
                    break;
                case 'x':
                    GenerateRandomString(sb, Numbers1To9Characters);
                    break;
                default:
                    // Just append the character as it is not a symbol.
                    sb.Append(symbol);
                    break;
            }
        }

        private static void GenerateRandomString(StringBuilder sb, string allowedCharacters)
        {
            sb.Append(allowedCharacters[Random.Next(allowedCharacters.Length)]);
        }
    }
}
