using Microsoft.AspNetCore.Http;

namespace APNAPASHU.DataContract.Models.Web.Seller.PostedAnimal
{
    public class PostedAnimalUpsertModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public int? CategoryId { get; set; }
        public string? Breed { get; set; }
        public string? Age { get; set; }
        public decimal? Price { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }

        public List<IFormFile>? NewImages { get; set; }
        public List<string>? ExistingImages { get; set; }
        public List<string>? ImageNames { get; set; }
    }
}
