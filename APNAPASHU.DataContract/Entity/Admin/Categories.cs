namespace APNAPASHU.DataContract.Entity.Admin
{
    public class Categories: BaseEntity
    {
        public string CategoryName { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
    }
}
