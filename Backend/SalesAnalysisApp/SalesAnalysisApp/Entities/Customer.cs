using System.ComponentModel.DataAnnotations;

namespace SalesAnalysisApp.Entities
{
    public class Customer: BaseAuditEntity
    {
        [Key]
        public int CustomerId { get; set; }
        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; }
        [EmailAddress]
        public string CustomerEmail { get; set; }
        [MaxLength(100)]
        public string CustomerAddress { get; set; }
    //    public ICollection<Order> Orders { get; set; }
    }
}
