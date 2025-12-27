using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SalesAnalysisApp.Entities
{
    public class OrderItem: BaseAuditEntity
    {
         public int OrderItemId { get; set; }

        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }

        [Required]
        public int OrderId { get; set; }
        [ForeignKey(nameof(OrderId))]
        public Order Order { get; set; }

       

        public int QuantitySold { get; set; }
        [Column(TypeName = "decimal(10,2)")]
        public decimal UnitPrice {  get; set; }
        [Column(TypeName = "decimal(5,2)")]
        public decimal Discount { get;set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "decimal(12,2)")]
        public decimal LineTotal { get; set; }

        public DateTime CreateAt { get; set; }=DateTime.Now;



    }
}
