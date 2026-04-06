namespace APNAPASHU.DataContract.Models.Web.Authentication
{
    public class LoginRequestModel
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = string.Empty;
    }
}
