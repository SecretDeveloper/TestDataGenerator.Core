using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommandLine;
using Eloquent;
using gk.DataGenerator.Generators;

namespace gk.DataGenerator.tdg
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var cla = new CommandLineArgs();
            Stopwatch sw = null;

#if DEBUG
            Debugger.Launch();
#endif
            try
            {
                var result = cla.Parse(args);
                if (result.Any())
                {
                    Console.WriteLine("Parse failed!  Use --help flag for instructions on usage.");
                    result.ForEach(Console.WriteLine);
                    return;
                }

                if (cla.ShowHelp)
                {
                    Console.WriteLine(cla.GetUsage());
                    return;
                }

                
                if (cla.Verbose)
                {
                    sw = new Stopwatch();
                    sw.Start();
                }

                var template = GetTemplateValue(cla);

                if (!template.IsNullOrEmpty() && !cla.OutputFilePath.IsNullOrEmpty()) // output path provided.
                {
                    OutputToFile(cla, template);
                }
                else if(!template.IsNullOrEmpty())
                {
                    OutputToConsole(cla, template);
                }
                else
                {
                    Console.WriteLine(cla.GetUsage());
                }

                if (cla.Verbose)
                {
                    sw.Stop();
                    Console.WriteLine("Generation took {0} milliseconds", sw.ElapsedMilliseconds);
                }
            }
            catch (GenerationException gex)
            {
                Console.WriteLine(string.Format("Error:\n{0}", gex.Message));
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("Error:{0}\n\nStackTrace:{1}",ex.Message, ex.StackTrace));
            }
        }

        private static string GetTemplateValue(CommandLineArgs cla)
        {
            string template="";
            if (!cla.Template.IsNullOrEmpty()) // template provided -- no header skipping required
            {
                template = cla.Template;
                if (cla.Verbose) Console.WriteLine("Provided template was '" + template + "'");
            }
            if (!cla.Pattern.IsNullOrEmpty()) // template provided -- no header skipping required
            {
                template = cla.Pattern;
                if (cla.Verbose) Console.WriteLine("Provided pattern was '" + template + "'");
            }
            if (!cla.InputFilePath.IsNullOrEmpty()) // input file provided
            {
                template = File.ReadAllText(cla.InputFilePath);
                if (cla.Verbose) Console.WriteLine("Provided template was '" + template + "'");
            }
            return template;
        }

        private static void OutputToConsole(CommandLineArgs cla, string template)
        {
            Func<string, string> generateFrom = AlphaNumericGenerator.GenerateFromTemplate;
            if (!cla.Pattern.IsNullOrEmpty())
            {
                generateFrom = AlphaNumericGenerator.GenerateFromPattern;
            }

            int ct = 0;
            while (ct < cla.Count)
            {
                var item = generateFrom(template);
                Console.WriteLine(item);
                ct++;
            }
        }

        private static void OutputToFile(CommandLineArgs cla, string template)
        {
            using (var fs = new StreamWriter(cla.OutputFilePath))
            {
                int ct = 0;
                while (ct < cla.Count)
                {
                    var item = AlphaNumericGenerator.GenerateFromTemplate(template);
                    fs.WriteLine(item);
                    ct++;
                }
            }
        }

    }
}
