using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs.SalesReports
{
    public class SalesTrend
    {
        public DateOnly Date { get; set; }
        public decimal TotalSales { get; set; }
    }
}