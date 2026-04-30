using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs
{
    public class TotalSalesAndOrdersResponse
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
    }
}