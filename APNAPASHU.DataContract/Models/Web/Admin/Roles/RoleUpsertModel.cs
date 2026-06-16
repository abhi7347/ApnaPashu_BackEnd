namespace APNAPASHU.DataContract.Models.Web.Admin.Roles
{
    public class RoleUpsertModel
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? RoleDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
