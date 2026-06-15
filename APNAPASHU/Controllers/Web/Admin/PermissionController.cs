using APNAPASHU.DataContract.Models.Web.Admin.Permission;
using APNAPASHU.ServiceContract.Web.Admin;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace APNAPASHU.API.Controllers.Web.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        [HttpGet("{roleId}")]
        public async Task<IActionResult> GetPermissions(int roleId)
        {
            try
            {
                var permissions = await _permissionService.GetRolePermissionsAsync(roleId);
                return Ok(new { StatusCode = 200, Message = "Success", Data = permissions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error: " + ex.Message });
            }
        }

        [HttpPost("save")]
        public async Task<IActionResult> SavePermissions([FromBody] SaveRolePermissionsRequest request)
        {
            try
            {
                // Assuming standard userId as 1 for now if claims are not set up
                int userId = 1; 
                var result = await _permissionService.SaveRolePermissionsAsync(request, userId);
                int statusCode = result.StatusCode == "200" ? 200 : 500;
                return Ok(new { StatusCode = statusCode, Message = result.Message ?? "Failed to save permissions", Data = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { StatusCode = 500, Message = "Internal server error: " + ex.Message });
            }
        }
    }
}
