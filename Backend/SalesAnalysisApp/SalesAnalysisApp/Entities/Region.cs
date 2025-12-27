using System.ComponentModel.DataAnnotations;
namespace SalesAnalysisApp.Entities
{
    public class Region : BaseAuditEntity
    {
        [Key]
        public int RegionId {get; set;}
        [Required]
        [MaxLength(100)]
        public string RegionName { get; set;}

      //  public ICollection<Order> Orders { get; set;}
    }
}
