namespace APNAPASHU.DataContract.Models.Web.Categories
{
    public class CategoryUpsertModel
    {
        public int? Id { get; set; }
        public string? CategoryName { get; set; }

        public string? Description { get; set; }

        public string? ImagePath { get; set; }
    }
}
