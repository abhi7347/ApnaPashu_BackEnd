namespace APNAPASHU.DataContract.Models.Web.Authentication
{
    public class LoginResponseModel : SqlResponseModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public string? ProfileImage { get; set; } // Raw path from DB
        public string? Image { get; set; } // Signed URL for Frontend
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string PasswordHash { get; set; } = string.Empty; // Keep for internal validation
    }
}
