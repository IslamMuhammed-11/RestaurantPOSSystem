using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs.SalesReports
{
    public class SalesComparisonResponse
    {
        public PeriodMetrics CurrentPeriod { get; set; } = new();

        public PeriodMetrics PreviousPeriod { get; set; } = new();

        public GrowthMetrics Growth { get; set; } = new();

        public ComparisonMeta Meta { get; set; } = new();
    }
}