using System.Collections.Generic;
using LINQtoCSV;

namespace CleanUpLog
{
    class GenerateOutputStats
    {
        public static void OutputCSV(IEnumerable<ResultLine> rows, string outputFileName)
        {
            CsvFileDescription outputFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };

            CsvContext cc = new CsvContext();

            cc.Write(
                rows,
                outputFileName,
                outputFileDescription
            );
        }
    }
}