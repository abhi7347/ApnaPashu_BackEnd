namespace APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal
{
    public class BrowseAnimalResponseModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Breed { get; set; }
        public int? AgeInMonths { get; set; }
        public int? GenderId { get; set; }
        public decimal Price { get; set; }
        public string? Location { get; set; }
        public string? Description { get; set; }
        public int UserId { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime? FeaturedTill { get; set; }
        public DateTime CreatedDate { get; set; }
        
        public string? ImagesJson { get; set; } 
        public List<string>? Images { get; set; } 
        public string? ImageUrl { get; set; } 
        public bool? IsFavorite { get; }

        public int TotalRecords { get; set; }
    }
}
