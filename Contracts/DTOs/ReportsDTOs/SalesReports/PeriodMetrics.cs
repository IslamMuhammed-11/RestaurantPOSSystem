using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs.SalesReports
{
    public class PeriodMetrics
    {
        public decimal TotalSales { get; set; }

        public int TotalOrders { get; set; }

        public int DaysCount { get; set; }

        public decimal AverageSalesPerDay { get; set; }

        public decimal AverageOrdersPerDay { get; set; }
    }
}