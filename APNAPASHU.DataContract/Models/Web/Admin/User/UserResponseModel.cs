using System;

namespace APNAPASHU.DataContract.Models.Web.Admin.User
{
    public class UserResponseModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? PasswordHash { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? PinCode { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? TotalRecords { get; set; }
    }
}
