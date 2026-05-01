using BusinessLogicLayer.Interfaces;
using BusinessLogicLayer.Mapping;
using Contracts.DTOs.ReportsDTOs;
using Contracts.DTOs.ReportsDTOs.SalesReports;
using Contracts.Enums;
using Contracts.Exceptions;
using Contracts.Queries.ReportsQueries;
using DataAccessLayer.Interfaces;

namespace BusinessLogicLayer.Services
{
    public class DailySalesService : IDailySalesService
    {
        private readonly IDailySalesRepo _dailySalesRepo;

        public DailySalesService(IDailySalesRepo dailySalesRepo)
        {
            _dailySalesRepo = dailySalesRepo;
        }

        public async Task<bool> LogDailySalesAsync(decimal amount)
        {
            if (amount < 0)
                throw new BusinessException("Amount cannot be negative.", 90162, ActionResultEnum.ActionResult.InvalidData);

            return await _dailySalesRepo.LogDailySales(amount);
        }

        public async Task<SalesComparisonResponse> GetSalesComparisonAsync(SalesComparisonQuery query)
        {
            if (query.currentStart > query.currentEnd)
                throw new BusinessException("Current start date cannot be after current end date.", 90163, ActionResultEnum.ActionResult.InvalidData);

            if (query.prevStart > query.prevEnd)
                throw new BusinessException("Previous start date cannot be after previous end date.", 90164, ActionResultEnum.ActionResult.InvalidData);

            var entity = await _dailySalesRepo.SalesComparison(query.currentStart, query.currentEnd, query.prevStart, query.prevEnd);

            if (entity == null)
                entity = new();

            var currentDays = query.currentEnd.DayNumber - query.currentStart.DayNumber + 1;
            var previousDays = query.prevEnd.DayNumber - query.prevStart.DayNumber + 1;

            var current = new PeriodMetrics
            {
                TotalOrders = entity.Current.TotalOrders,
                TotalSales = entity.Current.Gross,
                DaysCount = currentDays,
                AverageOrdersPerDay = currentDays == 0 ? 0 : entity.Current.TotalOrders / currentDays,
                AverageSalesPerDay = currentDays == 0 ? 0 : entity.Current.Gross / currentDays,
            };

            var prev = new PeriodMetrics
            {
                TotalOrders = entity.Prev.TotalOrders,
                TotalSales = entity.Prev.Gross,
                DaysCount = previousDays,
                AverageOrdersPerDay = previousDays == 0 ? 0 : entity.Prev.TotalOrders / previousDays,
                AverageSalesPerDay = previousDays == 0 ? 0 : entity.Prev.Gross / previousDays,
            };

            var growth = new GrowthMetrics
            {
                SalesGrowthPercent = _claculateGrowthPrecent(entity.Current.Gross, entity.Prev.Gross),
                OrderGrowthPercent = _claculateGrowthPrecent(entity.Current.TotalOrders, entity.Prev.TotalOrders),
                AvreageSalesGrowthPercent = _claculateGrowthPrecent(current.AverageSalesPerDay, prev.AverageSalesPerDay),
                AvreageOrdersGrowthPercent = _claculateGrowthPrecent(current.AverageOrdersPerDay, prev.AverageOrdersPerDay)
            };

            var meta = new ComparisonMeta
            {
                IsComparable = prev.TotalSales > 0 || prev.TotalOrders > 0,
                Notes = prev.TotalSales == 0 ? "Previous period had no sales, growth percentage is not applicable." : string.Empty
            };

            if (prev.TotalSales == 0)
            {
                growth.SalesGrowthPercent = null;
                growth.OrderGrowthPercent = null;
                growth.AvreageSalesGrowthPercent = null;
                growth.AvreageOrdersGrowthPercent = null;

                growth.Status = current.TotalSales > 0 ? GrowthMetrics.GrowthStatus.New : GrowthMetrics.GrowthStatus.NoChange;
                growth.Status = current.TotalOrders > 0 ? GrowthMetrics.GrowthStatus.New : growth.Status;
            }
            else
            {
                growth.Status = growth.SalesGrowthPercent > 0 ? GrowthMetrics.GrowthStatus.Increase :
                                growth.SalesGrowthPercent < 0 ? GrowthMetrics.GrowthStatus.Decrease :
                                GrowthMetrics.GrowthStatus.NoChange;
            }

            return new SalesComparisonResponse
            {
                CurrentPeriod = current,
                PreviousPeriod = prev,
                Growth = growth,
                Meta = meta
            };
        }

        public async Task<SalesDetailsResponse> GetSalesDetailsAsync(RangedQuery query)
        {
            if (!query.Validate())
                throw new BusinessException("Invalid period.", 8546, ActionResultEnum.ActionResult.InvalidData);

            query.Periodic = query.ResolvePeriod();

            var entity = await _dailySalesRepo.SalesDetails(query.Periodic.from, query.Periodic.to);

            if (entity == null)
                entity = new();

            var response = new SalesDetailsResponse
            {
                TotalOrders = entity.TotalOrders,
                TotalGross = entity.Gross,
                TotalNet = entity.Net,
            };

            return response;
        }

        public async Task<SaleTrendsResponse> GetSalesTrendsAsync(PeriodicQuery query)
        {
            if (query.from > query.to)
                throw new BusinessException("Start date cannot be after end date.", 90166, ActionResultEnum.ActionResult.InvalidData);

            var list = await _dailySalesRepo.SalesTrend(query.from, query.to);

            if (list.Count == 0)
                return new SaleTrendsResponse();

            SaleTrendsResponse response = new();

            response.Trends = DailySalesMap.MapToSalesTrendResponseList(list);

            return response;
        }

        private decimal? _claculateGrowthPrecent(decimal currentGross, decimal previousGross)
        {
            if (previousGross <= 0)
                return null;

            return (currentGross - previousGross) / previousGross * 100;
        }

        private decimal? _claculateGrowthPrecent(int currentGross, int previousGross)
        {
            if (previousGross <= 0)
                return null;

            return (decimal)((currentGross - previousGross) / previousGross * 100);
        }
    }
}