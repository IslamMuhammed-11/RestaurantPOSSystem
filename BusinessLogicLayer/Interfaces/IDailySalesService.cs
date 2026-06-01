using DataAccessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Contracts.DTOs.ReportsDTOs.SalesReports;
using Contracts.Queries.ReportsQueries;
using Contracts.ErrorHandling;

namespace BusinessLogicLayer.Interfaces
{
    public interface IDailySalesService
    {
        public Task<bool> LogDailySalesAsync(decimal amount);

        public Task<Result<SalesComparisonResponse>> GetSalesComparisonAsync(SalesComparisonQuery query);

        public Task<Result<SalesDetailsResponse>> GetSalesDetailsAsync(RangedQuery query);

        public Task<Result<SaleTrendsResponse>> GetSalesTrendsAsync(PeriodicQuery query);
    }
}