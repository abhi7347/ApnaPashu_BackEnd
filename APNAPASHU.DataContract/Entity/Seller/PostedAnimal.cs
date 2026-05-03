using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APNAPASHU.DataContract.Entity.Seller
{
    [Table("PostedAnimals")]
    public class PostedAnimal: BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? Breed { get; set; }
        public int? AgeInMonths { get; set; }
        public int? GenderId { get; set; }
        public decimal Price { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        public bool? IsFeatured { get; set; }
        public DateTime? FeaturedTill { get; set; }
        public bool? IsSold { get; set; }
        public DateTime? SoldDate { get; set; }
        
        public virtual ICollection<PostedAnimalImage> Images { get; set; } = new List<PostedAnimalImage>();
    }


}
