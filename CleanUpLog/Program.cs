using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CleanUpLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var utils = new Utils();

            var inputFileNameBefore = @"C:\TEMP\before.txt";
            var inputFileNameAfter = @"C:\TEMP\after.txt";
            var outputFileName = @"C:\TEMP\FILE2.csv";

            ConfigureArgs(args, ref inputFileNameBefore, ref inputFileNameBefore, ref outputFileName);
                                            
            List<string> readFileBefore = File.ReadAllLines(inputFileNameBefore).ToList();
            List<string> readFileAfter = File.ReadAllLines(inputFileNameAfter).ToList();

            var beforeRows = GetRowsFromFile(readFileBefore, utils);
            var afterRows = GetRowsFromFile(readFileAfter, utils);
           
           var generatedStats = utils.GenerateStats(beforeRows, afterRows);
           
            GenerateOutputStats.OutputCSV(generatedStats, outputFileName);
       }

        private static List<Row> GetRowsFromFile(List<string> readFile, Utils utils)
        {
            var rows = new List<Row>();

            foreach (string line in readFile)
            {
                if (line.Contains("cache")) continue;
                if (line.Length == 0) continue;                

                var items = line.Split(' ');
                var time = items[1];
                var url = items[5].TrimEnd(',');
                url = utils.RemoveParameters(url);
                var timeTaken = items[6];
                if (time.Length > 1 && time.Contains(":"))
                {
                    if (!utils.Exclude(url))
                    {
                        var row = BuildDomainRowFromFile(utils, time, url, timeTaken);
                        rows.Add(row);
                    }
                }
            }
            return rows;
        }

        private static Row BuildDomainRowFromFile(Utils utils, string time, string url, string timeTaken)
        {           
            var row = new Row
            {
                DateImported = DateTime.Now,
                Time = Convert.ToDateTime(time),
                URL = url,
                IsComparable = utils.IsComparable(url),
            };

            if (Double.TryParse(timeTaken, out var number))
            {
                row.TimeTaken = number;
            }
            return row;
        }

        private static void ConfigureArgs(string[] args, ref string inputFileNameBefore, ref string inputFileNameAfter, ref string outputFileName)
        {
            if (args.Length > 0)
            {
                inputFileNameBefore = args[0];
                inputFileNameAfter = args[1];
                outputFileName = args[2];

            }
        }
    }
}
