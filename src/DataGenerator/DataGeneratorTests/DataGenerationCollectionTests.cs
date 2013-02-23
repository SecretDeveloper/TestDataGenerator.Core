using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DataGenerator;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataGeneratorTests
{
    /// <summary>
    /// Summary description for DataGenerationCollectionTests
    /// </summary>
    [TestClass]
    public class DataGenerationCollectionTests
    {
        public DataGenerationCollectionTests()
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
        public void CanSaveOptionsToFile()
        {
            var collection = new DataGenerator.DataGenerationCollection();
            var alpha = new DataGenerator.AlphaNumericDataGenerator();
            var auto = new DataGenerator.AutoIncrementDataGenerator();
            var number = new DataGenerator.NumberRangeDataGenerator();
            var fixText = new DataGenerator.TextFixedDataGenerator();
            var random = new DataGenerator.TextRandomDataGenerator();


            ((AlphaNumericGenerationOptions) alpha.GenerationOptions).TextFormat = "LLllVVCCXX--ss";
            ((AutoIncrementGenerationOptions) auto.GenerationOptions).StartValue = 100;
            ((AutoIncrementGenerationOptions)auto.GenerationOptions).Increment = 2;
            ((NumberRangeGenerationOptions)number.GenerationOptions).Low = 2;
            ((NumberRangeGenerationOptions)number.GenerationOptions).Low = 20;
            ((TextFixedGenerationOptions)fixText.GenerationOptions).NumberofWordsToGenerate = 20;
            ((TextRandomGenerationOptions)random.GenerationOptions).MaximumNumberOfWordsToGenerate = 100;

            collection.Items.Add(alpha);
            collection.Items.Add(auto);
            collection.Items.Add(number);
            collection.Items.Add(fixText);
            collection.Items.Add(random);
            
            collection.Save(@"c:\datacollectiontest.txt");

            collection.Load(@"c:\datacollectiontest.txt");

            Assert.AreEqual(5 , collection.Items.Count);


        }
    }
}
