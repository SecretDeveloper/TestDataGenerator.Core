using System;
using System.Data;
using System.Text;

namespace gk.DataGenerator.Generators
{
    public static class AlphaNumericGenerator 
    {
        private static Random _random;

        private static readonly string AllAllowedCharacters = @"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!£$%^&*()-=_+;'#:@~,./<>?\|";
        private static readonly string AllCharacters = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string VowelCharacters = "aeiou";
        private static readonly string ConsonantCharacters = "bcdfghjklmnpqrstvwxyz";
        private static readonly string Numbers0To9Characters = "0123456789";
        private static readonly string Numbers1To9Characters = "123456789";

        static AlphaNumericGenerator()
        {
            _random = new Random(DateTime.Now.Millisecond);
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
        public static string Generate(string pattern = "[*]{15}")
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
                // check are we entering a repeat symbol section
                // Format = "LL[xx]{4}" = repeat xx pattern 4 times.
                if (ch == '[')
                {
                    sb.Append(GetRepeatedPart(pattern, ref i));
                    continue; // skip to next character - i has already been forwarded to new position
                }
                sb.Append(GenerateStringFromSymbol(ch));
                i++;
            }
            return sb.ToString();
        }



        private static string GetRepeatedPart(string characters, ref int i)
        {
            var tuple = GetRepeatingPatternTuple(characters, ref i);

            var sb = new StringBuilder();
            //ok so we have our pattern, lets repeat it
            for (int x = 0; x < tuple.Item1; x++)
            {
                foreach (var chx in tuple.Item2)
                {
                    sb.Append(GenerateStringFromSymbol(chx));
                }
            }
            return sb.ToString();
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

            int repeat = 1;
            int.TryParse(rs, out repeat);
            if(repeat < 1)
                throw new GenerationException("Invalid repeat section, repeat value must be an int greater than 0.");

            return new Tuple<int, string>(repeat, pattern);
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

        private static string GenerateStringFromSymbol(char symbol)
        {
            if(AllAllowedCharacters.Contains(symbol.ToString()) == false)
                throw new GenerationException("Invalid symbol '" + symbol + "' encountered.");

            bool makeUpper = _random.Next(2) > 0;

            switch (symbol)
            {
                case '*':
                    if (makeUpper)
                    {
                        return GenerateRandomString(AllAllowedCharacters, 1).ToUpper();
                    }
                    return GenerateRandomString(AllAllowedCharacters, 1).ToLower();
                case 'L':
                    return GenerateRandomString(AllCharacters, 1).ToUpper();
                case 'l':
                    return GenerateRandomString(AllCharacters, 1).ToLower();
                case 'V':
                    return GenerateRandomString(VowelCharacters, 1).ToUpper();
                case 'v':
                    return GenerateRandomString(VowelCharacters, 1).ToLower();
                case 'C':
                    return GenerateRandomString(ConsonantCharacters, 1).ToUpper();
                case 'c':
                    return GenerateRandomString(ConsonantCharacters, 1).ToLower();
                case 'X':
                    return GenerateRandomString(Numbers0To9Characters, 1);
                case 'x':
                    return GenerateRandomString(Numbers1To9Characters, 1);
                default:
                    // Just append the character as it is not a symbol.
                    return symbol.ToString();
            }

        }

        private static string GenerateRandomString(string allowedCharacters, int length)
        {
            int numberofChars = allowedCharacters.Length;
            var sb = new StringBuilder(length);

            for(int i = 0; i < length;i++)
            {
                sb.Append(allowedCharacters[_random.Next(numberofChars)]);
            }
            return sb.ToString();
        }
    }
}
