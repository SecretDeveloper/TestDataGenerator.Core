using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TestDataGenerator.Core;
using TestDataGenerator.Core.Exceptions;
using TestDataGenerator.Core.Generators;
using Xunit;


namespace TestDataGenerator.Tests
{
    public class TextTests
    {
        private const int _ErrorSnippet_ContextLength = 50;

        #region Template

        [Fact]
        public void Can_GenerateFromTemplate_Overload1()
        {
            var template = @"Generated <<\L\L>>";
            string text = TestDataGenerator.Core.Generators.AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"Generated [A-Z]{2}"), text);
        }

        [Fact]

        public void Can_GenerateFromTemplate_Overload2()
        {
            var template = @"Generated <<\L\L>>";
            var config = new GenerationConfig() {Seed = "100"};
            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"Generated [A-Z]{2}"), text);
        }

        [Fact]

        public void Can_GenerateFromTemplate_Overload3()
        {
            var template = @"<<@superhero@>>";

            var config = new GenerationConfig();
            config.NamedPatterns.Patterns.Add(new NamedPattern()
            {
                Name = "superhero",
                Pattern = "(Batman|Superman|Spiderman)"
            });
            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"(Batman|Superman|Spiderman)"), text);
        }

        [Fact]

        public void Can_Load_File_Supplied_In_Config_Absolute()
        {
            var template = @"<<@noun@ @verb@ @noun@ @verb@>>";

            var random = new Random(100);
            var config = new GenerationConfig() {Seed = "200"};
            config.Random = random;
            config.PatternFiles.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg-patterns", "language.tdg-patterns"));

            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.True(text.Length > 0);
        }

        [Fact]

        public void Can_Load_File_Supplied_In_Config_Relative()
        {
            var template = @"<<@noun@ @verb@ @noun@ @verb@>>";

            var config = new GenerationConfig() {Seed = "100"};
            config.PatternFiles.Add("language.tdg-patterns");

            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.True(text.Length > 0);
        }

        [Fact]

        public void Can_Load_File_Supplied_In_Config_Relative_No_Extension()
        {
            var template = @"<<@noun@ @verb@ @noun@ @verb@>>";

            var config = new GenerationConfig() {Seed = "100"};
            config.PatternFiles.Add("language");

            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.True(text.Length > 0);
        }

        [Fact]

        public void Can_Escape_Template()
        {
            var template = @"Generated \<<\L\L>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Equal(text, @"Generated <<\L\L>>");
        }

        [Fact]

        public void Can_UnEscape_Template()
        {
            var template = @"Generated \\<<\L\L>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"Generated \\\\[A-Z]{2}"), text);
        }


        [Fact]
        public void Can_Escape_Config()
        {
            var template = @"\<# blah #>abc";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Equal(@"\<# blah #>abc", text);

            // space at start
            template = @". <# blah #>abc";
            text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Equal(@". <# blah #>abc", text);
        }

        [Fact]

        public void Can_Generate_From_Template_No_Symbols()
        {
            var template = "Generated <<LL>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"Generated [L]{2}"), text);
        }

        [Fact]
        public void Can_Generate_From_Template_for_ReadMe1()
        {
            var template = @"Hi there <<\L\v{0,2}\l{0,2}\v \L\v{0,2}\l{0,2}\v{0,2}\l{0,2}\l>> how are you doing?";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"Hi there [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou] [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou]{0,2}[a-z]{0,2}[a-z] how are you doing\?"), text);
        }

        [Fact]
        public void Can_Generate_From_Template_for_ReadMe2()
        {
            var template = @"<<aa[1-9]\d\d-\d\d-\d\d\d\d>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"aa[1-9]\d{2}-\d{2}-\d{4}"), text);
        }

        [Fact]
        public void Can_Generate_From_Template_for_ReadMe3()
        {

            var template =
                @"Hi there <<\L\v{0,2}\l{0,2}\v \L\v{0,2}\l{0,2}\v{0,2}\l{0,2}\l>> how are you doing? Your SSN is <<[1-9]\d\d-\d\d-\d\d\d\d>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);

            Assert.Matches(new Regex(
                    @"Hi there [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou] [A-Z][aeiou]{0,2}[a-z]{0,2}[aeiou]{0,2}[a-z]{0,2}[a-z] how are you doing\? Your SSN is [1-9]\d{2}-\d{2}-\d{4}")
                    , text);
        }

        [Fact]
        public void Can_Generate_From_Template_with_Symbols()
        {
            var template = @"Generated <<L\L>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"Generated [L][A-Z]"), text);
        }

        [Fact]
        public void Can_Generate_From_Template_Multiple()
        {
            var template = @"Generated <<\L\L>> and <<(\d){5}>> with <<\d>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"Generated [A-Z]{2} and [0-9]{5} with [0-9]"), text);
        }

        [Fact]
        public void Can_Generate_From_Template_Harder()
        {
            var template =
                @"This is a very basic <<(\l){10}>> which can be used to create <<\l\l\l\l>> of varying <<\l\l\l\l\l>>. The main purpose is to generate dummy <<\L\l\l\l>> which can be used for <<\l\l\l\l\l\l\l>>.";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(
                    @"This is a very basic [a-z]{10} which can be used to create [a-z]{4} of varying [a-z]{5}. The main purpose is to generate dummy [A-Z][a-z]{3} which can be used for [a-z]{7}.")
                    , text);
        }

        [Fact]
        public void Can_Generate_From_Template_With_Alternatives_Symbols()
        {
            var template = @"<<\C|\c{10}|\V\V\V|\v{2,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"[BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{3}|[aeiou]{2,3}"), text);
        }

        [Fact]
        public void Can_Generate_From_Template_With_Alternative_Groups()
        {
            var template = @"<<(\C)|\c{10}|(\V\V\V){20}|(\v\v\v\v\v){2,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(
                new Regex(@"[BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{60}|[aeiou]{10,15}"), text);
        }

        [Fact]
        public void Can_Generate_From_Pattern_With_Alternatives()
        {
            var template = @"Alternatives <<(\C|(\c){10}|\V\V\V|\v{2,3})>>";
            var config = new GenerationConfig();
            string text = AlphaNumericGenerator.GenerateFromTemplate(template, config);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Console.WriteLine("Seed:{0}", config.Seed);
            Assert.Matches(
                new Regex(
                    @"Alternatives ([BCDFGHJKLMNPQRSTVWXYZ]{1}|[bcdfghjklmnpqrstvwxyz]{10}|[AEIOU]{3}|[aeiou]{2,3})"), text);
        }

        [Fact]
        public void Can_Generate_From_Pattern_With_Alternatives2()
        {
            var template = @"Alternatives <<(A|B){1000}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"Alternatives (A|B){1000}"), text);
        }


        [Fact]

        public void Can_Generate_From_Pattern_With_Alternatives_Repeated_Symbols()
        {
            var template = @"Alternatives <<(\C{1}|\c{10}|\V{3}|\v{2,3})>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(
                    @"Alternatives ([BCDFGHJKLMNPQRSTVWXYZ]{1})|([bcdfghjklmnpqrstvwxyz]{10})|([AEIOU]{3}|[aeiou]{2,3})"), text);
            if (text.Contains("|")) Assert.True(false, text);
        }

        [Fact]
        public void Can_Generate_From_Pattern_With_calculated_quantity()
        {
            var template = @"<<\v{2,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"([aeiou]{2,3})"), text);
            if (text.Contains("|")) Assert.True(false, text);
        }

        #endregion

        #region Sets

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Escape_Sets()
        {
            var template = @"<<\[LL]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^\[LL]$"), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_NonRange_Set()
        {
            var template = @"<<[AEI]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[AEI]{1}$"), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Set()
        {
            var template = @"<<[A-I]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[ABCDEFGHI]{1}$"), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Set_Lower()
        {
            var template = @"<<[a-i]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[abcdefghi]{1}$"), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Set2()
        {
            var template = @"<<[A-B]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[AB]{1}$"), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Set3()
        {
            var template = @"<<[W-X]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[W-X]{1}$"), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Repeated_Set1()
        {
            var template = @"<<[W-X]{10}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[W-X]{10}$"), text);
        }


        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Repeated_Set2()
        {
            var template = @"<<([W-X]{10}[W-X]{10})>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[W-X]{10}[W-X]{10}$"), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Repeated_Set3()
        {
            var template = @"<<([W-X]{10,100}[1-9]{10,100})>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[W-X]{10,100}[1-9]{10,100}$"), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Repeated_Set4()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"<<([W-X]{})>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[W-X]$"),text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Section_Repeated_Escape_Chars()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"(\\){10}";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(template), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Repeated_Escape_Chars()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"\\{10}";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(template), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Repeated_Chars()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"(a){10}";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches( new Regex(template), @"aaaaaaaaaa");
        }


        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Negated_Range()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"[^5-9]";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(template), text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Negated_Range_And_Characters()
        {
            // empty repeat expressions should result in a single instance - {} == {1}
            var template = @"[^A-Z5-9!£$%^&*()_+]";
            string text = AlphaNumericGenerator.GenerateFromPattern(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(template), text);
        }


        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Numeric()
        {
            var template = @"<<[1-9]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            var re = new Regex(@"^[1-9]$");
            Assert.Matches(re, text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Numeric5()
        {
            var template = @"<<[1-8]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            var re = new Regex(@"^[1-8]$");
            Assert.Matches(re, text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Numeric2()
        {
            var template = @"<<[100-150]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            int e = int.Parse(text);
            if (e < 100 || e > 150) Assert.True(false, "Number not between 100 and 150.");
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Numeric3()
        {
            var template = @"<<(.[100-101]){3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            var re = new Regex(@"^(.(100|101)){1}(.(100|101)){1}(.(100|101)){1}$");
            Assert.Matches(re, text);

        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Numeric4()
        {
            var template = @"<<(.[100-101]){1,3}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            var re = new Regex(@"^(.(100|101))?(.(100|101))?(.(100|101))?$");
            Assert.Matches(re, text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat()
        {
            var template = @"<<([1.00-10.00])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            double d;
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                Assert.False(true);
            if (d < 1.00d || d > 10.00d)
                Assert.True(false);
        }


        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat2()
        {
            var template = @"<<([1.00-2.00])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            double d;
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                Assert.True(false);
            if (d < 1.00 || d > 2.00d) Assert.True(false);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat3()
        {
            var template = @"<<([1.1-1.2])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            double d;
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                Assert.True(false);
            if (d < 1.1 || d > 1.2d) Assert.True(false);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat4()
        {
            var template = @"<<([12345.12345-12345.12346])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            double d;
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                Assert.True(false);
            if (d < 12345.12345 || d > 12345.12346d) Assert.True(false);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_Range_Numeric_DecimalFormat5()
        {
            var template = @"<<([12345.9999-12346])>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            double d;
            if (!double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
                Assert.True(false);
            if (d < 12345.9999d || d > 12346d) Assert.True(false);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_MultipleRange_Set()
        {
            var template = @"<<[A-B][1-3]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);

            var re = new Regex(@"^[A-B]{1}[1-3]{1}$");
            Assert.Matches(re, text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_MultipleRange_Set2()
        {
            var template = @"<<[1-9][0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            var re = new Regex(@"^[1-9]{1}[0-9]{1}[0-9]{1}-[0-9]{1}[0-9]{1}-[0-9]{1}[0-9]{1}[0-9]{1}[0-9]{1}$");

            Assert.Matches(re, text);
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_MultipleRange_Set3()
        {
            var template = @"<<[1-28]/[1-12]/[1960-2013]>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            DateTime dt;
            if (
                DateTime.TryParseExact(text, @"d/M/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal,
                    out dt) == false) Assert.True(false, "invalid Date");

        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_MultipleRange_Set4()
        {
            var template = @"<<[a-c1-3_]{100}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[a-c1-3_]{100}$"), text);
            Assert.True(text.Contains("_")); // check that we have produced at least 1 underscore.
        }

        [Fact]
        [Trait("Category", "Sets")]
        public void Can_Generate_MultipleRange_Set5()
        {
            var template = @"<<[a-c1-3_*?ABC]{100}>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);
            Assert.Matches(new Regex(@"^[a-c1-3_*?ABC]{100}$"), text);
            Assert.True(text.Contains("_"));
            Assert.True(text.Contains("*"));
            Assert.True(text.Contains("?"));
            Assert.True(text.Contains("A"));
            Assert.True(text.Contains("B"));
            Assert.True(text.Contains("C"));
        }

        #endregion

        #region ErrorMessages

        [Fact]
        [Trait("Category", "ErrorMessages")]
        public void Can_BuildErrorSnippet_Start()
        {
            var template = @"[a-z";
            try
            {
                string text = AlphaNumericGenerator.GenerateFromPattern(template);
            }
            catch (GenerationException genEx)
            {
                Assert.Equal(@"Expected ']' but it was not found.
[a-
 ^   ", genEx.Message);
            }
            catch (Exception)
            {
                Assert.True(false, "Incorrect exception thrown.");
            }
        }

        #endregion

        #region Pattern

        [Fact]
        [Trait("Category", "Pattern")]
        public void GeneratePattern_Overloads()
        {
            var pattern = @"\L\L\L\L\L\L-\L\L-\L\L\L\L\L";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(15, text.Length);
            Assert.Matches(new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void GeneratePattern_Overloads2()
        {
            var pattern = @"\L\L\L\L\L\L-\L\L-\L\L\L\L\L";
            var config = new GenerationConfig(){Seed = "300"};
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern, config: config);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(15, text.Length);
            Assert.Matches(text,"LVMPQS-IY-CXIRW");
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_AlphaNumeric_multiple()
        {

            var pattern = @"\L\L\L\L\L\L-\L\L-\L\L\L\L\L";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(15, text.Length);
            Assert.Matches(new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"), text);

            pattern = @"\l\l\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(6, text.Length);
            Assert.Matches(new Regex("[a-z]*"), text);

            pattern = @"\d\d\d\d\d\d\L\L\L\L\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(14, text.Length);
            Assert.Matches(new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"), text);

            pattern = @"\d\d\d-\d\d-\d\d\d\d";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(11, text.Length);
            Assert.Matches(new Regex(@"[\d]{3}-[\d]{2}-[\d]{4}"), text);

            //Test for escaped characters.
            pattern = @"L\LLLLL";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(6, text.Length);
            Assert.Matches(new Regex("L[A-Z]{1}LLLL"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_AlphaNumeric()
        {

            var pattern = @"\L\l\V\v\C\c\d\d";
            string text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"[A-Z]{1}[a-z]{1}[AEIOU]{1}[aeiou]{1}[QWRTYPSDFGHJKLZXCVBNM]{1}[qwrtypsdfghjklzxcvbnm]{1}[\d]{1}[\d]{1}"), text);

        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Simple_Patterns()
        {
            var pattern = @"\L\L\L\L\L\L-\L\L-\L\L\L\L\L";
            string text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(15, text.Length);
            Assert.Matches(new Regex("[A-Z]{6}-[A-Z]{2}-[A-Z]{5}"), text);

            pattern = @"\l\l\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(6, text.Length);
            Assert.Matches(new Regex("[a-z]*"), text);

            pattern = @"\d\d\d\d\d\d\L\L\L\L\l\l\l\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(14, text.Length);
            Assert.Matches(new Regex("[0-9]{6}[A-Z]{4}[a-z]{4}"), text);

            pattern = @"[1-9]\d\d-\d\d-\d\d\d\d";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(11, text.Length);
            Assert.Matches(new Regex(@"[1-9]\d{2}-\d{2}-\d{4}"), text);

            //Test for escaped characters.
            pattern = @"L\LLLLL";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(6, text.Length);
            Assert.Matches(new Regex("L[A-Z]{1}LLLL"), text);

        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Repeat_Character()
        {
            var pattern = @"w{3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"w{3}"), text);

        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Repeat_Character_Inside_Group()
        {
            var pattern = @"(\dC{3}\d{3}){3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^(\dC{3}\d{3}){3}$"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Repeat_Character_Inside_Group2()
        {
            var pattern = @"(\d(C){3}){3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^(\d(C){3}){3}$"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Repeat_Character_Inside_Group3()
        {
            var pattern = @"(\d(C){3})";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^(\d(C){3})$"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Repeat_Character_Inside_Group4()
        {
            var pattern = @"(\d(\\)\d)";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^(\d(\\)\d$)"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Repeat_Symbol_Inside_Group()
        {
            var pattern = @"(\w{3})";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"\w{3}"), text);

        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Repeat_Symbol_Inside_Group2()
        {
            var pattern = @"(\w(\d{2}|\v{2})\w{3})";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"\w(\d{2}|[aeiou]{2})\w{3}"), text);

        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Repeat_Pattern()
        {
            var pattern = @"(\L\L\d\d){3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}"), text);

        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Repeat_Symbol()
        {
            var pattern = @"\L{3}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"[A-Z]{3}"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Mixed_Pattern_With_Random_Length()
        {
            string pattern = @"\L{10,20}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"[A-Z]{10,20}"), text);

        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Output_NonEscaped_Symbols()
        {
            string pattern = @"X";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"X"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Output_Repeated_Symbols()
        {
            string pattern = @"\d{10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"[\d]{10}"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Output_Escaped_Slash()
        {
            string pattern = @"[\\]{1,10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"[\\]{1,10}"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Mixed_Pattern()
        {
            string pattern = @"\L\L(\L\L\d\d){3}\L\L(\L){23}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"[A-Z]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[0-9]{2}[A-Z]{2}[A-Z]{23}"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Repeat_Pattern_Long()
        {
            var pattern = @"(\L){1}(\d){1}(\L){2}(\d){2}(\L){4}(\d){4}(\L){8}(\d){8}(\L){16}(\d){16}(\L){32}(\d){32}(\L){64}(\d){64}(\L){128}(\d){128}(\L){256}(\d){256}(\L){512}(\d){512}(\L){1024}(\d){1024}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"[A-Z]{1}[0-9]{1}[A-Z]{2}[0-9]{2}[A-Z]{4}[0-9]{4}[A-Z]{8}[0-9]{8}[A-Z]{16}[0-9]{16}[A-Z]{32}[0-9]{32}[A-Z]{64}[0-9]{64}[A-Z]{128}[0-9]{128}[A-Z]{256}[0-9]{256}[A-Z]{512}[0-9]{512}[A-Z]{1024}[0-9]{1024}")
                , text);
            Assert.Equal((1+2+4+8+16+32+64+128+256+512+1024)*2, text.Length);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Expressions_With_Alternates()
        {
            var pattern = @"(\L\L|\d\d)";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches( new Regex(@"^([A-Z]{2}|[0-9]{2})$"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Expressions_With_Alternates2()
        {
            var pattern = @"(\L\L|\d\d|[AEIOU]|[100-120])";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^([A-Z]{2}|[0-9]{2}|[AEIOU]|\d\d\d)$"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Zero_Repeats_Invalid_End()
        {
            var pattern = @"(\L\L\d\d)33}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^[A-Z]{2}[0-9]{2}33}$"), text);
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Zero_Repeats()
        {
            var pattern = @"\L\L\d\d";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^[A-Z]{2}[0-9]{2}$"), text);
        }


        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Complex1()
        {
            var pattern = @"\{'\w':('\w{3,25}'|[1-100])\}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^\{'\w':('\w{3,25}'|\d{1,3})\}$"), text);
        }


        #endregion

        #region Symbols

        [Fact]
        [Trait("Category", "Symbols")]
        public void Can_Generate_All_Symbols()
        {
            var pattern = @"\.";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^.$"), text);

            pattern = @"\a";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^[A-Za-z]$"), text);

            pattern = @"\W";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^\W$"), text);

            pattern = @"\w";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^\w$"), text);

            pattern = @"\L";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^[A-Z]$"), text);

            pattern = @"\l";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^[a-z]$"), text);

            pattern = @"\V";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^[AEIOU]$"), text);

            pattern = @"\v";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^[aeiou]$"), text);

            pattern = @"\C";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^[BCDFGHJKLMNPQRSTVWXYZ]$"), text);

            pattern = @"\c";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^[bcdfghjklmnpqrstvwxyz]$"), text);

            pattern = @"\D";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^\D$"), text);

            pattern = @"\d";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^\d$"), text);

            pattern = @"\s";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^\s$", RegexOptions.ECMAScript), text);  // ECMA compliant needed as \s ECMA includes [SPACE] but .NET Regex does not.

            pattern = @"\t";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^\t$"), text);

            pattern = @"\n";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^\n$"), text);

            pattern = @"\r";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^\r$"), text);

            pattern = @"\\";
            text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Matches(new Regex(@"^\\$"), text);
        }

        #endregion

        #region NamedPatterns

        [Fact]
        [Trait("Category", "NamedPatterns")]
        public void Can_Generate_NamedPatterns()
        {
            //var namedPatterns = FileReader.LoadNamedPatterns("default.tdg-patterns");

            var pattern = @"<<(@name_firstname_male@)>>";
            var text = AlphaNumericGenerator.GenerateFromTemplate(pattern, new GenerationConfig(){LoadDefaultPatternFile = true});
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.True(text.Length>0);

        }

        [Fact]
        [Trait("Category", "NamedPatterns")]
        public void Can_Generate_NamedPatterns_CompoundPattern()
        {
            //var namedPatterns = FileReader.LoadNamedPatterns("default.tdg-patterns");

            var pattern = @"<<(@address_us_type1@)>>";
            var text = AlphaNumericGenerator.GenerateFromTemplate(pattern, new GenerationConfig(){LoadDefaultPatternFile = true});
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.True(text.Length>0);

        }

        [Fact]
        [Trait("Category", "NamedPatterns")]
        public void Can_Generate_NamedPatterns_All_Defaults_Name()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var namedPatterns = FileReader.LoadNamedPatterns(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg-patterns", "default.tdg-patterns"));

            foreach (var dic in namedPatterns.Patterns)
            {
                var text = AlphaNumericGenerator.GenerateFromPattern("@"+dic.Name+"@", config: new GenerationConfig(){LoadDefaultPatternFile = true});
                Console.WriteLine(@"'{0}' produced '{1}'", dic.Name, text);
                Assert.True(text.Length > 0);
            }
            sw.Stop();
            Console.WriteLine(@"All {0} default patterns generated in {1} milliseconds."
                                            , namedPatterns.Patterns.Count
                                            , sw.ElapsedMilliseconds);
        }

        [Fact]
        [Trait("Category", "NamedPatterns")]

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

            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern("@blah@", config:config));
        }

        [Fact]
        [Trait("Category", "NamedPatterns")]
        public void Can_Generate_NamedPatterns_All_Defaults_Patterns()
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            var namedPatterns = FileReader.LoadNamedPatterns(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg-patterns", "default.tdg-patterns"));

            foreach (var dic in namedPatterns.Patterns)
            {
                var text = AlphaNumericGenerator.GenerateFromPattern(dic.Pattern, config:new GenerationConfig() { LoadDefaultPatternFile = true });
                Console.WriteLine(@"'{0}' produced '{1}'", dic.Name, text);
                Assert.True(text.Length > 0);
            }
            sw.Stop();
            Console.WriteLine(@"All {0} default patterns generated in {1} milliseconds."
                                            , namedPatterns.Patterns.Count
                                            , sw.ElapsedMilliseconds);
        }


        #endregion

        #region Profiling

        [Fact]
        [Trait("Category", "Profiling")]
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

        [Fact]
        [Trait("Category", "Profiling")]
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

        [Fact]
        [Trait("Category", "Profiling")]
        public void Profile_NonRandom_Repeat_LowVolume()
        {
            var pattern = @"<<\L{50}>>";
            var testLimit = 2;
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

        [Fact]
        [Trait("Category", "Profiling")]
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

        [Fact]
        [Trait("Category", "Profiling")]
        public void Profile_Large_NonRandom_Repeat_LowVolume()
        {
            var pattern = @"<<\L{1}\d{1}\L{2}\d{2}\L{4}\d{4}\L{8}\d{8}\L{16}\d{16}\L{32}\d{32}\L{64}\d{64}\L{128}\d{128}\L{256}\d{256}\L{512}\d{512}\L{1024}\d{1024}>>";
            var testLimit = 1;
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

        [Fact]
        [Trait("Category", "Profiling")]
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

        [Fact]
        [Trait("Category", "Profiling")]
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

        [Fact]
        [Trait("Category", "Profiling")]
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


        [Fact]
        [Trait("Category", "Profiling")]
        public void Profile_Comparison_With_String_Append()
        {
            var testIterations = 100000;
            var pattern = @"(A){100}";
            var patternOutput = "A";
            var patternCount = 100;

            // Control
            var sb = new StringBuilder();
            var sw = new System.Diagnostics.Stopwatch();
            sw.Reset();
            sw.Start();
            for (var i = 0; i < testIterations; i++)
            {
                for (var j = 0; j < patternCount; j++)
                {
                    sb.Append(patternOutput);
                }
            }
            sw.Stop();
            Console.WriteLine(@"StringBuilder - {0} iterations of the following pattern {1} completed in {2} milliseconds.", testIterations, pattern, sw.ElapsedMilliseconds);

            sw.Reset();
            sw.Start();
            for (var i = 0; i < testIterations; i++)
            {
                AlphaNumericGenerator.GenerateFromPattern(pattern);
            }
            sw.Stop();
            Console.WriteLine(@"GenerateFromPattern(pattern) - {0} iterations of the following pattern {1} completed in {2} milliseconds.", testIterations, pattern, sw.ElapsedMilliseconds);


            var config = new GenerationConfig() {Seed = "100"};
            sw.Reset();
            sw.Start();
            for (var i = 0; i < testIterations; i++)
            {
                AlphaNumericGenerator.GenerateFromPattern(pattern, config);
            }
            sw.Stop();
            Console.WriteLine(@"GenerateFromPattern(pattern, config) - {0} iterations of the following pattern {1} completed in {2} milliseconds.", testIterations, pattern, sw.ElapsedMilliseconds);

        }

        #endregion

        #region Negation


        [Fact]
        [Trait("Category", "Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set()
        {
            var pattern = @"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var text = AlphaNumericGenerator.GenerateFromPattern("[^"+pattern+"]");
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.True(pattern.IndexOf(text, StringComparison.InvariantCulture) == -1);
        }

        [Fact]
        [Trait("Category", "Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set_Range()
        {
            var pattern = @"A-Z";
            var text = AlphaNumericGenerator.GenerateFromPattern("[^" + pattern + "]");
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.True(pattern.IndexOf(text, StringComparison.InvariantCulture) == -1);
        }

        [Fact]
        [Trait("Category", "Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set_Range2()
        {
            var pattern = @"3-6";
            var text = AlphaNumericGenerator.GenerateFromPattern("[^" + pattern + "]");
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.True("3456".IndexOf(text, StringComparison.InvariantCultureIgnoreCase) == -1);
        }

        [Fact]
        [Trait("Category", "Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set_Range_Multiple()
        {
            var pattern = @"A-Za-z";
            var text = AlphaNumericGenerator.GenerateFromPattern("[^" + pattern + "]");
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.True("ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(text, StringComparison.InvariantCultureIgnoreCase) == -1);
        }

        [Fact]
        [Trait("Category", "Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set_Range_Repeated()
        {
            var pattern = @"[^3-6]{10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.True("3456".IndexOf(text, StringComparison.InvariantCultureIgnoreCase) == -1);
        }

        [Fact]
        [Trait("Category", "Negation")]
        public void Can_Generate_Correct_Output_from_Negated_Set_Range_Multiple_Repeated()
        {
            var pattern = @"[^A-Za-z]{10}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.True("ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(text, StringComparison.InvariantCultureIgnoreCase) == -1);
        }

        #endregion

        #region NegativeTesting

        [Fact]
        [Trait("Category", "NegativeTesting")]
        public void Can_Throw_Exception_Missing_Placeholder_End()
        {
            var template = "This is a very basic <<(l){10}>> which can be used to create <<llll of varying <<lllll>>. The main purpose is to generate dummy <<Llll>> which can be used for <<lllllll>>.";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromTemplate(template));

        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Missing_Placeholder_End_End()
        {
            var template = "This is a very basic <<(l){10}>> which can be used to create <<llll>> of varying <<lllll>>. The main purpose is to generate dummy <<Llll>> which can be used for <<lllllll.";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromTemplate(template));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_IncompletePattern()
        {
            var pattern = "(LLXX";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));

        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_InvalidCardinality_Start()
        {
            var pattern = "(LLXX){33";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Invalid_Cardinaliy_Less_Than_Zero()
        {
            var pattern = "(LLXX){-1}";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_InvalidPattern_Null()
        {
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(null));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Mixed_Pattern_With_Invalid_Random_Length_Character()
        {
            string pattern = "(L){10,}";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Mixed_Pattern_With_Invalid_Random_Length_Min_Max()
        {
            string pattern = "(L){10,0}";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Invalid_Repeat_Pattern()
        {
            var pattern = "(LLXX){w}";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Unknown_NamedPatterns()
        {
            var pattern = @"<<(@blahblahblah21@)>>";
             Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromTemplate(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Negated_Set_Range_InvalidNumeric()
        {
            var pattern = @"[^30-60]";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Negated_Set_Range_InvalidNumeric2()
        {
            var pattern = @"[^3-60]";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Negated_Set_Range_InvalidNumeric3()
        {
            var pattern = @"[^3.00-6]";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Invalid_Config()
        {
            var pattern = @"<#{ asdsd }#>";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]

        public void Can_Throw_Exception_Invalid_Config2()
        {
            var pattern = @"<#";
            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromPattern(pattern));
        }

        [Fact]
        [Trait("Category", "NegativeTesting")]
        public void Can_Throw_Missing_Pattern_File()
        {
            var template = @"<<@superhero@>>";

            var namedPatterns = new NamedPatterns();
            namedPatterns.Patterns.Add(new NamedPattern() { Name = "superhero", Pattern = "(Batman|Superman|Spiderman)" });

            var config = new GenerationConfig() { Seed = "100" };
            config.PatternFiles.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg-patterns", "notpresent.tdg-pattern"));
            config.NamedPatterns = namedPatterns;

            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromTemplate(template, config));
        }


        [Fact]
        [Trait("Category", "NegativeTesting")]
        public void Can_Throw_Invalid_Pattern_File()
        {
            var template = @"<<@superhero@>>";

            var namedPatterns = new TestDataGenerator.Core.NamedPatterns();
            namedPatterns.Patterns.Add(new TestDataGenerator.Core.NamedPattern() { Name = "superhero", Pattern = "(Batman|Superman|Spiderman)" });

            var config = new TestDataGenerator.Core.GenerationConfig() { Seed = "100" };
            config.PatternFiles.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tdg.exe"));
            config.NamedPatterns = namedPatterns;

            Assert.Throws<GenerationException>(() => AlphaNumericGenerator.GenerateFromTemplate(template, config));
        }

        #endregion

        #region ConfigurationTesting
        /*
        [Fact]
        [Trait("Category", "ConfigurationTesting")]
        public void Can_Configure_Random_Seed_From_Config()
        {
            int ndx = 0;

            var configStr = "<# { 'Seed':100 } #>";
            var template = configStr+@"Generated <<L\L>>";
            var config = AlphaNumericGenerator.GetConfiguration(template, ref ndx);
            Assert.Equal("100", config.Seed);
            Assert.Equal(configStr.Length, ndx);
        }
        */

        [Fact]
        [Trait("Category", "ConfigurationTesting")]
        public void Can_Configure_And_Produce_Output_With_Seed()
        {
            var template = "<# { \"Seed\":\"100\" } #>Generated <<L\\L>>";
            string text = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, text);

            Assert.Equal(text, "Generated LZ");
        }

        [Fact]
        [Trait("Category", "ConfigurationTesting")]
        public void Can_Configure_And_Produce_Output_With_Seed2()
        {
            var template = @"<# { 'Seed':100 } #>Generated <<\.\w\W\L\l\V\v\d\D\S\s>>";
            string actual = AlphaNumericGenerator.GenerateFromTemplate(template);
            Console.WriteLine(@"'{0}' produced '{1}'", template, actual);

            Assert.Equal(@"Generated |k]XjUo6Go", actual);
        }

        /*
        [Fact]
        [Trait("Category", "ConfigurationTesting")]
        public void Can_Catch_Config_Not_At_Beginning_Template()
        {
            var template = @"<# { 'Seed':100 } #>Generated <<\.\w\W\L\l\V\v\d\D\S\s>>";
            int index = 1;
            var config = AlphaNumericGenerator.GetConfiguration(template, ref index);
            Assert.IsNull(config);
        }
        */

        #endregion

        #region UtilityTesting

        [Fact]
        [Trait("Category", "UtilityTesting")]
        public void Can_Deserialize()
        {
            var config = Utility.DeserializeJson<GenerationConfig>("{\"seed\":\"100\"}");
            Assert.NotNull(config);
            Assert.Equal("100", config.Seed);
        }

        [Fact]
        [Trait("Category", "UtilityTesting")]
        public void Can_Serialize()
        {
            var config = new GenerationConfig();
            config.Seed = "300";
            var configStr = Utility.SerializeJson(config);
            Console.WriteLine("SerializeJson produced" + configStr);
            Assert.NotNull(configStr);
            Assert.Equal("{\"LoadDefaultPatternFile\":false,\"NamedPatterns\":{\"Patterns\":[]},\"patternfiles\":[],\"seed\":\"300\"}", configStr);
        }

        #endregion

        #region "special functions"

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Anagram()
        {
            var pattern = @"[ABC]{:anagram:}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(3, text.Length);
            Assert.True(text.Contains("A"));
            Assert.True(text.Contains("B"));
            Assert.True(text.Contains("C"));
        }

        [Fact]
        [Trait("Category", "Pattern")]
        public void Can_Generate_Anagram_Long()
        {
            var input = "abcdefghijklmnopqrstuvwxyz";
            var pattern = @"["+input+"]{:anagram:}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern);
            Console.WriteLine(@"'{0}' produced '{1}'", pattern, text);
            Assert.Equal(input.Length, text.Length);
            foreach (var ch in input.ToCharArray())
            {
                Assert.True(text.Contains(ch.ToString()));
            }
        }

        #endregion

        #region Randomness
        [Fact]
        [Trait("Category", "Randomness")]
        public void Level_Of_Randomness()
        {
            var pattern = @"(\L\L\L\L\L\L-\L\L-\L\L\L\L\L\n){1000}";
            var text = AlphaNumericGenerator.GenerateFromPattern(pattern, config:new GenerationConfig() { Seed = "100" });
            var segments = new List<string>(text.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));

            Console.WriteLine(@"'{0}' produced {1} values, out of which, {2} are unique and {3} are duplicates.", pattern, segments.Count, segments.Distinct().Count(), segments.Count - segments.Distinct().Count());
        }

        [Fact]
        [Trait("Category", "Randomness")]
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

        [Fact]
        [Trait("Category", "Randomness")]
        public void Seed_As_String_IsUsed()
        {
            var pattern = @"(\L\L\L\L\L\L-\L\L-\L\L\L\L\L)";
            var config = new GenerationConfig();

            var segments = new Dictionary<string, string>();
            for (var i = 0; i < 1000; i++)
            {
                segments.Add(AlphaNumericGenerator.GenerateFromPattern(pattern, config: config), "");
                config.Seed += "a"; // reset the seed each loop which should result in new random generations.
            }
            Console.WriteLine(@"'{0}' produced {1} values, out of which, {2} are unique and {3} are duplicates.", pattern, segments.Count, segments.Distinct().Count(), segments.Count - segments.Distinct().Count());
        }

        [Fact]
        [Trait("Category", "Randomness")]public void Weird_s_shortkey_bug()
        {
            var pattern = @"\s";
            var config = new GenerationConfig();
            for (var i = 0; i < 1000; i++)
            {
                config.Seed = i.ToString();
                var text = AlphaNumericGenerator.GenerateFromPattern(pattern, config);
                Console.WriteLine(@"'{0}' with seed '{1}' produced '{2}'", pattern, config.Seed, text);
                Assert.Matches(new Regex(@"^\s$", RegexOptions.ECMAScript), text);  // ECMA compliant needed as \s ECMA includes [SPACE] but .NET Regex does not.
                config.Seed += "a"; // reset the seed each loop which should result in new random generations.
            }

        }
    #endregion
}
}

