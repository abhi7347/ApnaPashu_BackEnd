using Microsoft.AspNetCore.Mvc;
using APNAPASHU.DataContract.Models;
using APNAPASHU.ServiceContract.Web;
using APNAPASHU.DataContract.Models.Web.Authentication;

namespace APNAPASHU.API.Controllers.Web
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(
            IAuthenticationService authenticationService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ILogger<AuthenticationController> logger)
            : base(httpContextAccessor, configuration)
        {
            _authenticationService = authenticationService;
            _logger = logger;
        }

        /// <summary>
        /// User Registration
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(JsonModel<SqlResponseModel>), 200)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || !IsValidEmail(model.Email))
                return BadRequest(new JsonModel<object>(null, "Invalid email address", 400));

            var result = await _authenticationService.RegisterAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// User Login
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(new JsonModel<object>(null, "Email is required", 400));

            var result = await _authenticationService.LoginUserAsync(model.Email, model.Password);

            if (result.StatusCode == 200 && result.Data != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Required for SameSite=None cross-origin fetch
                    SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(1)
                };
                Response.Cookies.Append("AuthToken", result.Data.Token, cookieOptions);
            }

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(new JsonModel<object>(null, "Email is required", 400));

            var result = await _authenticationService.ForgotPasswordAsync(model.Email);

            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Token) || string.IsNullOrEmpty(model.NewPassword))
                return BadRequest(new JsonModel<object>(null, "Email, token, and new password are required", 400));

            var result = await _authenticationService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword);

            return Ok(result);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                Expires = DateTime.UtcNow.AddSeconds(-1)
            };
            Response.Cookies.Append("AuthToken", "", cookieOptions);
            return Ok(new JsonModel<object>(null, "Logged out successfully", 200));
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public IActionResult GetCurrentUser()
        {
            var cookieTokenString = Request.Cookies["AuthToken"];
            var hasAuthToken = !string.IsNullOrEmpty(cookieTokenString);
            
            // Log manually what the Identity principal thinks
            var isAuth = User.Identity?.IsAuthenticated == true;
            var userId = GetAuthenticatedUserId();

            if (!isAuth || userId == 0) 
            {
                var debugPayload = new { 
                    message = "Debug Validation Failed", 
                    cookieAttached = hasAuthToken, 
                    isAuthorizedByMiddleware = isAuth,
                    userIdExtracted = userId,
                    nameIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "NULL"
                };
                return StatusCode(401, new JsonModel<object>(debugPayload, "Development API Rejected Session", 401));
            }

            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            var data = new { UserId = userId, Email = email, Role = role };
            return Ok(new JsonModel<object>(data, "Success", 200));
        }
    }
}
