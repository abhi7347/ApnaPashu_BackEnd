namespace APNAPASHU.DataContract.Models.Web.Categories
{
    public class CatetoryResponseModel: CommonAuditDto
    {
        public string? CategoryName { get; set; }

        public string? Description { get; set; }

        public string? ImagePath { get; set; }
    }
}
