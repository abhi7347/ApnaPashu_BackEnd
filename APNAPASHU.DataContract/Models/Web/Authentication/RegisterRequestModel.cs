namespace APNAPASHU.DataContract.Models.Web.Authentication
{
    public class RegisterRequestModel
    {
        public int RoleId { get; set; }
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? GenderCode { get; set; }
        public string Password { get; set; } = null!; // Becomes PasswordHash in repository
        public string? Address { get; set; }
        public DateTime? DOB { get; set; }
        public int? PinCode { get; set; }
        public string? City { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public bool IsTermsAccepted { get; set; }
    }
}
