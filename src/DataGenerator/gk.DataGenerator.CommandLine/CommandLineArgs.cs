using System.Text;
using CommandLine;
using CommandLine.Text;

namespace gk.DataGenerator.tdg
{
    internal class CommandLineArgs
    {
        [Option('t', "template", DefaultValue = "", HelpText = "The template to use when producing data", MutuallyExclusiveSet = "fromCL")]
        public string Template { get; set; }

        [Option('f', "file", DefaultValue = "", HelpText = "The path of the input file.", Required = false, MutuallyExclusiveSet = "fromFile")]
        public string FilePath { get; set; }

        [Option('l', "line", DefaultValue = 0, HelpText = "The line to start reading from, 0 for start of file. Can be used to skip header rows in files.", Required = false, MutuallyExclusiveSet = "fromFile")]
        public int LineToStartFrom { get; set; }
        
        [Option('o', "output", DefaultValue = "", HelpText = "The path of the output file.", Required = false)]
        public string OutputPath { get; set; }

        [Option('c', "count", DefaultValue = 1, HelpText = "The number of items to produce.", Required = false)]
        public int Count { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            // this without using CommandLine.Text
            //  or using HelpText.AutoBuild
            var usage = new StringBuilder();
            usage.AppendLine("Test Data Generator");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }
    }
}
