using DataAccessLayer.Entites;

namespace DataAccessLayer.Interfaces
{
    public interface IProductSalesRepo
    {
        public Task<bool> LogProductSalesAsync(int orderID);

        public Task<List<ProductSalesEntity>> TopFiveProductsInPeriodAsync(DateOnly from, DateOnly to);
    }
}