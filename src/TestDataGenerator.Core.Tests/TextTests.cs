using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataGenerator.Core;
using TestDataGenerator.Core.Exceptions;
using TestDataGenerator.Core.Generators;

namespace TestDataGenerator.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class TextTests
    {
        
        #region Template

        [TestMethod]
        [TestCategory("Template")]
        public void Can_GenerateFromTemplate_Overload1()
        {
            var template = @"Generated <<\L\L>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"Generated [A-Z]{2}"));
        }
        
        [TestMethod]
        [TestCategory("Template")]
        public void Can_GenerateFromTemplate_Overload2()
        {
            var template = @"Generated <<\L\L>>";
            var config = new GenerationConfig() {Seed = "100"};
            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"Generated [A-Z]{2}"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_GenerateFromTemplate_Overload3()
        {
            var template = @"<<@superhero@>>";

            var config = new GenerationConfig();
            config.NamedPatterns.Patterns.Add(new NamedPattern(){Name = "superhero", Pattern = "(Batman|Superman|Spiderman)"});
            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"(Batman|Superman|Spiderman)"));
        }
        
        [TestMethod]
        [TestCategory("Template")]
        public void Can_Load_File_Supplied_In_Config_Absolute()
        {
            var template = @"<<@noun@ @verb@ @noun@ @verb@>>";

            var random = new Random(100);
            var config = new GenerationConfig() { Seed = "200" };
            config.Random = random;
            config.PatternFiles.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg-patterns", "language.tdg-patterns"));

            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.IsTrue(text.Length > 0);
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Load_File_Supplied_In_Config_Relative()
        {
            var template = @"<<@noun@ @verb@ @noun@ @verb@>>";

            var config = new GenerationConfig() { Seed = "100" };
            config.PatternFiles.Add("language.tdg-patterns");

            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.IsTrue(text.Length > 0);
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Load_File_Supplied_In_Config_Relative_No_Extension()
        {
            var template = @"<<@noun@ @verb@ @noun@ @verb@>>";

            var config = new GenerationConfig() { Seed = "100" };
            config.PatternFiles.Add("language");

            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.IsTrue(text.Length > 0);
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Escape_Template()
        {
            var template = @"Generated \<<\L\L>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.AreEqual(text, @"Generated <<\L\L>>");
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_UnEscape_Template()
        {
            var template = @"Generated \\<<\L\L>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"Generated \\\\[A-Z]{2}"));
        }


        [TestMethod]
        [TestCategory("Template")]
        public void Can_Escape_Config()
        {
            var template = @"\<# blah #>abc";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.AreEqual(@"\<# blah #>abc", text);

            // space at start
            template = @". <# blah #>abc";
            text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.AreEqual(@". <# blah #>abc", text);
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_No_Symbols()
        {
            var template = "Generated <<LL>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);            
            StringAssert.Matches(text, new Regex(@"Generated [L]{2}"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_for_ReadMe1()
        {
            var template = @"Hi there <<\L\v{0,2}\l{0,2}\v \L\v{0,2}\l{0,2}\v{0,2}\l{0,2}\l>> how are you doing?";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"Hi there [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou] [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou]{0,2}[a-z]{0,2}[a-z] how are you doing\?"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_for_ReadMe2()
        {
            var template = @"<<aa[1-9]\d\d-\d\d-\d\d\d\d>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"aa[1-9]\d{2}-\d{2}-\d{4}"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_for_ReadMe3()
        { 
            
            var template = @"Hi there <<\L\v{0,2}\l{0,2}\v \L\v{0,2}\l{0,2}\v{0,2}\l{0,2}\l>> how are you doing? Your SSN is <<[1-9]\d\d-\d\d-\d\d\d\d>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            
            StringAssert.Matches(text, new Regex(@"Hi there [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou] [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou]{0,2}[a-z]{0,2}[a-z] how are you doing\? Your SSN is [1-9]\d{2}-\d{2}-\d{4}"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_with_Symbols()
        {
            var template = @"Generated <<L\L>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"Generated [L][A-Z]"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_Multiple()
        {
            var template = @"Generated <<\L\L>> and <<(\d){5}>> with <<\d>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"Generated [A-Z]{2} and [0-9]{5} with [0-9]"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_Harder()
        {
            var template = @"This is a very basic <<(\l){10}>> which can be used to create <<\l\l\l\l>> of varying <<\l\l\l\l\l>>. The main purpose is to generate dummy <<\L\l\l\l>> which can be used for <<\l\l\l\l\l\l\l>>.";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"This is a very basic [a-z]{10} which can be used to create [a-z]{4} of varying [a-z]{5}. The main purpose is to generate dummy [A-Z][a-z]{3} which can be used for [a-z]{7}."));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_With_Alternatives_Symbols()
        {
            var template = @"<<\C|\c{10}|\V\V\V|\v{2,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"[BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{3}|[aeiou]{2,3}"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Template_With_Alternative_Groups()
        {
            var template = @"<<(\C)|\c{10}|(\V\V\V){20}|(\v\v\v\v\v){2,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"[BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{60}|[aeiou]{10,15}"));
        }
        
        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Pattern_With_Alternatives()
        {
            var template = @"Alternatives <<(\C|(\c){10}|\V\V\V|\v{2,3})>>";
            var config = new GenerationConfig();
            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Console.WriteLine("Seed:{0}", config.Seed);
            StringAssert.Matches(text, new Regex(@"Alternatives ([BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{3}|[aeiou]{2,3})"));
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Pattern_With_Alternatives2()
        {
            var template = @"Alternatives <<(A|B){1000}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"Alternatives (A|B){1000}"));
        }


        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Pattern_With_Alternatives_Repeated_Symbols()
        {
            var template = @"Alternatives <<(\C{1}|\c{10}|\V{3}|\v{2,3})>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"Alternatives ([BCDFGHJKLMNPQRSTVWXYZ]{1})|([bcdfghjklmnpqrstvwxyz]{10})|([AEIOU]{3}|[aeiou]{2,3})"));
            if (text.Contains("|")) Assert.Fail(text);
        }

        [TestMethod]
        [TestCategory("Template")]
        public void Can_Generate_From_Pattern_With_calculated_quantity()
        {
            var template = @"<<\v{2,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
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
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^\[LL]$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_NonRange_Set()
        {
            var template = @"<<[AEI]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[AEI]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Set()
        {
            var template = @"<<[A-I]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[ABCDEFGHI]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Set_Lower()
        {
            var template = @"<<[a-i]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[abcdefghi]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Set2()
        {
            var template = @"<<[A-B]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[AB]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Set3()
        {
            var template = @"<<[W-X]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[W-X]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Repeated_Set1()
        {
            var template = @"<<[W-X]{10}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[W-X]{10}$"));
        }


        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Repeated_Set2()
        {
            var template = @"<<([W-X]{10}[W-X]{10})>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[W-X]{10}[W-X]{10}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Repeated_Set3()
        {
            var template = @"<<([W-X]{10,100}[1-9]{10,100})>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[W-X]{10,100}[1-9]{10,100}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Repeated_Set4()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"<<([W-X]{})>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[W-X]$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Section_Repeated_Escape_Chars()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"(\\){10}";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(template));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Repeated_Escape_Chars()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"\\{10}";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(template));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Repeated_Chars()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"(a){10}";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(@"aaaaaaaaaa", new Regex(template));
        }


        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Negated_Range()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"[^5-9]";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(template));
        }
        
        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Negated_Range_And_Characters()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"[^A-Z5-9!£$%^&*()_+]";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(template));
        }


        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric()
        {
            var template = @"<<[1-9]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[1-9]$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric5()
        {
            var template = @"<<[1-8]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[1-8]$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric2()
        {
            var template = @"<<[100-150]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            int e = int.Parse(text);
            if(e < 100 || e>150) Assert.Fail("Number not between 100 and 150.");
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric3()
        {
            var template = @"<<(.[100-101]){3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^(.(100|101)){1}(.(100|101)){1}(.(100|101)){1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric4()
        {
            var template = @"<<(.[100-101]){1,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^(.(100|101))?(.(100|101))?(.(100|101))?$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat()
        {
            var template = @"<<([1.00-10.00])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            double d;
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                Assert.Fail();
            if(d<1.00d || d>10.00d)
                Assert.Fail();
        }

 
        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat2()
        {
            var template = @"<<([1.00-2.00])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            double d;
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                Assert.Fail();
            if (d < 1.00 || d > 2.00d) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat3()
        {
            var template = @"<<([1.1-1.2])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            double d;
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                Assert.Fail();
            if (d < 1.1 || d > 1.2d) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat4()
        {
            var template = @"<<([12345.12345-12345.12346])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            double d;
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                Assert.Fail();
            if (d < 12345.12345 || d > 12345.12346d) Assert.Fail();
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat5()
        {
            var template = @"<<([12345.9999-12346])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            double d;
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                Assert.Fail();
            if (d < 12345.9999d || d > 12346d) Assert.Fail();
        }
        
        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_MultipleRange_Set()
        {
            var template = @"<<[A-B][1-3]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[A-B]{1}[1-3]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_MultipleRange_Set2()
        {
            var template = @"<<[1-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[1-9]{1}[0-9]{1}[0-9]{1}-[0-9]{1}[0-9]{1}-[0-9]{1}[0-9]{1}[0-9]{1}[0-9]{1}$"));
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_MultipleRange_Set3()
        {
            var template = @"<<[1-28]/[1-12]/[1960-2013]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            DateTime dt;
            if(DateTime.TryParseExact(text, @"d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt) == false) Assert.Fail("invalid Date");
        
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_MultipleRange_Set4()
        {
            var template = @"<<[a-c1-3_]{100}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[a-c1-3_]{100}$"));
            Assert.IsTrue(text.Contains("_"));  // check that we have produced at least 1 underscore.
        }

        [TestMethod]
        [TestCategory("Sets")]
        public void Can_Generate_MultipleRange_Set5()
        {
            var template = @"<<[a-c1-3_*?ABC]{100}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"^[a-c1-3_*?ABC]{100}$"));
            Assert.IsTrue(text.Contains("_"));
            Assert.IsTrue(text.Contains("*"));
            Assert.IsTrue(text.Contains("?"));
            Assert.IsTrue(text.Contains("A"));
            Assert.IsTrue(text.Contains("B"));
            Assert.IsTrue(text.Contains("C"));
        }

        #endregion

        #region ErrorMessages

        [TestMethod]
        [TestCategory("ErrorMessages")]
        public void Can_BuildErrorSnippet_Start()
        {
            var template = @"This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  ";
            int ndx = 0;
            string text = AlphaNumericGenerator.BuildErrorSnippet(template, ndx);
            Console.WriteLine(@"Error Snippet for '{0}'
 at index {1} produced 
'{2}'", template, ndx, text);
            Assert.AreEqual(AlphaNumericGenerator.ErrorContext*2+4, text.Length);
        }

        [TestMethod]
        [TestCategory("ErrorMessages")]
        public void Can_BuildErrorSnippet_End()
        {
            var template = @"This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  ";
            int ndx = template.Length-1;
            string text = AlphaNumericGenerator.BuildErrorSnippet(template, ndx);
            Console.WriteLine(@"Error Snippet for '{0}'
 at index {1} produced 
'{2}'", template, ndx, text);
            Assert.AreEqual(AlphaNumericGenerator.ErrorContext*2+4, text.Length);
        }

        [TestMethod]
        [TestCategory("ErrorMessages")]
        public void Can_BuildErrorSnippet_Middle()
        {
            var template = @"This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  This is a long string.  ";
            int ndx = AlphaNumericGenerator.ErrorContext + 20;
            string text = AlphaNumericGenerator.BuildErrorSnippet(template, ndx);
            Console.WriteLine(@"Error Snippet for '{0}'
 at index {1} produced 
'{2}'", template, ndx, text);
            Assert.AreEqual(AlphaNumericGenerator.ErrorContext * 4 + 4, text.Length);
        }
        
        [TestMethod]
        [TestCategory("ErrorMessages")]
        public void Can_BuildErrorSnippet1()
        {
            var template = @"([100-900]{40])";
            int ndx = 12;
            string text = AlphaNumericGenerator.BuildErrorSnippet(template, ndx);
            Console.WriteLine(@"Error Snippet for '{0}'
 at index {1} produced 
'{2}'", template, ndx, text);
            Assert.AreEqual(template.Length*2+2, text.Length);
        }

        [TestMethod]
        [TestCategory("ErrorMessages")]
        public void Can_BuildErrorSnippet2()
        {
            var template = @"([100-900]{40])";
            int ndx = 12;
            string text = AlphaNumericGenerator.BuildErrorSnippet(template, ndx);
            Console.WriteLine(@"Error Snippet for '{0}'
 at index {1} produced 
'{2}'", template, ndx, text);
            Assert.AreEqual(template.Length * 2 + 2, text.Length);
        }

        #endregion

        #region Pattern

        [TestMethod]
        [TestCategory("Pattern")]
        public void GeneratePattern_Overloads()
        {
            var pattern = @"\L\L\L\L\L\L-\L\L-\L\L\L\L\L";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void GeneratePattern_Overloads2()
        {
            var pattern = @"\L\L\L\L\L\L-\L\L-\L\L\L\L\L";
            var config = new GenerationConfig(){Seed = "300"};
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern, config: config);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(15, text.Length);
            Assert.AreEqual(text,"LVMPQS-IY-CXIRW");
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_AlphaNumeric_multiple()
        {

            var pattern = @"\L\L\L\L\L\L-\L\L-\L\L\L\L\L";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));

            pattern = @"\l\l\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[a-z]*"));

            pattern = @"\d\d\d\d\d\d\L\L\L\L\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(14, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"));

            pattern = @"\d\d\d-\d\d-\d\d\d\d";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(11, text.Length);
            StringAssert.Matches(text, new Regex(@"[\d]{3}-[\d]{2}-[\d]{4}"));

            //Test for escaped characters.
            pattern = @"L\LLLLL";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("L[A-Z]{1}LLLL"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_AlphaNumeric()
        {

            var pattern = @"\L\l\V\v\C\c\d\d";
            string text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{1}[a-z]{1}[AEIOU]{1}[aeiou]{1}[QWRTYPSDFGHJKLZXCVBNM]{1}[qwrtypsdfghjklzxcvbnm]{1}[\d]{1}[\d]{1}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Simple_Patterns()
        {
            var pattern = @"\L\L\L\L\L\L-\L\L-\L\L\L\L\L";
            string text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(15, text.Length);
            StringAssert.Matches(text, new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"));

            pattern = @"\l\l\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("[a-z]*"));

            pattern = @"\d\d\d\d\d\d\L\L\L\L\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(14, text.Length);
            StringAssert.Matches(text, new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"));

            pattern = @"[1-9]\d\d-\d\d-\d\d\d\d";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(11, text.Length);
            StringAssert.Matches(text, new Regex(@"[1-9]\d{2}-\d{2}-\d{4}"));

            //Test for escaped characters.
            pattern = @"L\LLLLL";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(6, text.Length);
            StringAssert.Matches(text, new Regex("L[A-Z]{1}LLLL"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Character()
        {
            var pattern = @"w{3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"w{3}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Character_Inside_Group()
        {
            var pattern = @"(\dC{3}\d{3}){3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^(\dC{3}\d{3}){3}$"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Character_Inside_Group2()
        {
            var pattern = @"(\d(C){3}){3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^(\d(C){3}){3}$"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Character_Inside_Group3()
        {
            var pattern = @"(\d(C){3})";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^(\d(C){3})$"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Character_Inside_Group4()
        {
            var pattern = @"(\d(\\)\d)";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^(\d(\\)\d$)"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Symbol_Inside_Group()
        {
            var pattern = @"(\w{3})";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"\w{3}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Symbol_Inside_Group2()
        {
            var pattern = @"(\w(\d{2}|\v{2})\w{3})";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"\w(\d{2}|[aeiou]{2})\w{3}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Pattern()
        {
            var pattern = @"(\L\L\d\d){3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Symbol()
        {
            var pattern = @"\L{3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{3}"));
        }
        
        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Mixed_Pattern_With_Random_Length()
        {
            string pattern = @"\L{10,20}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{10,20}"));

        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Output_NonEscaped_Symbols()
        {
            string pattern = @"X";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"X"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Output_Repeated_Symbols()
        {
            string pattern = @"\d{10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"[\d]{10}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Output_Escaped_Slash()
        {
            string pattern = @"[\\]{1,10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"[\\]{1,10}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Mixed_Pattern()
        {
            string pattern = @"\L\L(\L\L\d\d){3}\L\L(\L){23}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[A-Z]{23}"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Repeat_Pattern_Long()
        {
            var pattern = @"(\L){1}(\d){1}(\L){2}(\d){2}(\L){4}(\d){4}(\L){8}(\d){8}(\L){16}(\d){16}(\L){32}(\d){32}(\L){64}(\d){64}(\L){128}(\d){128}(\L){256}(\d){256}(\L){512}(\d){512}(\L){1024}(\d){1024}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"[A-Z]{1}[0-9]{1}[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{4}[A-Z]{8}[0-9]{8}[A-Z]{16}[0-9]{16}[A-Z]{32}[0-9]{32}[A-Z]{64}[0-9]{64}[A-Z]{128}[0-9]{128}[A-Z]{256}[0-9]{256}[A-Z]{512}[0-9]{512}[A-Z]{1024}[0-9]{1024}"));
            Assert.AreEqual((1+2+4+8+16+32+64+128+256+512+1024)*2, text.Length);
        }
        
        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Expressions_With_Alternates()
        {
            var pattern = @"(\L\L|\d\d)";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^([A-Z]{2}|[0-9]{2})$"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Expressions_With_Alternates2()
        {
            var pattern = @"(\L\L|\d\d|[AEIOU]|[100-120])";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^([A-Z]{2}|[0-9]{2}|[AEIOU]|\d\d\d)$"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Zero_Repeats_Invalid_End()
        {
            var pattern = @"(\L\L\d\d)33}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^[A-Z]{2}[0-9]{2}33}$"));
        }
        
        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Zero_Repeats()
        {
            var pattern = @"\L\L\d\d";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^[A-Z]{2}[0-9]{2}$"));
        }

        
        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Complex1()
        {
            var pattern = @"\{'\w':('\w{3,25}'|[1-100])\}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^\{'\w':('\w{3,25}'|\d{1,3})\}$"));
        }


        #endregion

        #region Symbols

        [TestMethod]
        [TestCategory("Symbols")]
        public void Can_Generate_All_Symbols()
        {
            var pattern = @"\.";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^.$"));

            pattern = @"\a";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^[A-Za-z]$"));

            pattern = @"\W";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^\W$"));

            pattern = @"\w";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^\w$"));

            pattern = @"\L";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^[A-Z]$"));

            pattern = @"\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^[a-z]$"));

            pattern = @"\V";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^[AEIOU]$"));

            pattern = @"\v";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^[aeiou]$"));

            pattern = @"\C";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^[BCDFGHJKLMNPQRSTVWXYZ]$"));

            pattern = @"\c";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^[bcdfghjklmnpqrstvwxyz]$"));

            pattern = @"\D";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^\D$"));

            pattern = @"\d";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^\d$"));

            pattern = @"\s";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^\s$", RegexOptions.ECMAScript));  // ECMA compliant needed as \s ECMA includes [SPACE] but .NET Regex does not.
            
            pattern = @"\t";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^\t$"));

            pattern = @"\n";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^\n$"));

            pattern = @"\r";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^\r$"));

            pattern = @"\\";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            StringAssert.Matches(text, new Regex(@"^\\$"));
        }

        #endregion

        #region NamedPatterns

        [TestMethod]
        [TestCategory("NamedPatterns")]
        public void Can_Generate_NamedPatterns()
        {
            //var namedPatterns = FileReader.LoadNamedPatterns("default.tdg-patterns");

            var pattern = @"<<(@name_firstname_male@)>>";
            var text = AlphaNumericGenerator.GenerateFromTemplate(pattern, new GenerationConfig(){LoadDefaultPatternFile = true});
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.IsTrue(text.Length>0);
            
        }
        
        [TestMethod]
        [TestCategory("NamedPatterns")]
        public void Can_Generate_NamedPatterns_CompoundPattern()
        {
            //var namedPatterns = FileReader.LoadNamedPatterns("default.tdg-patterns");

            var pattern = @"<<(@address_us_type1@)>>";
            var text = AlphaNumericGenerator.GenerateFromTemplate(pattern, new GenerationConfig(){LoadDefaultPatternFile = true});
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.IsTrue(text.Length>0);
            
        }

        [TestMethod]
        [TestCategory("NamedPatterns")]
        public void Can_Generate_NamedPatterns_All_Defaults_Name()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var namedPatterns = FileReader.LoadNamedPatterns(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg-patterns", "default.tdg-patterns"));

            foreach (var dic in namedPatterns.Patterns)
            {
                var text = AlphaNumericGenerator.GenerateFromPattern("@"+dic.Name+"@", config: new GenerationConfig(){LoadDefaultPatternFile = true});
                Console.WriteLine(@"'{0}' produced '{1}'", dic.Name, text);
                Assert.IsTrue(text.Length > 0);
            }
            sw.Stop();
            Console.WriteLine(@"All {0} default patterns generated in {1} milliseconds."
                                            , namedPatterns.Patterns.Count
                                            , sw.ElapsedMilliseconds);
        }

        [TestMethod]
        [TestCategory("NamedPatterns")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Invalid_Named_Pattern()
        {
            var config = new GenerationConfig()
            {
                PatternFiles =
                    new List<string>()
                    {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg-patterns", "invalid.tdg-patterns")
                    }
            };

            AlphaNumericGenerator.GenerateFromPattern("@blah@", config:config);
        }

        [TestMethod]
        [TestCategory("NamedPatterns")]
        public void Can_Generate_NamedPatterns_All_Defaults_Patterns()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var namedPatterns = FileReader.LoadNamedPatterns(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg-patterns", "default.tdg-patterns"));

            foreach (var dic in namedPatterns.Patterns)
            {
                var text = AlphaNumericGenerator.GenerateFromPattern(dic.Pattern, config:new GenerationConfig() { LoadDefaultPatternFile = true });
                Console.WriteLine(@"'{0}' produced '{1}'", dic.Name, text);
                Assert.IsTrue(text.Length > 0);
            }
            sw.Stop();
            Console.WriteLine(@"All {0} default patterns generated in {1} milliseconds."
                                            , namedPatterns.Patterns.Count
                                            , sw.ElapsedMilliseconds);
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
                //Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            }
            sw.Stop();

            Console.WriteLine(@"{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern);
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
                //Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            }
            sw.Stop();

            Console.WriteLine(@"{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern);
        }

        [TestMethod]
        [TestCategory("Profiling")]
        public void Profile_Large_NonRandom_Repeat()
        {
            var pattern = @"<<\L{1}\d{1}\L{2}\d{2}\L{4}\d{4}\L{8}\d{8}\L{16}\d{16}\L{32}\d{32}\L{64}\d{64}\L{128}\d{128}\L{256}\d{256}\L{512}\d{512}\L{1024}\d{1024}>>";
            var testLimit = 1000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (var i = 0; i < testLimit; i++)
            {
                var text = AlphaNumericGenerator.GenerateFromTemplate(pattern);
                //Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            }
            sw.Stop();
            Console.WriteLine(@"{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern);
        }

        [TestMethod]
        [TestCategory("Profiling")]
        public void Profile_Large_NonRandom_NonSeeded_Repeat()
        {
            var pattern = @"(\w){64}";
            var testLimit = 10000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (var i = 0; i < testLimit; i++)
            {
                var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
                //Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            }
            sw.Stop();
            Console.WriteLine(@"{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern);
        }

        [TestMethod]
        [TestCategory("Profiling")]
        public void Profile_Large_NonRandom_Seeded_Repeat()
        {
            var pattern = @"(\w){64}";
            var testLimit = 10000;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (var i = 0; i < testLimit; i++)
            {
                var text = AlphaNumericGenerator.GenerateFromPattern(pattern, config: new GenerationConfig(){Seed = "100"});
                //Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            }
            sw.Stop();
            Console.WriteLine(@"{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern);
        }

        [TestMethod]
        [TestCategory("Profiling")]
        public void Profile_Small_NonRandom_Seeded_Repeat()
        {
            var pattern = @"(\w){64}";
            var testLimit = 100;
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            for (var i = 0; i < testLimit; i++)
            {
                var text = AlphaNumericGenerator.GenerateFromPattern(pattern, config: new GenerationConfig() { Seed = "100" });
                //Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            }
            sw.Stop();
            Console.WriteLine(@"{0} instances of the following template generated in {1} milliseconds.\n'{2}'", testLimit, sw.ElapsedMilliseconds, pattern);
        }
        
        #endregion

        #region Negation


        [TestMethod]
        [TestCategory("Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set()
        {
            var pattern = @"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var text = AlphaNumericGenerator.GenerateFromPattern("[^"+pattern+"]");
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.IsTrue(pattern.IndexOf(text, StringComparison.InvariantCulture) == -1);
        }

        [TestMethod]
        [TestCategory("Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set_Range()
        {
            var pattern = @"A-Z";
            var text = AlphaNumericGenerator.GenerateFromPattern("[^" + pattern + "]");
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.IsTrue(pattern.IndexOf(text, StringComparison.InvariantCulture) == -1);
        }

        [TestMethod]
        [TestCategory("Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set_Range2()
        {
            var pattern = @"3-6";
            var text = AlphaNumericGenerator.GenerateFromPattern("[^" + pattern + "]");
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.IsTrue("3456".IndexOf(text, StringComparison.InvariantCultureIgnoreCase) == -1);
        }

        [TestMethod]
        [TestCategory("Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set_Range_Multiple()
        {
            var pattern = @"A-Za-z";
            var text = AlphaNumericGenerator.GenerateFromPattern("[^" + pattern + "]");
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.IsTrue("ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(text, StringComparison.InvariantCultureIgnoreCase) == -1);
        }

        [TestMethod]
        [TestCategory("Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set_Range_Repeated()
        {
            var pattern = @"[^3-6]{10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.IsTrue("3456".IndexOf(text, StringComparison.InvariantCultureIgnoreCase) == -1);
        }

        [TestMethod]
        [TestCategory("Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set_Range_Multiple_Repeated()
        {
            var pattern = @"[^A-Za-z]{10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.IsTrue("ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(text, StringComparison.InvariantCultureIgnoreCase) == -1);
        }
        
        #endregion

        #region NegativeTesting

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Missing_Placeholder_End()
        {
            var template = "This is a very basic <<(l){10}>> which can be used to create <<llll of varying <<lllll>>. The main purpose is to generate dummy <<Llll>> which can be used for <<lllllll>>.";
            AlphaNumericGenerator.GenerateFromTemplate(template);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Missing_Placeholder_End_End()
        {
            var template = "This is a very basic <<(l){10}>> which can be used to create <<llll>> of varying <<lllll>>. The main purpose is to generate dummy <<Llll>> which can be used for <<lllllll.";
            AlphaNumericGenerator.GenerateFromTemplate(template);
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
            string pattern = "(L){10,}";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Mixed_Pattern_With_Invalid_Random_Length_Min_Max()
        {
            string pattern = "(L){10,0}";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Invalid_Repeat_Pattern()
        {
            var pattern = "(LLXX){w}";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Unknown_NamedPatterns()
        {
            var pattern = @"<<(@blahblahblah21@)>>";
            AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Negated_Set_Range_InvalidNumeric()
        {
            var pattern = @"[^30-60]";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Negated_Set_Range_InvalidNumeric2()
        {
            var pattern = @"[^3-60]";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Negated_Set_Range_InvalidNumeric3()
        {
            var pattern = @"[^3.00-6]";
            AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Invalid_Config()
        {
            var pattern = @"<#{ asdsd }#>";
            AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Exception_Invalid_Config2()
        {
            var pattern = @"<#";
            AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Missing_Pattern_File()
        {
            var template = @"<<@superhero@>>";

            var namedPatterns = new NamedPatterns();
            namedPatterns.Patterns.Add(new NamedPattern() { Name = "superhero", Pattern = "(Batman|Superman|Spiderman)" });

            var config = new GenerationConfig() { Seed = "100" };
            config.PatternFiles.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg-patterns", "notpresent.tdg-pattern"));
            config.NamedPatterns = namedPatterns;

            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"(Batman|Superman|Spiderman)"));
        }


        [TestMethod]
        [TestCategory("NegativeTesting")]
        [ExpectedException(typeof(GenerationException))]
        public void Can_Throw_Invalid_Pattern_File()
        {
            var template = @"<<@superhero@>>";

            var namedPatterns = new NamedPatterns();
            namedPatterns.Patterns.Add(new NamedPattern() { Name = "superhero", Pattern = "(Batman|Superman|Spiderman)" });

            var config = new GenerationConfig() { Seed = "100" };
            config.PatternFiles.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg.exe"));
            config.NamedPatterns = namedPatterns;

            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            StringAssert.Matches(text, new Regex(@"(Batman|Superman|Spiderman)"));
        }
        
        #endregion

        #region ConfigurationTesting

        [TestMethod]
        [TestCategory("ConfigurationTesting")]
        public void Can_Configure_Random_Seed_From_Config()
        {
            int ndx = 0;

            var configStr = "<# { 'Seed':100 } #>";
            var template = configStr+@"Generated <<L\L>>";
            var config = AlphaNumericGenerator.GetConfiguration(template, ref ndx);
            Assert.AreEqual("100", config.Seed);
            Assert.AreEqual(configStr.Length, ndx);
        }

        [TestMethod]
        [TestCategory("ConfigurationTesting")]
        public void Can_Configure_And_Produce_Output_With_Seed()
        {
            var template = "<# { \"Seed\":\"100\" } #>Generated <<L\\L>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);

            Assert.AreEqual(text, "Generated LZ");
        }

        [TestMethod]
        [TestCategory("ConfigurationTesting")]
        public void Can_Configure_And_Produce_Output_With_Seed2()
        {
            var template = @"<# { 'Seed':100 } #>Generated <<\.\w\W\L\l\V\v\d\D\S\s>>";
            string actual = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, actual);

            Assert.AreEqual(@"Generated |k]XjUo6Go​", actual);
        }

        [TestMethod]
        [TestCategory("ConfigurationTesting")]
        public void Can_Catch_Config_Not_At_Beginning_Template()
        {
            var template = @"<# { 'Seed':100 } #>Generated <<\.\w\W\L\l\V\v\d\D\S\s>>";
            int index = 1;
            var config = AlphaNumericGenerator.GetConfiguration(template, ref index);
            Assert.IsNull(config);
        }


        #endregion

        #region UtilityTesting

        [TestMethod]
        [TestCategory("UtilityTesting")]
        public void Can_Deserialize()
        {
            var config = Utility.DeserializeJson<GenerationConfig>("{\"seed\":\"100\"}");
            Assert.IsNotNull(config);
            Assert.AreEqual("100", config.Seed);
        }

        [TestMethod]
        [TestCategory("UtilityTesting")]
        public void Can_Serialize()
        {
            var config = new GenerationConfig();
            config.Seed = "300";
            var configStr = Utility.SerializeJson(config);
            Console.WriteLine("SerializeJson produced" + configStr);
            Assert.IsNotNull(configStr);
            Assert.AreEqual("{\"LoadDefaultPatternFile\":false,\"NamedPatterns\":{\"Patterns\":[]},\"patternfiles\":[],\"seed\":\"300\"}", configStr);
        }

        #endregion

        #region "special functions"

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Anagram()
        {
            var pattern = @"[ABC]{:anagram:}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(3, text.Length);
            Assert.IsTrue(text.Contains("A"));
            Assert.IsTrue(text.Contains("B"));
            Assert.IsTrue(text.Contains("C"));
        }

        [TestMethod]
        [TestCategory("Pattern")]
        public void Can_Generate_Anagram_Long()
        {
            var input = "abcdefghijklmnopqrstuvwxyz";
            var pattern = @"["+input+"]{:anagram:}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.AreEqual(input.Length, text.Length);
            foreach (var ch in input.ToCharArray())
            {
                Assert.IsTrue(text.Contains(ch.ToString()));  
            }
        }

        #endregion

        #region Randomness
        [TestMethod]
        [TestCategory("Randomness")]
        public void Level_Of_Randomness()
        {
            var pattern = @"(\L\L\L\L\L\L-\L\L-\L\L\L\L\L\n){1000}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern, config:new GenerationConfig() { Seed = "100" });
            var segments = new List<string>(text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));

            Console.WriteLine(@"'{0}' produced {1} values, out of which, {2} are unique and {3} are duplicates.", pattern, segments.Count, segments.Distinct().Count(), segments.Count - segments.Distinct().Count());
        }

        [TestMethod]
        [TestCategory("Randomness")]
        public void Level_Of_Loop()
        {
            var pattern = @"(\L\L\L\L\L\L-\L\L-\L\L\L\L\L)";
            var segments = new List<string>();

            var config = new GenerationConfig();

            for (var i = 0; i < 1000; i++)
            {
                segments.Add(AlphaNumericGenerator.GenerateFromPattern(pattern, config:config));
            }

            Console.WriteLine(@"'{0}' produced {1} values, out of which, {2} are unique and {3} are duplicates.", pattern, segments.Count, segments.Distinct().Count(), segments.Count - segments.Distinct().Count());
        }
        #endregion
    }
}

