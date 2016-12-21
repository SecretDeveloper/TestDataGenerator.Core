using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestDataGenerator.Core;
using TestDataGenerator.Core.Generators;

namespace TestDataGenerator.Tests
{
    [TestClass]
    public class BenchmarkInit
    {
        //[TestMethod]
        public void RunBenchmarks()
        {
            BenchmarkRunner.Run<TDGBenchmarks>(new ManualConfig() {});
        }
    }

    public class TDGBenchmarks
    {
        public GenerationConfig _GenerationConfig;

        [Setup]
        public void SetupData()
        {
            _GenerationConfig = new GenerationConfig();
        }

        #region Benchmarks

        [Benchmark]
        public string Benchmark_Template_L()
        {
            var pattern = @"<<\L>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

       
        [Benchmark]
        public string Benchmark_Template_L_Repeat_50()
        {
            var pattern = @"<<\L{50}>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

        [Benchmark]
        public string Benchmark_Template_L_Repeat_1_to_50()
        {
            var pattern = @"<<\L{1,50}>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

        [Benchmark]
        public string Benchmark_Template_Set_10_100()
        {
            var pattern = @"<<[10-100]>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

        [Benchmark]
        public string Benchmark_Template_Set_10_100_Repeat_100()
        {
            var pattern = @"<<[10-100]{100}>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

        [Benchmark]
        public string Benchmark_Template_Set_Decimal_10_100()
        {
            var pattern = @"<<[10.00-100]>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

        [Benchmark]
        public string Benchmark_Template_Set_Decimal_10_100_Repeat_100()
        {
            var pattern = @"<<[10.00-100]{100}>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

        [Benchmark]
        public string Benchmark_Template_MultipleCharacters_Repeat()
        {
            var pattern = @"<<\L{1}\d{1}\L{2}\d{2}\L{4}\d{4}\L{8}\d{8}\L{16}\d{16}\L{32}\d{32}\L{64}\d{64}\L{128}\d{128}\L{256}\d{256}\L{512}\d{512}\L{1024}\d{1024}>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern);
        }

        [Benchmark]
        public string Benchmark_Pattern_L()
        {
            var pattern = @"\L";
            return AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [Benchmark]
        public string Benchmark_Pattern_L_Repeat_50()
        {
            var pattern = @"\L{50}";
            return AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [Benchmark]
        public string Benchmark_Pattern_L_Repeat_1_to_50()
        {
            var pattern = @"\L{1,50}";
            return AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [Benchmark]
        public string Benchmark_Pattern_Set_10_100()
        {
            var pattern = @"[10-100]";
            return AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [Benchmark]
        public string Benchmark_Pattern_Set_10_100_Repeat_100()
        {
            var pattern = @"[10-100]{100}";
            return AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [Benchmark]
        public string Benchmark_Pattern_Set_Decimal_10_100()
        {
            var pattern = @"[10.00-100]";
            return AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [Benchmark]
        public string Benchmark_Pattern_Set_Decimal_10_100_Repeat_100()
        {
            var pattern = @"[10.00-100]{100}";
            return AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [Benchmark]
        public string Benchmark_Pattern_MultipleCharacters_Repeat()
        {
            var pattern = @"<<\L{1}\d{1}\L{2}\d{2}\L{4}\d{4}\L{8}\d{8}\L{16}\d{16}\L{32}\d{32}\L{64}\d{64}\L{128}\d{128}\L{256}\d{256}\L{512}\d{512}\L{1024}\d{1024}>>";
            return AlphaNumericGenerator.GenerateFromPattern(pattern);
        }

        [Benchmark]
        public string Benchmark_Template_L_With_Config()
        {
            var pattern = @"<<\L>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern, _GenerationConfig);
        }


        [Benchmark]
        public string Benchmark_Template_L_Repeat_50_With_Config()
        {
            var pattern = @"<<\L{50}>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Template_L_Repeat_1_to_50_With_Config()
        {
            var pattern = @"<<\L{1,50}>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Template_Set_10_100_With_Config()
        {
            var pattern = @"<<[10-100]>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Template_Set_10_100_Repeat_100_With_Config()
        {
            var pattern = @"<<[10-100]{100}>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Template_Set_Decimal_10_100_With_Config()
        {
            var pattern = @"<<[10.00-100]>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Template_Set_Decimal_10_100_Repeat_100_With_Config()
        {
            var pattern = @"<<[10.00-100]{100}>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Template_MultipleCharacters_Repeat_With_Config()
        {
            var pattern = @"<<\L{1}\d{1}\L{2}\d{2}\L{4}\d{4}\L{8}\d{8}\L{16}\d{16}\L{32}\d{32}\L{64}\d{64}\L{128}\d{128}\L{256}\d{256}\L{512}\d{512}\L{1024}\d{1024}>>";
            return AlphaNumericGenerator.GenerateFromTemplate(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Pattern_L_With_Config()
        {
            var pattern = @"\L";
            return AlphaNumericGenerator.GenerateFromPattern(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Pattern_L_Repeat_50_With_Config()
        {
            var pattern = @"\L{50}";
            return AlphaNumericGenerator.GenerateFromPattern(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Pattern_L_Repeat_1_to_50_With_Config()
        {
            var pattern = @"\L{1,50}";
            return AlphaNumericGenerator.GenerateFromPattern(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Pattern_Set_10_100_With_Config()
        {
            var pattern = @"[10-100]";
            return AlphaNumericGenerator.GenerateFromPattern(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Pattern_Set_10_100_Repeat_100_With_Config()
        {
            var pattern = @"[10-100]{100}";
            return AlphaNumericGenerator.GenerateFromPattern(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Pattern_Set_Decimal_10_100_With_Config()
        {
            var pattern = @"[10.00-100]";
            return AlphaNumericGenerator.GenerateFromPattern(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Pattern_Set_Decimal_10_100_Repeat_100_With_Config()
        {
            var pattern = @"[10.00-100]{100}";
            return AlphaNumericGenerator.GenerateFromPattern(pattern, _GenerationConfig);
        }

        [Benchmark]
        public string Benchmark_Pattern_MultipleCharacters_Repeat_With_Config()
        {
            var pattern = @"\L{1}\d{1}\L{2}\d{2}\L{4}\d{4}\L{8}\d{8}\L{16}\d{16}\L{32}\d{32}\L{64}\d{64}\L{128}\d{128}\L{256}\d{256}\L{512}\d{512}\L{1024}\d{1024}";
            return AlphaNumericGenerator.GenerateFromPattern(pattern, _GenerationConfig);
        }

        #endregion

    }
}
