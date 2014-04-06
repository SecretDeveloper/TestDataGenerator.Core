using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gk.DataGenerator;
using gk.DataGenerator.Generators;
using gk.DataGenerator.Options;

namespace gk.DataGeneratorTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TextTests
    {
        [TestMethod]
        public void CanGenerateCorrectNumberOfWords()
        {
            var fixedText = new AlphaNumericGenerator();
            var option = new AlphaNumericGenerationOption();
            option.NumberOfWords = 5;

            string text = fixedText.GenerateValue(option);
            Assert.AreEqual(5,(text.Split(' ')).Count());

        }

        [TestMethod]
        public void CanGenerateRandomWords()
        {
            var option = new AlphaNumericGenerationOption();
            option.NumberOfWords = 5;
            
            var alphaNumericGenerator = new AlphaNumericGenerator();
            string text = alphaNumericGenerator.GenerateValue(option);
            Assert.AreEqual(5, text.Split(' ').Count());
            
            string text2 = alphaNumericGenerator.GenerateValue(option);
            Assert.AreEqual(5, text2.Split(' ').Count());
            Assert.AreNotEqual(text,text2);
        }

        [TestMethod]
        public void CanGenerateAlphaNumeric()
        {
            var randomText = new AlphaNumericGenerator();
            var opt = new AlphaNumericGenerationOption();
            
            opt.TextFormat = "*LlVvCcXx";
            string text = randomText.GenerateValue(opt);
            Assert.AreEqual(9, text.Length);
            //SGuIeRy76
            StringAssert.Matches(text, new Regex(@".{1}[A-Z]{1}[a-z]{1}[AEIOU]{1}[aeiou]{1}[QWRTYPSDFGHJKLZXCVBNM]{1}[qwrtypsdfghjklzxcvbnm]{1}[0-9]{1}[1-9]{1}"));

            opt.TextFormat = "LLLLLL-LL-LLLLL";
            text = randomText.GenerateValue(opt);
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text,new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));

            opt.TextFormat = "llllll";
            text = randomText.GenerateValue(opt);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[a-z]*"));

            opt.TextFormat = "XXXXXXLLLLllll";
            text = randomText.GenerateValue(opt);
            Assert.AreEqual(14, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"));

            opt.TextFormat = "XXX-XX-XXXX";
            text = randomText.GenerateValue(opt);
            Assert.AreEqual(11, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{3}-[0-9]{2}-[0-9]{4}"));

            //Test for escaped characters.
            opt.TextFormat = @"L\LLLLL";
            text = randomText.GenerateValue(opt);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{1}L[A-Z]{4}"));
        }

        [TestMethod]
        public void CanGenerateSimplePatterns()
        {
            var randomText = new AlphaNumericGenerator();
            var opt = new AlphaNumericGenerationOption();

            opt.TextFormat = "LLLLLL-LL-LLLLL";
            string text = randomText.GenerateValue(opt);
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));

            opt.TextFormat = "llllll";
            text = randomText.GenerateValue(opt);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[a-z]*"));

            opt.TextFormat = "XXXXXXLLLLllll";
            text = randomText.GenerateValue(opt);
            Assert.AreEqual(14, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"));

            opt.TextFormat = "XXX-XX-XXXX";
            text = randomText.GenerateValue(opt);
            Assert.AreEqual(11, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{3}-[0-9]{2}-[0-9]{4}"));

            //Test for escaped characters.
            opt.TextFormat = @"L\LLLLL";
            text = randomText.GenerateValue(opt);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{1}L[A-Z]{4}"));
            
        }

        [TestMethod]
        public void CanGenerateRepeatPattern()
        {
            var randomText = new AlphaNumericGenerator();
            var opt = new AlphaNumericGenerationOption();

            opt.TextFormat = "";
            string text = randomText.GenerateValue(opt);
            StringAssert.Matches(text, new Regex(@".{15}")); // Default is 15 chars

            opt.TextFormat = "[LLXX]{3}";
            text = randomText.GenerateValue(opt);
            StringAssert.Matches(text, new Regex(@"[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}"));
            
        }

        [TestMethod]
        public void CanGenerate_Exceptions()
        {
            var randomText = new AlphaNumericGenerator();
            var opt = new AlphaNumericGenerationOption();

            opt.TextFormat = "[LLXX";
            try
            {
                randomText.GenerateValue(opt);
                Assert.Fail("Exception not thrown for invalid pattern.");
            }
            catch{}

            opt.TextFormat = "[LLXX]{33";
            try
            {
                randomText.GenerateValue(opt);
                Assert.Fail("Exception not thrown for invalid pattern.");
            }
            catch { }

            opt.TextFormat = "[LLXX]33}";
            try
            {
                randomText.GenerateValue(opt);
                Assert.Fail("Exception not thrown for invalid pattern.");
            }
            catch { }


            opt.TextFormat = "[LLXX]{}";
            try
            {
                randomText.GenerateValue(opt);
                Assert.Fail("Exception not thrown for invalid pattern.");
            }
            catch { }

            opt.TextFormat = "LLXX]{}";
            try
            {
                randomText.GenerateValue(opt);
                Assert.Fail("Exception not thrown for invalid pattern.");
            }
            catch { }

            opt.TextFormat = "[LLXX]";
            try
            {
                randomText.GenerateValue(opt);
                Assert.Fail("Exception not thrown for invalid pattern.");
            }
            catch { }

            opt.TextFormat = "[LLXX]{0}";
            try
            {
                randomText.GenerateValue(opt);
                Assert.Fail("Exception not thrown for invalid pattern.");
            }
            catch { }

            try
            {
                randomText.GenerateValue(null);
                Assert.Fail("Exception not thrown for invalid pattern.");
            }
            catch { }

            try
            {
                randomText.GenerateValue(new NumberRangeGenerationOption()); // wrong type
                Assert.Fail("Exception not thrown for invalid pattern.");
            }
            catch { }

            opt.TextFormat = "[LLXX";
            opt.NumberOfWords = -1;
            try
            {
                randomText.GenerateValue(opt);
                Assert.Fail("Exception not thrown for invalid pattern.");
            }
            catch { }
        }
    }
}
