using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesAnalysisApp.Entities
{
    public class Product: BaseAuditEntity
    {
        [Key]
       
        public int ProductId {  get; set; }
        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }
        [Required]
        public int CategoryId {  get; set; }
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public decimal UnitPrice { get; set; }

        [Column(TypeName ="decimal(10,2)")]
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
