namespace APNAPASHU.DataContract.Models.Web.Admin.Roles
{
    public class RoleResponseModel : CommonAuditDto
    {
        public string RoleName { get; set; } = string.Empty;
        public string? RoleDescription { get; set; }
    }
}
