namespace SalesAnalysisApp.Dtos
{
    public class DataRefreshResponse
    {

        public int LogId {  get; set; }
        public int TotalCustomers {  get; set; }
        public string Message { get; set; }

        public int RecordsProcessed {  get; set; }
    }
}
