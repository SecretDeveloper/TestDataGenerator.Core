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

        //[Option('l', "line", DefaultValue = 0, HelpText = "The line to start reading from, 0 for start of file. Can be used to skip header rows in files.", Required = false, MutuallyExclusiveSet = "fromFile")]
        public int LineToStartFrom { get; set; }
        
        [Option('o', "output", DefaultValue = "", HelpText = "The path of the output file.", Required = false)]
        public string OutputPath { get; set; }

        [Option('c', "count", DefaultValue = 1, HelpText = "The number of items to produce.", Required = false)]
        public int Count { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            var help = new HelpText
            {
                Heading = new HeadingInfo("Test Data Generator", "1.0"),
                Copyright = new CopyrightInfo("Gary Kenneally (@SecretDeveloper)", 2014),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of the MIT License <http://www.opensource.org/licenses/mit-license.php>.");

            help.AddPostOptionsLine("Examples:");
            help.AddPostOptionsLine("\t tdg -t '((LL))'");
            help.AddPostOptionsLine("\t tdg -t '((LL))' -c 10");
            help.AddPostOptionsLine("\t tdg -t '((LL))' -o 'c:\\test.txt' -c 10");
            help.AddPostOptionsLine("Either a Template (-t) or input File (-f) value must be provided as input.");
            
            help.AddOptions(this);
            return help;
        }
    }
}
