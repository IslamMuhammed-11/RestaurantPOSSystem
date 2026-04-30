using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs
{
    public class SalesComparisonResponse
    {
        public TotalSalesAndOrdersResponse CurrentPeriod { get; set; } = new();
        public TotalSalesAndOrdersResponse PreviousPeriod { get; set; } = new();

        public double GrothPrecent { get; set; }
    }
}