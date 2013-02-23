using System;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using DataGenerator.Enum;

namespace DataGenerator
{
    [XmlRootAttribute("AlphaNumericGenerationOptions", Namespace = "", IsNullable = false)]
    public class AlphaNumericGenerationOptions:IGenerationOptions,ISerializable
    {
        #region IGenerationOptions Members
        [XmlAttributeAttribute(DataType = "string")]
        public string FormatString
        {
            get; set;
        }

        #endregion
        
        /// <summary>
        /// * - An uppercase or lowercase letter or number.
        /// L - An uppercase Letter.  	
        /// l - A lowercase letter. 	
        /// V - An uppercase Vowel.
        /// v - A lowercase vowel.
        /// C - An uppercase Consonant. 	
        /// c - A lowercase consonant. 	
        /// X - Any number, 1-9.
        /// x - Any number, 0-9.
        /// --
        /// Or leave empty to generate a random 15 character string.
        /// </summary>
        [XmlAttributeAttribute(DataType = "string")]
        public string TextFormat { get; set; }

        public AlphaNumericGenerationOptions()
        {
            this.TextFormat = string.Empty;
        }

        #region ISerializable Members

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //info.A//
        }

        #endregion
    }

    public class AlphaNumericDataGenerator : IDataTypeGenerator 
    {
        #region IDataTypeGenerator Members

        private Random mRand;

        public DataTypes DataType
        {
            get
            {
                return DataTypes.TextFixed;
            }
        }

        public IGenerationOptions GenerationOptions
        {
            get; set;
        }

        public AlphaNumericDataGenerator()
        {
            this.GenerationOptions = new AlphaNumericGenerationOptions();
        }
        public AlphaNumericDataGenerator(IGenerationOptions options)
        {
            this.GenerationOptions = options;
        }

        private const string AllAllowedCharacters = @"abcdefghijklmnopqrstuvwxyz0123456789!£$%^&*()-=_+[]{};'#:@~,./<>?\|";
        private const string AllCharacters = "abcdefghijklmnopqrstuvwxyz";
        private const string VowelCharacters = "aeiou";
        private const string ConsonantCharacters = "bcdfghjklmnpqrstvwxyz";
        private const string Numbers0To9Characters = "0123456789";
        private const string Numbers1To9Characters = "123456789";

        public string GenerateValue()
        {
            if (GenerationOptions == null)
                throw new NoNullAllowedException("GenerateOptions cannot be null.");
            var opt = GenerationOptions as AlphaNumericGenerationOptions;
            if (opt == null)
                throw new InvalidConstraintException(
                    "Cannot cast GenerateOptions to AlphaNumericGenerationOptions. Unable to continue.");

            mRand = new Random(DateTime.Now.Millisecond);
            if (opt.TextFormat.Length == 0)
            {
                return GenerateRandomString(AllAllowedCharacters, mRand.Next(15));
            }

            StringBuilder sb = new StringBuilder(opt.TextFormat.Length);
            bool skipNext = false;

            var characters = opt.TextFormat.ToCharArray();
            for(int i =0; i < characters.Length; i++)
            {
                char ch = characters[i];
                // check for escape chars for next part
                if(ch == '\\')
                {
                    skipNext = true;
                    continue;
                }
                // if we are escaping this char then just stick it in as is.
                if(skipNext)
                {
                    skipNext = false;
                    sb.Append(ch);
                    continue;
                }
                // check are we entering a repeat symbol section
                // Format = "LL[xx{4}]" = repeat xx 4 times.
                if(ch == '[')
                {
                    sb.Append(GetRepeatedPart(characters, ref i));
                    continue;
                }

                sb.Append(GetNextPart(ch));
            }
            return sb.ToString();
        }

        private string GetRepeatedPart(char[] characters, ref int i)
        {

            i++;
            // ok so we are in a repeat section to lets figure out what we need to repeat and how many times
            string repeat = "";
            bool keepGoing = true;
            while (i < characters.Length && keepGoing)
            {
                char nc = characters[i];
                if (nc != '{')
                {
                    repeat = repeat + nc;
                    i++;
                }
                else
                {
                    keepGoing = false;
                    i++;
                }
            }
            // ok so we should have our repeatable string
            // now how many times do we repeat it?
            keepGoing = true;
            string sNumber = "";
            while (i < characters.Length && keepGoing)
            {
                char nc = characters[i];
                if (nc != '}')
                {
                    sNumber = sNumber + nc;
                    i++;
                }
                else
                {
                    keepGoing = false;
                    i++;//}
                    //i++; //]
                }
            }
            int nTimes = 0;
            if (int.TryParse(sNumber, out nTimes) == false)
            {
                throw new ArgumentException("Invalid repeat pattern near '" + nTimes + "'");
            }
            var sb = new StringBuilder();
            //ok so we have our pattern, lets repeat it
            for (int x = 0; x < nTimes; x++)
            {
                foreach (var chx in repeat)
                {
                    sb.Append(GetNextPart(chx));
                }
            }
            return sb.ToString();
        }

        private string GetNextPart(char symbol)
        {
            bool makeUpper = mRand.Next(2) > 0;

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
                sb.Append(allowedCharacters[mRand.Next(numberofChars)]);
            }
            return sb.ToString();
        }

        #endregion
    }
}
