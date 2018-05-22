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
                @"/comparables.Add(/proposal/current/products-available",
                @"/dealer/proposals/convert/customer-information",
                @"/dealer/proposals/create/customer-information",
                @"/dealer/proposals/create/summary",
                @"/dealer/proposals/create/click-price",
                @"/dealer/proposals/create/products",
                @"/proposal/current/product/add-or-update",
                @"/dealer/proposals/create/description",
                @"/dealer/proposals/create/term-type",
                @"/dealer/proposals/summary",
                @"/dealer/proposals/convert/term-type",
                @"/proposal/current/click-price",
                @"/dealer/proposals/convert/products",
                @"/proposal/current/product-configuration/1",
                @"/dealer/proposals/convert/click-price",
                @"/dealer/contracts/awaiting-acceptance",
                @"/dealer/dashboard",
                @"/dealer/contracts/rejected",
                @"/dealer/proposals/in progress",
                @"/dealer/contracts/approved-proposals",
                @"/local-office/approval/proposals/awaiting-approval",
                @"/dealer/proposals/awaiting-approval",
                @"/proposal/current/product-selection-status",
                @"/dealer/proposals/approved",
                @"/local-office/approval/contracts/awaiting-acceptance",
                @"/local-office/approval/contracts/rejected",
                @"/local-office/approval/contracts/approved-proposals",
                @"/local-office/approval/proposals/approved",
                @"/local-office/approval/proposals",
                @"/local-office/approval/dashboard",
                @"/local-office/dashboard",
                @"/dealer",
                @"/js/datatables",
                @"/dealer/create-proposal",
                @"/dealer/contracts",
                @"/local-office/approval",
                @"/local-office/approval/contracts",
                @"/dealer/proposals",
                @"/handlebars/product-selection-status-no-items",
                @"/local-office",
                @"/sitecore/content/e-mail campaign/brother online/standard messages/self-service subscription/mps/notifications/proposal approved notification email",
                @"/dealer/proposals/create",
                @"/sitecore/content/e-mail campaign/brother online/standard messages/self-service subscription/mps/notifications/contract rejected notification email",
                @"/handlebars/product-selection-status-items",
                @"/handlebars/product-configuration / default",
                @"/handlebars/products-available-items",
                @"/handlebars/product-added-successfully",
                @"/sitecore/content/e-mail campaign/brother online/standard messages/self-service subscription/mps/notifications/proposal submitted notification email",
                @"/sitecore/content/e-mail campaign/brother online/standard messages/self-service subscription/mps/notifications/contract signed notification email"
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