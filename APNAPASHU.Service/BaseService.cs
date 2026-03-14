using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace APNAPASHU.Service
{
    /// <summary>
    /// Base Service with common functionality
    /// </summary>
    public abstract class BaseService
    {
        protected IConfiguration Configuration { get; set; }
        protected string ClientIPAddress { get; set; } = string.Empty;
        protected int LoggedInUserId { get; set; }
        protected string FullUserName { get; set; } = string.Empty;
        protected string ApplicationHostUrl { get; set; } = string.Empty;
        protected IHttpContextAccessor HttpContextAccessor { get; private set; }

        public BaseService(IHttpContextAccessor accessor, IConfiguration configuration)
        {
            HttpContextAccessor = accessor;
            Configuration = configuration;

            if (accessor?.HttpContext != null)
            {
                try
                {
                    ApplicationHostUrl = accessor.HttpContext.Request.Scheme + "://" + accessor.HttpContext.Request.Host;
                    ClientIPAddress = accessor.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";

                    if (accessor.HttpContext.User?.Identity?.IsAuthenticated == true)
                    {
                        var userIdClaim = accessor.HttpContext.User.FindFirst("UserId");
                        if (int.TryParse(userIdClaim?.Value, out int userId))
                            LoggedInUserId = userId;

                        var nameClaim = accessor.HttpContext.User.FindFirst("FullUserName");
                        FullUserName = nameClaim?.Value ?? string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    // Log exception if needed
                }
            }
        }
    }
}