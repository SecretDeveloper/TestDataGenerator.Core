using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eloquent;

namespace gk.DataGenerator.tdg
{
    internal class CommandLineArgs
    {
        public string Template { get; set; }
        
        public string Pattern { get; set; }

        public string InputFilePath { get; set; }

        public int NumberOfHeaderLines { get; set; }
        
        public string OutputFilePath { get; set; }

        public int Count { get; set; }
        
        public bool Verbose { get; set; }

        public string NamedPatternsFiles { get; set; }

        public bool ShowHelp { get; set; }

        public CommandLineArgs()
        {
            reset();
        }

        private void reset()
        {
            this.Template = string.Empty;
            this.Verbose = false;
            this.ShowHelp = false;
            this.Pattern = string.Empty;
            this.OutputFilePath = string.Empty;
            this.NumberOfHeaderLines = 0;
            this.NamedPatternsFiles = string.Empty;
            this.InputFilePath = string.Empty;
            this.Count = 1;
        }

        public string GetUsage()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Test Data Generator 3.3.0");
            sb.AppendLine("Copyright (C) 2014 Gary Kenneally (@SecretDeveloper)");
            sb.AppendLine("This is free software. You may redistribute copies of it under the terms of the MIT License.");
            sb.AppendLine("  http://www.opensource.org/licenses/mit-license.php");
            sb.AppendLine();
            sb.AppendLine("Parameters:");
            sb.AppendLine("  -t:  --template:       The template containing 1 or more patterns to use when producing data.");
            sb.AppendLine("  -p:  --pattern:        The pattern to use when producing data.");
            sb.AppendLine("  -i:  --inputfile:      The path of the input file.");
            sb.AppendLine("  -h:  --headers:        The number of header rows contained in the --file (i) provided. Can be used to skip header rows in files.");
            sb.AppendLine("  -o:  --output:         The path of the output file.");
            sb.AppendLine("  -c:  --count:          The number of items to produce.");
            sb.AppendLine("  -v:  --verbose:        Verbose output including debug and performance information.");
            //sb.AppendLine("  -n:  --namedpatterns:  A list of ';' separated file paths containing named patterns to be used in addition to default.tdg-patterns.");
            sb.AppendLine();
            sb.AppendLine("Examples:");
            sb.AppendLine("     tdg -p:'\\L\\L'");
            sb.AppendLine("     tdg -t:'<<\\L\\L>>'");
            sb.AppendLine("     tdg -t:'<<\\L\\L>>' -c:10");
            sb.AppendLine("     tdg -t:'<<\\L\\L>>' -o:'c:\\test.txt' -c:10");
            sb.AppendLine("     tdg -i:'c:\\input.txt' -o:'c:\\test.txt' -c:10");
            sb.AppendLine();
            sb.AppendLine("Either a Template (-t), Pattern (-p) or Input file (-i) value must be provided as input.");

            return sb.ToString();
        }

        public List<string> Parse(string[] args)
        {
            var result = new List<string>();
            try
            {
                reset();

                //populate
                var parsedArgs = args.Select(s => s.Split(':')).ToDictionary(s => s[0].ToLowerInvariant(), s => s.Length>1?s[1]:"");
                
                if (args.Length == 0 || parsedArgs.ContainsKey("--help"))
                {
                    this.ShowHelp = true;
                    return result;
                }

                if (parsedArgs.ContainsKey("-t"))
                    this.Template = parsedArgs["-t"];
                if (parsedArgs.ContainsKey("--template"))
                    this.Template = parsedArgs["--template"];

                if (parsedArgs.ContainsKey("-p"))
                    this.Pattern = parsedArgs["-p"];
                if (parsedArgs.ContainsKey("--pattern"))
                    this.Pattern = parsedArgs["--pattern"];

                if (parsedArgs.ContainsKey("-i"))
                    this.InputFilePath = parsedArgs["-i"];
                if (parsedArgs.ContainsKey("--inputfile"))
                    this.InputFilePath = parsedArgs["--inputfile"];

                if (parsedArgs.ContainsKey("-l"))
                    this.NumberOfHeaderLines = parsedArgs["-l"].ToInt32();
                if (parsedArgs.ContainsKey("--line"))
                    this.NumberOfHeaderLines = parsedArgs["--line"].ToInt32();

                if (parsedArgs.ContainsKey("-o"))
                    this.OutputFilePath = parsedArgs["-o"];
                if (parsedArgs.ContainsKey("--output"))
                    this.OutputFilePath = parsedArgs["--output"];

                if (parsedArgs.ContainsKey("-c"))
                    this.Count = parsedArgs["-c"].ToInt32();
                if (parsedArgs.ContainsKey("--count"))
                    this.Count = parsedArgs["--count"].ToInt32();

                if (parsedArgs.ContainsKey("-v"))
                    this.Verbose = true;
                if (parsedArgs.ContainsKey("--verbose"))
                    this.Verbose = true;

                if (parsedArgs.ContainsKey("-n"))
                    this.NamedPatternsFiles = parsedArgs["-n"];
                if (parsedArgs.ContainsKey("--namedpatterns"))
                    this.NamedPatternsFiles = parsedArgs["--namedpatterns"];

                //validate exclusions

                if (Template.IsNullOrWhiteSpace() && Pattern.IsNullOrWhiteSpace() && InputFilePath.IsNullOrWhiteSpace())
                {
                    result.Add("Either a Template (-t), Pattern (-p) or input File (-i) value must be provided as input.");
                }
            }
            catch (Exception ex)
            {
                result.Add("Error!");
                result.Add(ex.Message);
                result.Add(ex.StackTrace);
            }
            return result;
        }
    }
}
