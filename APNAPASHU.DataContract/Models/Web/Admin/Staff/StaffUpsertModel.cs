using System.ComponentModel.DataAnnotations;

namespace APNAPASHU.DataContract.Models.Web.Admin.Staff
{
    public class StaffUpsertModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public int RoleId { get; set; }

        public string? Password { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
