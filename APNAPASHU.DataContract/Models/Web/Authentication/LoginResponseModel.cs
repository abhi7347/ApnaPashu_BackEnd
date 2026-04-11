namespace APNAPASHU.DataContract.Models.Web.Authentication
{
    public class LoginResponseModel
    {
        public int UserId { get; set; }
        public string? StatusCode { get; set; }
        public string? Message { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Keep for internal validation
    }
}
