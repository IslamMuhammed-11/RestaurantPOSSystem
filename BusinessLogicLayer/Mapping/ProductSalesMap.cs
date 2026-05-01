using Contracts.DTOs.ProductDTOs;
using Contracts.DTOs.ReportsDTOs.ProductsReports;
using DataAccessLayer.Entites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Mapping
{
    public class ProductSalesMap
    {
        public static ProductSaleRecord ToSaleRecord(ProductSalesEntity Entity)
        {
            return new ProductSaleRecord
            {
                ProductId = Entity.ProductId,
                ProductName = Entity.Name,
                TotalSold = Entity.QuantitySold,
                TotalRevenue = Entity.TotalRevenue
            };
        }

        public static List<ProductSaleRecord> ToProductSalesList(List<ProductSalesEntity> entities)
        {
            return entities.Select(ToSaleRecord).ToList();
        }
    }
}