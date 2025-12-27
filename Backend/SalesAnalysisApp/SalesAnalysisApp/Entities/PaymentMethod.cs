using System.ComponentModel.DataAnnotations;
namespace SalesAnalysisApp.Entities
{
    public class PaymentMethod: BaseAuditEntity
    {
        [Key]
        public int PaymentMethodId { get; set; }
        [Required]
        [MaxLength(100)]
        public string PaymentMethodName { get; set; }

        public ICollection<Order> Orders { get; set; }

    }
}
