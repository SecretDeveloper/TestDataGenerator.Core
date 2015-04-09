using System;
using System.Reflection;
using System.Security.Policy;
using System.Text;
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

        [Option('d', "detailed", DefaultValue = false, HelpText = "Show help text for pattern symbols")]
        public bool ShowPatternHelp { get; set; }

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

        [HelpOption]
        public string GetUsage()
        {
            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            var help = new HelpText
            {
                Heading = new HeadingInfo("Test Data Generator", assemblyName.Version.ToString()),
                Copyright = new CopyrightInfo("Gary Kenneally (@SecretDeveloper)", 2015),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };
            help.AddPreOptionsLine("This is free software. You may redistribute copies of it under the terms of the MIT License.");
            help.AddPreOptionsLine(" <http://www.opensource.org/licenses/mit-license.php>");
            help.AddPreOptionsLine("Commandline Arguments:");

            help.AddPostOptionsLine("Examples:");
            help.AddPostOptionsLine("\t tdg -t '<<\\L\\L>>'");
            help.AddPostOptionsLine("\t tdg -t '<<\\L\\L>>' -c 10");
            help.AddPostOptionsLine("\t tdg -t '<<\\L\\L>>' -o 'c:\\test.txt' -c 10");
            help.AddPostOptionsLine("\t tdg -i 'c:\\input.txt' -o 'c:\\test.txt' -c 10");
            help.AddPostOptionsLine("Either a template (-t), pattern (-p) or input file (-i) value must be provided as input.");
            
            help.AddOptions(this);
            return help;
        }

        public string GetPatternUsage()
        {
            var sb = new StringBuilder();

            sb.AppendLine("Detailed Pattern Usage");
            sb.AppendLine();

            sb.AppendLine("The following symbols can be used within a pattern to produce the desired output.");
            sb.AppendLine("\t\\. - A single random character of any type.");
            sb.AppendLine("\t\\a - A single random upper-case or lower-case letter.");
            sb.AppendLine("\t\\W - A single random character from the following list ' .,;:'\"!&?£€$%^<>{}*+-=\\@#|~/'.");
            sb.AppendLine("\t\\w - A single random upper-case, lower-case letter or number.");
            sb.AppendLine("\t\\L - A single random upper-case letter.");
            sb.AppendLine("\t\\l - A single random lower-case letter.");
            sb.AppendLine("\t\\V - A single random upper-case Vowel.");
            sb.AppendLine("\t\\v - A single random lower-case vowel.");
            sb.AppendLine("\t\\C - A single random upper-case consonant.");
            sb.AppendLine("\t\\c - A single random lower-case consonant.");
            sb.AppendLine("\t\\D - A single random non number character.");
            sb.AppendLine("\t\\d - A single random number, 1-9.");
            sb.AppendLine("\t\\S - A single random non-whitespace character.");
            sb.AppendLine("\t\\s - A whitespace character (Tab, New Line, Space, Carriage Return or Form Feed)");
            sb.AppendLine("\t\\n - A newline character.");
            sb.AppendLine("\t\\t - A tab character.");
            
            sb.AppendLine("Patterns usage");
            sb.AppendLine("\ttdg -p '(\\L)' - Will generate a random upper-case character.");
            sb.AppendLine("\ttdg -p '(\\L){5}' - Will generate 5 random upper-case characters.");
            sb.AppendLine("\ttdg -p '(\\L){10,20}' - Will generate between 10 and 20 random upper-case characters.");
            
            sb.AppendLine("Patterns and normal text can be combined in templates");
            sb.AppendLine("Template usage");
            sb.AppendLine("\ttdg -t 'Text containing a <<(\\L)>> pattern'");
            sb.AppendLine("\ttdg -t 'Text containing a <<(\\L){5}>> repeating pattern'");
            sb.AppendLine("\ttdg -t 'Text containing a <<(\\L){10,20}>> repeating pattern between 10 and 20 upper-case characters'");
            sb.AppendLine("View the Readme document for further examples");

            return sb.ToString();
        }

    }
}

       