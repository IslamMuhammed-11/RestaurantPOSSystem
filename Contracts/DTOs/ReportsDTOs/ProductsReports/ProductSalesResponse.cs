using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.DTOs.ReportsDTOs.ProductsReports
{
    public class ProductSalesResponse
    {
        public List<ProductSaleRecord> TopProducts { get; set; } = new List<ProductSaleRecord>();
    }
}