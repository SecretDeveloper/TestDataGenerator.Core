using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataGeneratorTests
{
    /// <summary>
    /// Summary description for NumberTests
    /// </summary>
    [TestClass]
    public class NumberTests
    {
        public NumberTests()
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
        public void AutoIncrementTest()
        {
            var auto = new DataGenerator.AutoIncrementDataGenerator();
            ((DataGenerator.AutoIncrementGenerationOptions) auto.GenerationOptions).StartValue = 100;
            ((DataGenerator.AutoIncrementGenerationOptions)auto.GenerationOptions).Increment = 1;

            for(int i =100; i<120;i++)
            {
               Assert.AreEqual(i.ToString(),auto.GenerateValue());
            }

            ((DataGenerator.AutoIncrementGenerationOptions)auto.GenerationOptions).StartValue = 0;
            ((DataGenerator.AutoIncrementGenerationOptions)auto.GenerationOptions).Increment = 100;

            for (int i = 0; i < 1000; i=i+100)
            {
                Assert.AreEqual(i.ToString(), auto.GenerateValue());
            }
        }


        [TestMethod]
        public void NumberRangeTest()
        {
            var auto = new DataGenerator.NumberRangeDataGenerator();
            ((DataGenerator.NumberRangeGenerationOptions)auto.GenerationOptions).Low = 1;
            ((DataGenerator.NumberRangeGenerationOptions)auto.GenerationOptions).High = 100;

            for (int i = 1; i < 100; i++)
            {
                int val = int.Parse(auto.GenerateValue());
                Assert.IsTrue((1 < val));
                Assert.IsTrue((100 > val));
            }

        }
    }
}
