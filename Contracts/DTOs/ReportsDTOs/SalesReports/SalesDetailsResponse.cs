using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs.SalesReports
{
    public class SalesDetailsResponse
    {
        public int TotalOrders { get; set; } = 0;
        public decimal TotalGross { get; set; } = 0;
        public decimal TotalNet { get; set; } = 0;
    }
}