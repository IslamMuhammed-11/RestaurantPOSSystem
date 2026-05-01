using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs.SalesReports
{
    public record SaleTrendsResponse
    {
        public List<SalesTrend> Trends { get; set; } = new();
    }
}