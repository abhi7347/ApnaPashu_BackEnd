namespace APNAPASHU.DataContract.Models.Web.Authentication
{
    public class AuthResponseModel
    {
        public int? UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public string? Role { get; set; }
        public string? Message { get; set; }
        public int? StatusCode { get; set; }
        public int? Status { get; set; } // SP Return Value
        public string? Token { get; set; }
    }
}
