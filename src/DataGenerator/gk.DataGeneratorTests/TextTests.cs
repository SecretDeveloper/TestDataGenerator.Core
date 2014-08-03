using System;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gk.DataGenerator.Generators;

namespace gk.DataGeneratorTests
{
    [TestClass]
    public class TextTests
    {
        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template()
        {
            var template = "Generated ((LL))";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.Write(text);            
            StringAssert.Matches(text, new Regex(@"Generated [A-Z]{2}"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_Multiple()
        {
            var template = "Generated ((LL)) and (([X]{5})) with ((X))";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"Generated [A-Z]{2} and [0-9]{5} with [0-9]"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_Harder()
        {
            var template = "This is a very basic (([l]{10})) which can be used to create ((llll)) of varying ((lllll)). The main purpose is to generate dummy ((Llll)) which can be used for ((lllllll)).";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"This is a very basic [a-z]{10} which can be used to create [a-z]{4} of varying [a-z]{5}. The main purpose is to generate dummy [A-Z][a-z]{3} which can be used for [a-z]{7}."));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_With_Alternatives()
        {
            var template = "((C|c{10}|VVV|[vv]{2,3}))";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"[BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{3}|[aeiou]{2,3}"));
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Missing_Placeholder_End()
        {
            var template = "This is a very basic (([l]{10} which can be used to create ((llll)) of varying ((lllll)). The main purpose is to generate dummy ((Llll)) which can be used for ((lllllll)).";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Missing_Placeholder_End_End()
        {
            var template = "This is a very basic (([l]{10})) which can be used to create ((llll)) of varying ((lllll)). The main purpose is to generate dummy ((Llll)) which can be used for ((lllllll.";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
        }


        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_AlphaNumeric()
        {

            var pattern = "*LlVvCcXx";
            string text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(9, text.Length);
            StringAssert.Matches(text, new Regex(@".{1}[A-Z]{1}[a-z]{1}[AEIOU]{1}[aeiou]{1}[QWRTYPSDFGHJKLZXCVBNM]{1}[qwrtypsdfghjklzxcvbnm]{1}[0-9]{1}[1-9]{1}"));

            pattern = "LLLLLL-LL-LLLLL";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));

            pattern = "llllll";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[a-z]*"));

            pattern = "XXXXXXLLLLllll";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(14, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"));

            pattern = "XXX-XX-XXXX";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(11, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{3}-[0-9]{2}-[0-9]{4}"));

            //Test for escaped characters.
            pattern = @"L\LLLLL";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{1}L[A-Z]{4}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Simple_Patterns()
        {
            var pattern = "LLLLLL-LL-LLLLL";
            string text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));

            pattern = "llllll";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[a-z]*"));

            pattern = "XXXXXXLLLLllll";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(14, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"));

            pattern = "XXX-XX-XXXX";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(11, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{3}-[0-9]{2}-[0-9]{4}"));

            //Test for escaped characters.
            pattern = @"L\LLLLL";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{1}L[A-Z]{4}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Pattern()
        {
            string pattern;
            string text = AlphaNumericGenerator.GenerateFromPattern();
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@".{15}")); // Default is 15 chars

            pattern = "[LLXX]{3}";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Symbol()
        {
            var pattern = "L{3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{3}"));
        }
        
        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Mixed_Pattern_With_Random_Length()
        {
            string pattern;
            pattern = "[L]{10,20}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{10,20}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Output_Escaped_Symbols()
        {
            string pattern;
            pattern = @"\X";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"X"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Output_Escaped_Repeated_Symbols()
        {
            string pattern;
            pattern = @"\X{10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"[X]{10}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Output_Escaped_Slash()
        {
            string pattern;
            pattern = @"[\\]{1,10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"[\\]{1,10}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Mixed_Pattern()
        {
            string pattern;
            pattern = "LL[LLXX]{3}LL[L]{23}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[A-Z]{23}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Pattern_Long()
        {
            var pattern = "[L]{1}[X]{1}[L]{2}[X]{2}[L]{4}[X]{4}[L]{8}[X]{8}[L]{16}[X]{16}[L]{32}[X]{32}[L]{64}[X]{64}[L]{128}[X]{128}[L]{256}[X]{256}[L]{512}[X]{512}[L]{1024}[X]{1024}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{1}[0-9]{1}[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{4}[A-Z]{8}[0-9]{8}[A-Z]{16}[0-9]{16}[A-Z]{32}[0-9]{32}[A-Z]{64}[0-9]{64}[A-Z]{128}[0-9]{128}[A-Z]{256}[0-9]{256}[A-Z]{512}[0-9]{512}[A-Z]{1024}[0-9]{1024}"));
            Assert.AreEqual((1+2+4+8+16+32+64+128+256+512+1024)*2, text.Length);
        }
        
        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Expressions_With_Alternates()
        {
            var pattern = "[[LL]|[XX]]";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"^[A-Z]{2}|[0-9]{2}$"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Zero_Repeats_Invalid_End()
        {
            var pattern = "[LLXX]33}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"^[A-Z]{2}[0-9]{2}33}$"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Pattern_With_Alternatives()
        {
            var template = "[[C]|[c]{10}|[VVV]|[v]{2,3}]";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"([BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{3}|[aeiou]{2,3})"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Pattern_With_Alternatives_Repeated_Symbols()
        {
            var template = "[[C]{1}|[c]{10}|[V]{3}|[v]{2,3}]";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"([BCDFGHJKLMNPQRSTVWXYZ]{1})|([bcdfghjklmnpqrstvwxyz]{10})|([AEIOU]{3}|[aeiou]{2,3})"));
            if(text.Contains("|")) Assert.Fail(text);
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Zero_Repeats()
        {
            var pattern = "[LLXX]";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.Write(text);
            StringAssert.Matches(text, new Regex(@"^[A-Z]{2}[0-9]{2}$"));
        }

        [TestMethod]
        [TestCategory("Profiling")]
        public void Profile_Random_Repeat()
        {
            var pattern = "((L{50,51}))";
            var testLimit = 1000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (var i = 0; i < testLimit; i++)
            {
                AlphaNumericGenerator.GenerateFromTemplate(pattern);
            }
            sw.Stop();

            Console.WriteLine(string.Format("{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern));
        }
        
        [TestMethod]
        [TestCategory("Profiling")]
        public void Profile_NonRandom_Repeat()
        {
            var pattern = "((L{50}))";
            var testLimit = 1000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (var i = 0; i < testLimit; i++)
            {
                AlphaNumericGenerator.GenerateFromTemplate(pattern);
            }
            sw.Stop();

            Console.WriteLine(string.Format("{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern));
        }

        [TestMethod]
        [TestCategory("Profiling")]
        public void Profile_Large_NonRandom_Repeat()
        {
            var pattern = "(([L]{1}[X]{1}[L]{2}[X]{2}[L]{4}[X]{4}[L]{8}[X]{8}[L]{16}[X]{16}[L]{32}[X]{32}[L]{64}[X]{64}[L]{128}[X]{128}[L]{256}[X]{256}[L]{512}[X]{512}[L]{1024}[X]{1024}))";
            var testLimit = 1000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (var i = 0; i < testLimit; i++)
            {
                AlphaNumericGenerator.GenerateFromTemplate(pattern);
            }
            sw.Stop();
            Console.WriteLine(string.Format("{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern));
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof (GenerationException))]
        public void Can_Throw_Exception_IncompletePattern()
        {
            var pattern = "[LLXX";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof (GenerationException))]
        public void Can_Throw_Exception_InvalidCardinality_Start()
        {
            var pattern = "[LLXX]{33";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof (GenerationException))]
        public void Can_Throw_Exception_InvalidCardinality_Value()
        {
            var pattern = "[LLXX]{}";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof (GenerationException))]
        public void Can_Throw_Exception_InvalidPattern_InvalidCardinality()
        {
            var pattern = "LLXX]{}";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof (GenerationException))]
        public void Can_Throw_Exception_Invalid_Cardinaliy_Less_Than_Zero()
        {
            var pattern = "[LLXX]{-1}";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof (GenerationException))]
        public void Can_Throw_Exception_InvalidPattern_Null()
        {
            AlphaNumericGenerator.GenerateFromPattern(null);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Mixed_Pattern_With_Invalid_Random_Length_Character()
        {
            string pattern;
            pattern = "[L]{10,}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);

        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Mixed_Pattern_With_Invalid_Random_Length_Min_Max()
        {
            string pattern;
            pattern = "[L]{10,0}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
        }


        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Invalid_Repeat_Pattern()
        {
            string pattern;
            string text = AlphaNumericGenerator.GenerateFromPattern();
            StringAssert.Matches(text, new Regex(@".{15}")); // Default is 15 chars

            pattern = "[LLXX]{w}";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Less_Than_Zero_Repeat_Pattern()
        {
            string pattern;
            string text = AlphaNumericGenerator.GenerateFromPattern();
            StringAssert.Matches(text, new Regex(@".{15}")); // Default is 15 chars

            pattern = "[LLXX]{-1}";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
        }
    }
}

