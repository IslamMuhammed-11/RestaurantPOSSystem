using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.Entites;

namespace DataAccessLayer.Interfaces
{
    public interface IDailySalesRepo
    {
        public Task<bool> LogDailySales(decimal amount);

        public Task<DailySalesEntity?> SalesDetails(DateOnly from, DateOnly to);

        public Task<SalesComparisonEntity> SalesComparison(DateOnly currentStart, DateOnly currentEnd,
                                                                           DateOnly prevStart, DateOnly prevEnd);

        public Task<List<DailySalesEntity>> SalesTrend(DateOnly from, DateOnly to);
    }
}