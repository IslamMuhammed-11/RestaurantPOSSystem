using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs
{
    public class SalesTrendResponse
    {
        public DateOnly Date { get; set; }
        public decimal TotalSales { get; set; }
    }
}