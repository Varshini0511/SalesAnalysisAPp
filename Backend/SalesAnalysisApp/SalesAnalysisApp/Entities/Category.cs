using System.ComponentModel.DataAnnotations;
namespace SalesAnalysisApp.Entities
{
    public class Category : BaseAuditEntity
    {
        [Key]
        public int CategoryId { get; set; }
       [Required]
        [MaxLength(50)]
        public string CategoryName { get; set; }

       // public ICollection<Product> Products { get; set; }
    }
}
