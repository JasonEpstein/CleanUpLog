using System.Collections.Generic;
using System.Linq;
using LINQtoCSV;

namespace CleanUpLog
{
    public class Utils
    {
        public string RemoveParameters(string s)
        {
            if (s.Contains('?'))
            {
                s = s.Remove(s.IndexOf('?'));
                return s;
            }
            return s;
        }

        public bool Exclude(string s)
        {
            var listToExlude = new List<string> {"/sitecore/admin/"};

            foreach (var excluder in listToExlude)
                if (s.Contains(excluder))
                    return true;

            return false;
        }


        public bool IsComparable(string url)
        {
            var comparables = new List<string>
            {
                @"/mps/local-office/approval/proposals/summary",
                @"/mps/local-office/approval/contracts/summary",
                @"/mps/dealer/contracts/summary",
                @"/mps/dealer/contracts/summary",
                @"/mps/dealer/proposals/convert/summary",
                @"/mps/proposal/current/products-available",
                @"/mps//dealer/proposals/convert/customer-information",
                @"/mps//dealer/proposals/create/customer-information",
                @"/mps//dealer/proposals/create/summary",
                @"/mps//dealer/proposals/create/click-price",
                @"/mps//dealer/proposals/create/products",
                @"/mps//proposal/current/product/add-or-update",
                @"/mps//dealer/proposals/create/description",
                @"/mps//dealer/proposals/create/term-type",
                @"/mps//dealer/proposals/summary",
                @"/mps//dealer/proposals/convert/term-type",
                @"/mps//proposal/current/click-price",
                @"/mps/dealer/proposals/convert/products",
                @"/mps/proposal/current/product-configuration/1",
                @"/mps/dealer/proposals/convert/click-price",
                @"/mps/dealer/contracts/awaiting-acceptance",
                @"/mps/dealer/dashboard",
                @"/mps/dealer/contracts/rejected",
                @"/mps/dealer/proposals/in progress",
                @"/mps/dealer/contracts/approved-proposals",
                @"/mps/local-office/approval/proposals/awaiting-approval",
                @"/mps/dealer/proposals/awaiting-approval",
                @"/mps/proposal/current/product-selection-status",
                @"/mps/dealer/proposals/approved",
                @"/mps/local-office/approval/contracts/awaiting-acceptance",
                @"/mps/local-office/approval/contracts/rejected",
                @"/mps/local-office/approval/contracts/approved-proposals",
                @"/mps/local-office/approval/proposals/approved",
                @"/mps/local-office/approval/proposals",
                @"/mps/local-office/approval/dashboard",
                @"/mps/local-office/dashboard",
                @"/mps/dealer",
                @"/mps/js/datatables",
                @"/mps/dealer/create-proposal",
                @"/mps/dealer/contracts",
                @"/mps/local-office/approval",
                @"/mps/local-office/approval/contracts",
                @"/mps/dealer/proposals",
                @"/mps/handlebars/product-selection-status-no-items",
                @"/mps/local-office",
                @"/mps/sitecore/content/e-mail campaign/brother online/standard messages/self-service subscription/mps/notifications/proposal approved notification email",
                @"/mps/dealer/proposals/create",
                @"/mps/sitecore/content/e-mail campaign/brother online/standard messages/self-service subscription/mps/notifications/contract rejected notification email",
                @"/mps/handlebars/product-selection-status-items",
                @"/mps/handlebars/product-configuration / default",
                @"/mps/handlebars/products-available-items",
                @"/mps/handlebars/product-added-successfully",
                @"/mps/sitecore/content/e-mail campaign/brother online/standard messages/self-service subscription/mps/notifications/proposal submitted notification email",
                @"/mps/sitecore/content/e-mail campaign/brother online/standard messages/self-service subscription/mps/notifications/contract signed notification email"
            };


            if (comparables.Contains(url))
                return true;
            return false;
        }

        internal IEnumerable<ResultLine> GenerateStats(List<Row> beforeRows, List<Row> afterRows)
        {
            var beforeStats = beforeRows
                .GroupBy(row => row.URL)
                .Select(c1 => new ResultLine
                {
                    URL = c1.First().URL,
                    HitsBefore = c1.Count(),
                    SumBefore = c1.Sum(x => x.TimeTaken),
                    MaxBefore = c1.Max(x => x.TimeTaken),
                    MinBefore = c1.Min(x=>x.TimeTaken),
                    Comparable = c1.First().IsComparable
                })
                .OrderByDescending(x => x.Comparable)
                .ThenBy(x => x.URL);

            var afterStats = afterRows
                .GroupBy(row => row.URL)
                .Select(c1 => new ResultLine
                {
                    URL = c1.First().URL,
                    HitsAfter = c1.Count(),
                    SumAfter = c1.Sum(x => x.TimeTaken),
                    MaxAfter = c1.Max(x => x.TimeTaken),
                    MinAfter = c1.Min(x => x.TimeTaken),
                    Comparable = c1.First().IsComparable
                })
                .OrderByDescending(x => x.Comparable)
                .ThenBy(x => x.URL);

            // Now merge two lists
            return (from beforeStat in beforeStats
                from afterStat in afterStats
                where afterStat.URL.ToString() == beforeStat.URL.ToString()
                select new ResultLine
                {
                    URL = beforeStat.URL,
                    Comparable = beforeStat.Comparable,
                    HitsBefore = beforeStat.HitsBefore,
                    HitsAfter = afterStat.HitsAfter,
                    MaxBefore = beforeStat.MaxBefore,
                    MaxAfter = afterStat.MaxAfter,
                    SumBefore = beforeStat.SumBefore,
                    SumAfter = afterStat.SumAfter,
                    MinBefore = beforeStat.MinBefore,
                    MinAfter = afterStat.MinAfter
                }).ToList();
        }
    }

    internal class ResultLine
    {
        [CsvColumn(FieldIndex = 1)]
        public object URL { get; set; }

        [CsvColumn(FieldIndex = 2)]
        public int HitsBefore { get; set; }

        [CsvColumn(FieldIndex = 3)]
        public double? SumBefore { get; set; }

        [CsvColumn(FieldIndex = 4)]
        public double? AvgBefore => (double) (SumBefore / HitsBefore);

        [CsvColumn(FieldIndex = 5)]
        public double? MaxBefore { get; set; }

        [CsvColumn(FieldIndex = 6)]
        public double? MinBefore { get; set; }

        [CsvColumn(FieldIndex = 7)]
        public bool Comparable { get; set; }

        [CsvColumn(FieldIndex = 9)]
        public int HitsAfter { get; set; }

        [CsvColumn(FieldIndex = 10)]
        public double? SumAfter { get; set; }

        [CsvColumn(FieldIndex = 11)]
        public double? AvgAfter => (double) (SumAfter / HitsAfter);

        [CsvColumn(FieldIndex = 12)]
        public double? MaxAfter { get; set; }

        [CsvColumn(FieldIndex = 13)]
        public double? MinAfter { get; set; }

        [CsvColumn(FieldIndex = 14, Name = "Improvement %")]
        public double? Improvement => (AvgBefore-AvgAfter)/AvgBefore * 100;
    }
}