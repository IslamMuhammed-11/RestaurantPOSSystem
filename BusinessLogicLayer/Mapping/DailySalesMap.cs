using Contracts.DTOs.ReportsDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entites;
using Contracts.DTOs.ReportsDTOs.SalesReports;

namespace BusinessLogicLayer.Mapping
{
    public class DailySalesMap
    {
        public static SalesComparisonResponse MapToSalesComparisonResponse(SalesComparisonEntity entity)
        {
            return new SalesComparisonResponse
            {
                CurrentPeriod = new PeriodMetrics
                {
                    TotalSales = entity.Current.Gross,
                    TotalOrders = entity.Current.TotalOrders,

                    DaysCount = entity.Current.Date.DayNumber - entity.Prev.Date.DayNumber + 1
                },
                PreviousPeriod = new PeriodMetrics
                {
                    TotalSales = entity.Current.Gross,
                    TotalOrders = entity.Current.TotalOrders
                }
            };
        }

        public static SalesDetailsResponse MapToSalesDetailsResponse(DailySalesEntity entity)
        {
            return new SalesDetailsResponse
            {
                TotalOrders = entity.TotalOrders,
                TotalGross = entity.Gross,
                TotalNet = entity.Net
            };
        }

        public static SalesTrend MapToSalesTrendResponse(DailySalesEntity entity)
        {
            return new SalesTrend
            {
                Date = entity.Date,
                TotalSales = entity.Gross
            };
        }

        public static List<SalesTrend> MapToSalesTrendResponseList(List<DailySalesEntity> entities)
        {
            return entities.Select(s => MapToSalesTrendResponse(s)).ToList();
        }
    }
}