namespace SalesAnalysisApp.Dtos
{
    public class RevenueResponse
    {
        public decimal TotalRevenue { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set;}
    }
}
