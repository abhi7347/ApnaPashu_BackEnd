using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace APNAPASHU.API.Controllers
{
    /// <summary>
    /// Base Controller with common functionality
    /// </summary>
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected IHttpContextAccessor ContextAccessor { get; set; }
        protected IConfiguration Configuration { get; set; }

        public BaseController(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            ContextAccessor = contextAccessor;
            Configuration = configuration;
        }

        /// <summary>
        /// Validate email format
        /// </summary>
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validate phone number
        /// </summary>
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            string pattern = @"^[0-9]{10,15}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        /// <summary>
        /// Get authenticated user ID
        /// </summary>
        protected int GetAuthenticatedUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) ?? User.FindFirst("UserId");
            if (int.TryParse(userIdClaim?.Value, out int userId))
                return userId;
            return 0;
        }

        /// <summary>
        /// Get client IP Address
        /// </summary>
        protected string GetClientIpAddress()
        {
            return ContextAccessor?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        }
    }
}