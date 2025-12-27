namespace SalesAnalysisApp.Entities
{
    public abstract class BaseAuditEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set;} = DateTime.Now;
    }

}
