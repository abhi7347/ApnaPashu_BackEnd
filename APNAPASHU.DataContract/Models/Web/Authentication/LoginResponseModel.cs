namespace APNAPASHU.DataContract.Models.Web.Authentication
{
    public class LoginResponseModel : SqlResponseModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string PasswordHash { get; set; } = string.Empty; // Keep for internal validation
    }
}
