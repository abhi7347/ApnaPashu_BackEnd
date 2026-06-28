namespace APNAPASHU.DataContract.Models.Web.Admin.Categories
{
    public class CategoryResponseModel : CommonAuditDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
        
        // Signed URL returned to frontend
        public string? ImageUrl { get; set; }
        
        public int TotalRecords { get; set; }
    }
}
