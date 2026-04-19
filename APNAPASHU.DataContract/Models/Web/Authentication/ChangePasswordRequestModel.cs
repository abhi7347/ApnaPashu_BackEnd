namespace APNAPASHU.DataContract.Models.Web.Authentication
{
    public class ChangePasswordRequestModel
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
}
