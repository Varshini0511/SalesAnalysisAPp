namespace SalesAnalysisApp.Dtos
{
    public class CustomerReportResponse
    {
        public int TotalCustomers  { get; set; }

        public int TotalOrdrs { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}
