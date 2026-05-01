using Contracts.DTOs.ReportsDTOs.ProductsReports;
using Contracts.Queries.ReportsQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Interfaces
{
    public interface IProductSalesService
    {
        public Task<bool> LogProductSalesAsync(int orderID);

        public Task<ProductSalesResponse> GetTopProductsAsync(RangedQuery query);
    }
}