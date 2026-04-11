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
        [ProducesResponseType(typeof(JsonModel<LoginResponseModel>), 200)]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            if (string.IsNullOrEmpty(model.Email))
                return BadRequest(new JsonModel<object>(null, "Email is required", 400));

            var result = await _authenticationService.LoginUserAsync(model.Email, model.Password);
            return Ok(result);
        }
    }
}
