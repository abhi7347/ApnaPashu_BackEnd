using Microsoft.AspNetCore.Http;

namespace APNAPASHU.DataContract.Models.Web.Authentication
{
    public class UpdateProfileRequestModel
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? NewPassword { get; set; }
        public IFormFile? Image { get; set; }
    }
}
