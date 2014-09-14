using System;
using System.Reflection;
using CommandLine;
using CommandLine.Text;

namespace gk.DataGenerator.tdg
{
    internal class CommandLineArgs
    {
        [Option('t', "template", DefaultValue = "", HelpText = "The template containing 1 or more patterns to use when producing data.", MutuallyExclusiveSet = "fromCL")]
        public string Template { get; set; }

        [Option('p', "pattern", DefaultValue = "", HelpText = "The pattern to use when producing data.", MutuallyExclusiveSet = "fromCL")]
        public string Pattern { get; set; }

        [Option('i', "inputfile", DefaultValue = "", HelpText = "The path of the input file.", Required = false, MutuallyExclusiveSet = "fromFile")]
        public string InputFilePath { get; set; }

        //[Option('headers', "headers", DefaultValue = 0, HelpText = "The line to start reading from, 0 for start of file. Can be used to skip header rows in files.", Required = false, MutuallyExclusiveSet = "fromFile")]
        public int HeaderCount { get; set; }
        
        [Option('o', "output", DefaultValue = "", HelpText = "The path of the output file.", Required = false)]
        public string OutputFilePath { get; set; }

        [Option('c', "count", DefaultValue = 1, HelpText = "The number of items to produce.", Required = false)]
        public int Count { get; set; }

        [Option('s', "seed", HelpText = "The seed value for random generation. Default is a random value.", Required = false)]
        public int? Seed { get; set; }
        
        [Option('v', "verbose", DefaultValue = false, HelpText = "Verbose output including debug and performance information.", Required = false)]
        public bool Verbose { get; set; }

        [Option('n', "namedpatterns", DefaultValue = "", HelpText = "A list of ';' seperated file paths containing named patterns to be used in addition to default.tdg-patterns.", Required = false)]
        public string NamedPatterns { get; set; }

        [Option('l', "listnamedpatterns", DefaultValue = false, HelpText = "Outputs a list of the named patterns from the default.tdg-patterns file.", Required = false)]
        public bool ListNamedPatterns { get; set; }


        public CommandLineArgs()
        {
            //Random random = new Random(DateTime.Now.Millisecond);
            //this.Seed = random.Next(int.MinValue, int.MaxValue);
        }

        [HelpOption]
        public string GetUsage()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            var help = new HelpText
            {
                Heading = new HeadingInfo("Test Data Generator", assemblyName.Version.ToString()),
                Copyright = new CopyrightInfo("Gary Kenneally (@SecretDeveloper)", 2014),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of the MIT License.");
            help.AddPreOptionsLine(" <http://www.opensource.org/licenses/mit-license.php>");

            help.AddPostOptionsLine("Examples:");
            help.AddPostOptionsLine("\t tdg -t '<<LL>>'");
            help.AddPostOptionsLine("\t tdg -t '<<LL>>' -c 10");
            help.AddPostOptionsLine("\t tdg -t '<<LL>>' -o 'c:\\test.txt' -c 10");
            help.AddPostOptionsLine("\t tdg -i 'c:\\input.txt' -o 'c:\\test.txt' -c 10");
            help.AddPostOptionsLine("Either a Template (-t), Pattern (-p) or input File (-i) value must be provided as input.");
            
            help.AddOptions(this);
            return help;
        }

    }
}

       