using APNAPASHU.API.Controllers;
using APNAPASHU.ServiceContract.Web.Admin;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace APNAPASHU.API.Controllers.Web.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class SidebarController : BaseController
    {
        private readonly IAdminSidebarService _adminSidebarService;

        public SidebarController(IAdminSidebarService adminSidebarService,
            IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _adminSidebarService = adminSidebarService;
        }

        [HttpGet("menu")]
        public async Task<IActionResult> GetMenu()
        {
            try
            {
                int userId = GetAuthenticatedUserId();
                var menu = await _adminSidebarService.GetAdminSidebarMenuAsync(userId);
                return Ok(new { StatusCode = 200, Message = "Success", Data = menu });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error: " + ex.Message });
            }
        }
    }
}
