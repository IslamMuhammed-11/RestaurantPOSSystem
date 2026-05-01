namespace Contracts.DTOs.ReportsDTOs.SalesReports
{
    public class GrowthMetrics
    {
        public enum GrowthStatus
        {
            Increase,
            Decrease,
            NoChange,
            New
        }

        public decimal? SalesGrowthPercent { get; set; }
        public decimal? OrderGrowthPercent { get; set; }

        public decimal? AvreageSalesGrowthPercent { get; set; }
        public decimal? AvreageOrdersGrowthPercent { get; set; }
        public GrowthStatus Status { get; set; } = GrowthStatus.New;
    }
}