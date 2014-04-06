using Microsoft.VisualStudio.TestTools.UnitTesting;
using gk.DataGenerator;
using gk.DataGenerator.Generators;
using gk.DataGenerator.Options;

namespace gk.DataGeneratorTests
{
    /// <summary>
    /// Summary description for NumberTests
    /// </summary>
    [TestClass]
    public class NumberTests
    {
        [TestMethod]
        public void AutoIncrementTest()
        {
            var auto = new AutoIncrementGenerator();

            AutoIncrementGenerationOption autoOption = new AutoIncrementGenerationOption();
            autoOption.StartValue = 100;
            autoOption.Increment = 1;

            for(int i =100; i<120;i++)
            {
               Assert.AreEqual(i.ToString(),auto.GenerateValue(autoOption));
            }

            autoOption.StartValue = 0;
            autoOption.Increment = 100;

            for (int i = 0; i < 1000; i=i+100)
            {
                Assert.AreEqual(i.ToString(), auto.GenerateValue(autoOption));
            }
        }


        [TestMethod]
        public void NumberRangeTest()
        {
            var auto = new NumberRangeGenerator();
            var autoOption = new NumberRangeGenerationOption();
            autoOption.Low = 1;
            autoOption.High = 100;

            for (int i = 1; i < 100; i++)
            {
                int val = int.Parse(auto.GenerateValue(autoOption));
                Assert.IsTrue((1 < val));
                Assert.IsTrue((100 > val));
            }

        }
    }
}
