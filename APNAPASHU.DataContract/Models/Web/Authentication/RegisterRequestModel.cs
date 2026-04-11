namespace APNAPASHU.DataContract.Models.Web.Authentication
{
    public class RegisterRequestModel
    {
        public int RoleId { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!; // Becomes Password Hash in Service
        public bool IsTermsAccepted { get; set; }
    }
}
