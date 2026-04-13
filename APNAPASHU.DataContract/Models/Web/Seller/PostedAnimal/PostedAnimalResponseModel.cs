namespace APNAPASHU.DataContract.Models.Web.Seller.PostedAnimal
{

    public class PostedAnimalResponseModel: CommonAuditDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Breed { get; set; }
        public string? Age { get; set; }
        public decimal Price { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        public string? ImagesJson { get; set; } // raw JSON from DB (usp_GetPostedAnimalById & usp_GetAllPostedAnimals)
        public List<string>? Images { get; set; } // processed full URLs for frontend
        public string? ImageUrl { get; set; } // primary image for list display
    }
}
