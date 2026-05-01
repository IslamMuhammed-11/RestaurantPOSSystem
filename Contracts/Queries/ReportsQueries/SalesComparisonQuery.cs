using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Queries.ReportsQueries
{
    public class SalesComparisonQuery
    {
        public DateOnly currentStart { get; set; }
        public DateOnly currentEnd { get; set; }

        public DateOnly prevStart { get; set; }
        public DateOnly prevEnd { get; set; }
    }
}