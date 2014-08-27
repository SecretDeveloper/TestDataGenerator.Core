using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using gk.DataGenerator.Generators;

namespace gk.DataGeneratorTests
{
    [TestClass]
    public class TextTests
    {


        [TestMethod]
        [TestCategory("Documentation")]
        [DeploymentItem(@"..\templates\README.template")]
        public void Documentation_Builder()
        {
            var template = System.IO.File.ReadAllText("README.template");

            var text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(text);

        }

        #region Template

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Escape_Template()
        {
            var template = @"Generated \<<LL>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"Generated \\<<LL>>"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_No_Symbols()
        {
            var template = "Generated <<LL>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");            
            StringAssert.Matches(text, new Regex(@"Generated [L]{2}"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_for_ReadMe1()
        {
            var template = @"Hi there <<\L\v{0,2}\l{0,2}\v \L\v{0,2}\l{0,2}\v{0,2}\l{0,2}\l>> how are you doing?";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"Hi there [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou] [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou]{0,2}[a-z]{0,2}[a-z] how are you doing\?"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_for_ReadMe2()
        {
            var template = @"<<aa\D\d\d-\d\d-\d\d\d\d>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"aa[0-9][1-9][1-9]-[1-9][1-9]-[1-9][1-9][1-9][1-9]"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_for_ReadMe3()
        { 
            
            var template = @"Hi there <<\L\v{0,2}\l{0,2}\v \L\v{0,2}\l{0,2}\v{0,2}\l{0,2}\l>> how are you doing? Your SSN is <<\d\D\D-\D\D-\D\D\D\D>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            
            StringAssert.Matches(text, new Regex(@"Hi there [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou] [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou]{0,2}[a-z]{0,2}[a-z] how are you doing\? Your SSN is [1-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_with_Symbols()
        {
            var template = @"Generated <<L\L>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"Generated [L][A-Z]"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_Multiple()
        {
            var template = @"Generated <<\L\L>> and <<(\d){5}>> with <<\d>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"Generated [A-Z]{2} and [0-9]{5} with [0-9]"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_Harder()
        {
            var template = @"This is a very basic <<(\l){10}>> which can be used to create <<\l\l\l\l>> of varying <<\l\l\l\l\l>>. The main purpose is to generate dummy <<\L\l\l\l>> which can be used for <<\l\l\l\l\l\l\l>>.";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"This is a very basic [a-z]{10} which can be used to create [a-z]{4} of varying [a-z]{5}. The main purpose is to generate dummy [A-Z][a-z]{3} which can be used for [a-z]{7}."));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_With_Alternatives_Symbols()
        {
            var template = @"<<\C|\c{10}|\V\V\V|\v{2,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"[BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{3}|[aeiou]{2,3}"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_With_Alternative_Groups()
        {
            var template = @"<<(\C)|\c{10}|(\V\V\V){20}|(\v\v\v\v\v){2,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"[BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{60}|[aeiou]{10,15}"));
        }
        
        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Pattern_With_Alternatives()
        {
            var template = @"Alternatives <<\C|(\c){10}|\V\V\V|\v{2,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"Alternatives ([BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{3}|[aeiou]{2,3})"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Pattern_With_Alternatives_Repeated_Symbols()
        {
            var template = @"Alternatives <<(\C{1}|\c{10}|\V{3}|\v{2,3})>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"Alternatives ([BCDFGHJKLMNPQRSTVWXYZ]{1})|([bcdfghjklmnpqrstvwxyz]{10})|([AEIOU]{3}|[aeiou]{2,3})"));
            if (text.Contains("|")) Assert.Fail(text);
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Pattern_With_calculated_quantity()
        {
            var template = @"<<\v{2,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"([aeiou]{2,3})"));
            if (text.Contains("|")) Assert.Fail(text);
        }

#endregion

        #region Sets
        
        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Escape_Sets()
        {
            var template = @"<<\[LL]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^\[LL]$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_NonRange_Set()
        {
            var template = @"<<[AEI]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[AEI]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Set()
        {
            var template = @"<<[A-I]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[ABCDEFGHI]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Set2()
        {
            var template = @"<<[A-B]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[AB]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Set3()
        {
            var template = @"<<[W-X]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[W-X]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Repeated_Set1()
        {
            var template = @"<<[W-X]{10}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[W-X]{10}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric()
        {
            var template = @"<<[1-8]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[1-8]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric2()
        {
            var template = @"<<[100-150]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            int e = int.Parse(text);
            if(e < 100 || e>150) Assert.Fail("Number not between 100 and 150.");
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric3()
        {
            var template = @"<<(.[100-101]){3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^(.(100|101)){1}(.(100|101)){1}(.(100|101)){1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric4()
        {
            var template = @"<<(.[100-101]){1,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^(.(100|101))?(.(100|101))?(.(100|101))?$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat()
        {
            var template = @"<<([1.00-10.00])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            double d = -1d;
            if (!double.TryParse(text, out d))
                Assert.Fail();
            if(d<1.00 || d>10.00d) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat2()
        {
            var template = @"<<([1.00-2.00])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            double d = -1d;
            if (!double.TryParse(text, out d))
                Assert.Fail();
            if (d < 1.00 || d > 2.00d) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat3()
        {
            var template = @"<<([1.1-1.2])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            double d = -1d;
            if (!double.TryParse(text, out d))
                Assert.Fail();
            if (d < 1.1 || d > 1.2d) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat4()
        {
            var template = @"<<([12345.12345-12345.12346])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            double d = -1d;
            if (!double.TryParse(text, out d))
                Assert.Fail();
            if (d < 12345.12345 || d > 12345.12346d) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Complex()
        {
            var template = @"<<\C\C\C-[0-9]{6}>>,<<(818|33)>>,<<\D\d\d-\d\d-\d\d\d\d>>,,GHI,Enroll,<<[1-12]/[1-28]/[1960-2000]>>,<<(M|F)>>,<<\d \L\v\l\l (st|rd|ave|blv)>>,,<<Dallas>>,<<(TX|GA|TN|FL)>>,<<[3000-5000]>>,<<[1-12]/[1-28]/[2005-2014]>>,<<2014-[1-12]-[1-28]T:[0-24]:[0-60]:[0-60]>>,<<[20-30]>>,<<[10-20]>>,<<[1-12]/[1-28]/[2014]>>,N,,N,N,,,,,,,,,,,,,,,,,,,,,,,,,,,";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
        }
        
        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_MultipleRange_Set()
        {
            var template = @"<<[A-B][1-3]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[A-B]{1}[1-3]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_MultipleRange_Set2()
        {
            var template = @"<<[1-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[1-9]{1}[0-9]{1}[0-9]{1}-[0-9]{1}[0-9]{1}-[0-9]{1}[0-9]{1}[0-9]{1}[0-9]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_MultipleRange_Set3()
        {
            var template = @"<<[1-28]/[1-12]/[1960-2013]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine("'" + template + "' produced '" + text + "'");
            DateTime dt = DateTime.Now;
            if(DateTime.TryParseExact(text, @"d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt) == false) Assert.Fail("invalid Date");
        
        }

        #endregion

        #region Pattern

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_AlphaNumeric_multiple()
        {

            var pattern = @"\L\L\L\L\L\L-\L\L-\L\L\L\L\L";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));

            pattern = @"\l\l\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[a-z]*"));

            pattern = @"\d\d\d\d\d\d\L\L\L\L\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            Assert.AreEqual(14, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"));

            pattern = @"\d\d\d-\d\d-\d\d\d\d";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            Assert.AreEqual(11, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{3}-[0-9]{2}-[0-9]{4}"));

            //Test for escaped characters.
            pattern = @"L\LLLLL";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("L[A-Z]{1}LLLL"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_AlphaNumeric()
        {

            var pattern = @"\L\l\V\v\C\c\D\d";
            string text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"[A-Z]{1}[a-z]{1}[AEIOU]{1}[aeiou]{1}[QWRTYPSDFGHJKLZXCVBNM]{1}[qwrtypsdfghjklzxcvbnm]{1}[0-9]{1}[1-9]{1}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Simple_Patterns()
        {
            var pattern = @"\L\L\L\L\L\L-\L\L-\L\L\L\L\L";
            string text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));

            pattern = @"\l\l\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[a-z]*"));

            pattern = @"\d\d\d\d\d\d\L\L\L\L\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            Assert.AreEqual(14, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"));

            pattern = @"\D\d\d-\d\d-\d\d\d\d";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            Assert.AreEqual(11, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{3}-[0-9]{2}-[0-9]{4}"));

            //Test for escaped characters.
            pattern = @"L\LLLLL";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("L[A-Z]{1}LLLL"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Pattern()
        {
            var pattern = @"(\L\L\d\d){3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Symbol()
        {
            var pattern = @"\L{3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"[A-Z]{3}"));
        }
        
        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Mixed_Pattern_With_Random_Length()
        {
            string pattern;
            pattern = @"\L{10,20}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"[A-Z]{10,20}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Output_NonEscaped_Symbols()
        {
            string pattern;
            pattern = @"X";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"X"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Output_Repeated_Symbols()
        {
            string pattern;
            pattern = @"\d{10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"[\d]{10}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Output_Escaped_Slash()
        {
            string pattern;
            pattern = @"[\\]{1,10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"[\\]{1,10}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Mixed_Pattern()
        {
            string pattern;
            pattern = @"\L\L(\L\L\d\d){3}\L\L(\L){23}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"[A-Z]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[A-Z]{23}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Pattern_Long()
        {
            var pattern = @"(\L){1}(\d){1}(\L){2}(\d){2}(\L){4}(\d){4}(\L){8}(\d){8}(\L){16}(\d){16}(\L){32}(\d){32}(\L){64}(\d){64}(\L){128}(\d){128}(\L){256}(\d){256}(\L){512}(\d){512}(\L){1024}(\d){1024}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"[A-Z]{1}[0-9]{1}[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{4}[A-Z]{8}[0-9]{8}[A-Z]{16}[0-9]{16}[A-Z]{32}[0-9]{32}[A-Z]{64}[0-9]{64}[A-Z]{128}[0-9]{128}[A-Z]{256}[0-9]{256}[A-Z]{512}[0-9]{512}[A-Z]{1024}[0-9]{1024}"));
            Assert.AreEqual((1+2+4+8+16+32+64+128+256+512+1024)*2, text.Length);
        }
        
        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Expressions_With_Alternates()
        {
            var pattern = @"(\L\L|\d\d)";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^([A-Z]{2}|[0-9]{2})$"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Expressions_With_Alternates2()
        {
            var pattern = @"(\L\L|\d\d|[AEIOU]|[100-120])";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^([A-Z]{2}|[0-9]{2}|[AEIOU]|\d\d\d)$"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Zero_Repeats_Invalid_End()
        {
            var pattern = @"(\L\L\d\d)33}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[A-Z]{2}[0-9]{2}33}$"));
        }
        
        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Zero_Repeats()
        {
            var pattern = @"\L\L\d\d";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[A-Z]{2}[0-9]{2}$"));
        }

        #endregion

        #region Symbols

        [TestMethod]
        [TestCategory("Symbols")]
        public void Can_Generate_All_Symbols()
        {
            var pattern = @"\.";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!£$%^&*_+;'#,./:@~?]$"));

            pattern = @"\w";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[A-Za-z]$"));

            pattern = @"\L";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[A-Z]$"));

            pattern = @"\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[a-z]$"));

            pattern = @"\V";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[AEIOU]$"));

            pattern = @"\v";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[aeiou]$"));

            pattern = @"\C";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[BCDFGHJKLMNPQRSTVWXYZ]$"));

            pattern = @"\c";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[bcdfghjklmnpqrstvwxyz]$"));

            pattern = @"\D";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[0-9]$"));

            pattern = @"\d";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            StringAssert.Matches(text, new Regex(@"^[1-9]$"));
        }

        #endregion

        #region Profiling

        [TestMethod]
        [TestCategory("Profiling")]
        public void Profile_Random_Repeat()
        {
            var pattern = @"<<\L{50,51}>>";
            var testLimit = 1000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (var i = 0; i < testLimit; i++)
            {
                var text = AlphaNumericGenerator.GenerateFromTemplate(pattern);
                Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            }
            sw.Stop();

            Console.WriteLine(string.Format("{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern));
        }
        
        [TestMethod]
        [TestCategory("Profiling")]
        public void Profile_NonRandom_Repeat()
        {
            var pattern = @"<<\L{50}>>";
            var testLimit = 1000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (var i = 0; i < testLimit; i++)
            {
                var text = AlphaNumericGenerator.GenerateFromTemplate(pattern);
                Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            }
            sw.Stop();

            Console.WriteLine(string.Format("{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern));
        }

        [TestMethod]
        [TestCategory("Profiling")]
        public void Profile_Large_NonRandom_Repeat()
        {
            var pattern = @"<<\L{1}\D{1}\L{2}\D{2}\L{4}\D{4}\L{8}\D{8}\L{16}\D{16}\L{32}\D{32}\L{64}\D{64}\L{128}\D{128}\L{256}\D{256}\L{512}\D{512}\L{1024}\D{1024}>>";
            var testLimit = 1000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (var i = 0; i < testLimit; i++)
            {
                var text = AlphaNumericGenerator.GenerateFromTemplate(pattern);
                Console.WriteLine("'" + pattern + "' produced '" + text + "'");
            }
            sw.Stop();
            Console.WriteLine(string.Format("{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern));
        }
        
        #endregion

        #region NegativeTesting

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Missing_Placeholder_End()
        {
            var template = "This is a very basic <<(l){10}>> which can be used to create <<llll of varying <<lllll>>. The main purpose is to generate dummy <<Llll>> which can be used for <<lllllll>>.";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Missing_Placeholder_End_End()
        {
            var template = "This is a very basic <<(l){10}>> which can be used to create <<llll>> of varying <<lllll>>. The main purpose is to generate dummy <<Llll>> which can be used for <<lllllll.";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof (GenerationException))]
        public void Can_Throw_Exception_IncompletePattern()
        {
            var pattern = "(LLXX";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof (GenerationException))]
        public void Can_Throw_Exception_InvalidCardinality_Start()
        {
            var pattern = "(LLXX){33";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }
        
        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof (GenerationException))]
        public void Can_Throw_Exception_InvalidPattern_InvalidCardinality()
        {
            var pattern = "(LLXX){}";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof (GenerationException))]
        public void Can_Throw_Exception_Invalid_Cardinaliy_Less_Than_Zero()
        {
            var pattern = "(LLXX){-1}";
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
            pattern = "(L){10,}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);

        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Mixed_Pattern_With_Invalid_Random_Length_Min_Max()
        {
            string pattern;
            pattern = "(L){10,0}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
        }
        
        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Invalid_Repeat_Pattern()
        {
            var pattern = "(LLXX){w}";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        #endregion
    }
}

