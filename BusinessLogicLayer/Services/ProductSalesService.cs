using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.ReportsDTOs.ProductsReports;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Queries.ReportsQueries;
using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services
{
    public class ProductSalesService : IProductSalesService
    {
        private readonly IProductSalesRepo _productSalesRepo;

        public ProductSalesService(IProductSalesRepo productSalesRepo)
        {
            _productSalesRepo = productSalesRepo;
        }

        public async Task<ProductSalesResponse> GetTopProductsAsync(RangedQuery query)
        {
            if (!query.Validate())
                throw new BusinessException("Invalid period.", 8546, ActionResultEnum.ActionResult.InvalidData);

            query.Periodic = query.ResolvePeriod();

            var topProducts = await _productSalesRepo.TopFiveProductsInPeriodAsync(query.Periodic.from, query.Periodic.to);

            if (topProducts.Count == 0)
                return new ProductSalesResponse();

            List<ProductSaleRecord> list = ProductSalesMap.ToProductSalesList(topProducts);

            ProductSalesResponse response = new ProductSalesResponse();

            response.TopProducts = list;

            return response;
        }

        public async Task<bool> LogProductSalesAsync(int orderID)
        {
            return await _productSalesRepo.LogProductSalesAsync(orderID);
        }

        private bool _ValidatePeriodic(PeriodicQuery? periodic)
        {
            if (periodic == null)
                return false;

            if (periodic.from > periodic.to)
                return false;

            return true;
        }
    }
}