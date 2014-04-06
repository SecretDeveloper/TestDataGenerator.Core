using System;
using System.Data;
using System.Text;
using gk.DataGenerator.Interfaces;
using gk.DataGenerator.Options;

namespace gk.DataGenerator.Generators
{
    public class AlphaNumericGenerator : IDataGenerator 
    {
        
        private Random _random;
        

        private const string AllAllowedCharacters = @"abcdefghijklmnopqrstuvwxyz0123456789!£$%^&*()-=_+[]{};'#:@~,./<>?\|";
        private const string AllCharacters = "abcdefghijklmnopqrstuvwxyz";
        private const string VowelCharacters = "aeiou";
        private const string ConsonantCharacters = "bcdfghjklmnpqrstvwxyz";
        private const string Numbers0To9Characters = "0123456789";
        private const string Numbers1To9Characters = "123456789";

        public AlphaNumericGenerator()
        {
            _random = new Random(DateTime.Now.Millisecond);
        }

        #region IDataGenerator Members

        public string GenerateValue(IGenerationOption option)
        {
            if (option == null)
                throw new NoNullAllowedException("GenerateOptions cannot be null.");
            var opt = option as AlphaNumericGenerationOption;
            if (opt == null)
                throw new InvalidConstraintException(
                    "Cannot cast GenerateOptions to AlphaNumericGenerationOption. Unable to continue.");
            
            if (opt.NumberOfWords < 1)
                opt.NumberOfWords = 1;

            if (opt.TextFormat.Length == 0)
                opt.TextFormat = "[*]{15}";
            
            if (opt.NumberOfWords > 1)
                return GenerateWords(opt);

            return GenerateString(opt);
        }

        private string GenerateWords(AlphaNumericGenerationOption opt)
        {
            var sb = new StringBuilder();
            for (int c = 0; c < opt.NumberOfWords; c++)
            {
                if (c > 0)
                    sb.Append(" ");

                sb.Append(GenerateString(opt));
            }
            return sb.ToString();
        }

        #endregion


        private string GenerateString(AlphaNumericGenerationOption opt)
        {
            var sb = new StringBuilder();
            bool skipNext = false;

            var characters = opt.TextFormat.ToCharArray();
            for (int i = 0; i < characters.Length; i++)
            {
                char ch = characters[i];
                // check for escape chars for next part
                if (ch == '\\')
                {
                    skipNext = true;
                    continue;
                }
                // if we are escaping this char then just stick it in as is.
                if (skipNext)
                {
                    skipNext = false;
                    sb.Append(ch);
                    continue;
                }
                // check are we entering a repeat symbol section
                // Format = "LL[xx{4}]" = repeat xx 4 times.
                if (ch == '[')
                {
                    sb.Append(GetRepeatedPart(opt.TextFormat, ref i));
                    continue;
                }
                sb.Append(GenerateStringFromSymbol(ch));
            }
            return sb.ToString();
        }



        private string GetRepeatedPart(string characters, ref int i)
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
        private Tuple<int,string> GetRepeatingPatternTuple(string characters, ref int i)
        {
            string pattern = GetRepeatedPartSection(characters, ref i, '[', ']');
            string rs = GetRepeatedPartSection(characters, ref i, '{', '}');

            int repeat = 1;
            int.TryParse(rs, out repeat);
            if(repeat < 1)
                throw new InvalidExpressionException("Invalid repeat section, repeat value must be an int greater than 0.");

            return new Tuple<int, string>(repeat, pattern);
        }


        private string GetRepeatedPartSection(string characters, ref int i, char sectionStartChar, char sectionEndChar)
        {
            if (characters[i].Equals(sectionStartChar) == false)
                throw new InvalidExpressionException("Expected '"+sectionStartChar+"' at " + i + " but it was not found.");
            
            i++;
            int patternStart = i;

            int patternLength = (characters.IndexOf(sectionEndChar, i)) - patternStart;
            if(patternLength <= 0)
                throw new InvalidExpressionException("Expected '"+ sectionEndChar +"' but it was not found.");

            i = patternLength+2; // maintain index position.
            return characters.Substring(patternStart, patternLength);
        }

        private string GenerateStringFromSymbol(char symbol)
        {
            
            bool makeUpper = _random.Next(2) > 0;

            switch (symbol)
            {
                case '*':
                    if (makeUpper)
                    {
                        return GenerateRandomString(AllAllowedCharacters, 1).ToUpper();
                    }
                    return GenerateRandomString(AllAllowedCharacters, 1).ToUpper();
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

        private string GenerateRandomString(string allowedCharacters, int length)
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
