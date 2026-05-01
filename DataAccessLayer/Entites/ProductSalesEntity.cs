namespace DataAccessLayer.Entites
{
    public class ProductSalesEntity
    {
        public int ProductId { get; set; }

        public string Name { get; set; }

        public int QuantitySold { get; set; }

        public decimal TotalRevenue { get; set; }
    }
}