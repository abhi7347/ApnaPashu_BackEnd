using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APNAPASHU.DataContract.Entity.Seller
{
    [Table("PostedAnimalImages")]
    public class PostedAnimalImage
    {
        [Key]
        public int Id { get; set; }
        public int PostedAnimalId { get; set; }
        public string ImageName { get; set; } = string.Empty;

        [ForeignKey("PostedAnimalId")]
        public virtual PostedAnimal PostedAnimal { get; set; } = null!;
    }
}
