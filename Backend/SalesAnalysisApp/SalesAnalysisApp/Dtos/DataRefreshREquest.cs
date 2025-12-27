namespace SalesAnalysisApp.Dtos
{
    public class DataRefreshREquest
    {
        public string FilePath   {  get; set; }

        public bool IsFullRefresh { get; set; }
    }
}
