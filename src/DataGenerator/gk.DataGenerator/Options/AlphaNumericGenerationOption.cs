using System.Runtime.Serialization;
using System.Xml.Serialization;
using gk.DataGenerator.Interfaces;

namespace gk.DataGenerator.Options
{
    public class AlphaNumericGenerationOption:IGenerationOption
    {
        /// <summary>
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
        /// "[*{10}]" will produce a random string 10 characters long.
        /// </summary>
        public string TextFormat { get; set; }

        /// <summary>
        /// The number of words to produce.
        /// 1 by default.
        /// </summary>
        public int NumberOfWords { get; set; }

        public AlphaNumericGenerationOption()
        {
            this.TextFormat = "";
            this.NumberOfWords = 1;
        }

    }
}