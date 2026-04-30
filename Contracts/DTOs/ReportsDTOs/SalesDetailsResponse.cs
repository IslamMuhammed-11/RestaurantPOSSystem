using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs
{
    public class SalesDetailsResponse
    {
        public int TotalOrders { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalNet { get; set; }
    }
}