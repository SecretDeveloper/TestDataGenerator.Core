using System;
using System.IO;
using CommandLine;
using gk.DataGenerator.Generators;

namespace gk.DataGenerator.tdg
{
    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var cla = new CommandLineArgs();

                if (!CommandLine.Parser.Default.ParseArguments(args, cla))
                {
                    Console.WriteLine("Parse failed");
                    return;
                }

                if (cla.Template != "") // template provided -- no header skipping required
                {
                    if (cla.OutputPath != "") // output path provided.
                    {
                        using (var fs = new StreamWriter(cla.OutputPath))
                        {
                            int ct = 0;
                            while (ct < cla.Count)
                            {
                                var item = AlphaNumericGenerator.GenerateFromTemplate(cla.Template);
                                fs.WriteLine(item);
                                ct++;
                            }
                        }
                    }
                    else
                    {
                        int ct = 0;
                        while (ct < cla.Count)
                        {
                            var item = AlphaNumericGenerator.GenerateFromTemplate(cla.Template);
                            Console.WriteLine(item);
                            ct++;
                        }
                    }
                }
                else if (cla.FilePath != "") // input file provided
                {
                    var template = File.ReadAllText(cla.FilePath);

                    if (cla.OutputPath != "") // output path provided.
                    {
                        using (var fs = new StreamWriter(cla.OutputPath))
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
                    else
                    {
                        int ct = 0;
                        while (ct < cla.Count)
                        {
                            var item = AlphaNumericGenerator.GenerateFromTemplate(template);
                            Console.WriteLine(item);
                            ct++;
                        }
                    }
                }
                else
                {
                    Console.WriteLine(cla.GetUsage());
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
