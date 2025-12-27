namespace SalesAnalysisApp.Dtos
{
    public class TopProductResponse
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }
        public string CategoryNAme { get; set; }

        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue  { get; set; }

    }
}
