using System;

namespace APNAPASHU.DataContract.Models.Web.Admin.Staff
{
    public class StaffResponseModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string? Password { get; set; }
        public string? Phone { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? TotalRecords { get; set; }
    }
}
