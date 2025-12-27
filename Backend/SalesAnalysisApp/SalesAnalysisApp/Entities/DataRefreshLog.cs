using System.ComponentModel.DataAnnotations;

namespace SalesAnalysisApp.Entities
{
    public class DataRefreshLog
    {
        [Key]
       
        public int LogId { get; set; }
        [Required]
        public string RefreshType { get; set; }

        [Required]
        public string Status { get; set; }

        public int RecordsProcessed {  get; set; }
        public int RecordsInserted { get; set; }
        public int RecordsUpdated { get; set; }
        public int RecordsFailed { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime StartedAt { get; set; }=DateTime.Now;
        public DateTime? CompletedAt { get; set; }





    }
}
