using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataGeneratorTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TextTests
    {
        public TextTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void CanGenerateCorrectNumberOfWords()
        {
            var fixedText = new DataGenerator.TextFixedDataGenerator();
            ((DataGenerator.TextFixedGenerationOptions)fixedText.GenerationOptions).NumberofWordsToGenerate = 5;


            string text = fixedText.GenerateValue();
            Assert.AreEqual(5,(text.Split(' ')).Count());

        }

        [TestMethod]
        public void CanGenerateRandomWords()
        {
            var randomText = new DataGenerator.TextRandomDataGenerator();

            string text = randomText.GenerateValue();
            string text2 = randomText.GenerateValue();
            Assert.AreNotEqual(text,text2);
        }

        [TestMethod]
        public void CanGenerateAlphaNumeric()
        {
            var randomText = new DataGenerator.AlphaNumericDataGenerator();
            var opt = (DataGenerator.AlphaNumericGenerationOptions) randomText.GenerationOptions;

            opt.TextFormat = "*LlVvCcXx";
            string text = randomText.GenerateValue();
            Assert.AreEqual(9, text.Length);
            //SGuIeRy76
            StringAssert.Matches(text, new Regex(@".{1}[A-Z]{1}[a-z]{1}[AEIOU]{1}[aeiou]{1}[QWRTYPSDFGHJKLZXCVBNM]{1}[qwrtypsdfghjklzxcvbnm]{1}[0-9]{1}[1-9]{1}"));

            opt.TextFormat = "LLLLLL-LL-LLLLL";
            text = randomText.GenerateValue();
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text,new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));

            opt.TextFormat = "llllll";
            text = randomText.GenerateValue();
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[a-z]*"));

            opt.TextFormat = "XXXXXXLLLLllll";
            text = randomText.GenerateValue();
            Assert.AreEqual(14, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"));

            opt.TextFormat = "XXX-XX-XXXX";
            text = randomText.GenerateValue();
            Assert.AreEqual(11, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{3}-[0-9]{2}-[0-9]{4}"));

            //Test for escaped characters.
            opt.TextFormat = @"L\LLLLL";
            text = randomText.GenerateValue();
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{1}L[A-Z]{4}"));
        }

        [TestMethod]
        public void CanGenerateRepeatPattern()
        {
            var randomText = new DataGenerator.AlphaNumericDataGenerator();
            var opt = (DataGenerator.AlphaNumericGenerationOptions)randomText.GenerationOptions;

            opt.TextFormat = "[LLXX{3}]";
            string text = randomText.GenerateValue();
            StringAssert.Matches(text, new Regex(@"[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}"));
            
            
            /*
            opt.TextFormat = "LLLLLL-LL-LLLLL";
            text = randomText.GenerateValue();
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));

            opt.TextFormat = "llllll";
            text = randomText.GenerateValue();
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[a-z]*"));

            opt.TextFormat = "XXXXXXLLLLllll";
            text = randomText.GenerateValue();
            Assert.AreEqual(14, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"));

            opt.TextFormat = "XXX-XX-XXXX";
            text = randomText.GenerateValue();
            Assert.AreEqual(11, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{3}-[0-9]{2}-[0-9]{4}"));

            //Test for escaped characters.
            opt.TextFormat = @"L\LLLLL";
            text = randomText.GenerateValue();
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{1}L[A-Z]{4}"));
             * */
        }
    }
}
