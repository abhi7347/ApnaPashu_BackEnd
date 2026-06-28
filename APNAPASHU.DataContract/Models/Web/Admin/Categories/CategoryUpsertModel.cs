using Microsoft.AspNetCore.Http;

namespace APNAPASHU.DataContract.Models.Web.Admin.Categories
{
    public class CategoryUpsertModel
    {
        public int? Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        
        // New image upload from frontend
        public IFormFile? Image { get; set; }
        
        // Existing image URL kept by user
        public string? ExistingImage { get; set; }
        
        // Name to be saved in DB
        public string? ImagePath { get; set; }
    }
}
