using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs.SalesReports
{
    public class ComparisonMeta
    {
        public bool IsComparable { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}