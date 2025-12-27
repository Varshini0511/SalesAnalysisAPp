using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalysisApp.Entities
{
    public class Order: BaseAuditEntity
    {
        [Key]
       
        public int OrderId { get; set; }

       
        [Required]
        public int CustomerId {  get; set; }
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; }
    
        [Required]
        public int RegionId {  get; set; }
        [ForeignKey(nameof(RegionId))]
        public Region Region { get; set; }


        [Required]
        public int PaymentMethodId { get; set; }
        [ForeignKey(nameof(PaymentMethodId))]
        public Region PaymentMethod { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Column(TypeName ="decimal(10,2)")]
        public decimal ShippingCost {  get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
