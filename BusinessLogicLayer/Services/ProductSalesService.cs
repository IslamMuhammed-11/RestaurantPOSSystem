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
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Services
{
    public class ProductSalesService : IProductSalesService
    {
        private readonly IProductSalesRepo _productSalesRepo;

        public ProductSalesService(IProductSalesRepo productSalesRepo)
        {
            _productSalesRepo = productSalesRepo;
        }

        public async Task<Result<ProductSalesResponse>> GetTopProductsAsync(RangedQuery query)
        {
            if (!query.Validate())
                return Result<ProductSalesResponse>.Failure(new Error("Invalid period.", ErrorCodes.enErrorCodes.INVALID_DATA));

            query.Periodic = query.ResolvePeriod();

            var topProducts = await _productSalesRepo.TopFiveProductsInPeriodAsync(query.Periodic.from, query.Periodic.to);

            if (topProducts.Count == 0)
                return Result<ProductSalesResponse>.Success(new ProductSalesResponse());

            List<ProductSaleRecord> list = ProductSalesMap.ToProductSalesList(topProducts);

            ProductSalesResponse response = new ProductSalesResponse();

            response.TopProducts = list;

            return Result<ProductSalesResponse>.Success(response);
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