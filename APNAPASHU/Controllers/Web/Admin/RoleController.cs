using Microsoft.AspNetCore.Mvc;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Roles;
using APNAPASHU.ServiceContract.Web.Admin;

namespace APNAPASHU.API.Controllers.Web.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(
            IRoleService roleService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _roleService = roleService;
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(JsonModel<List<RoleResponseModel>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] FilterDto filter)
        {
            var result = await _roleService.GetAllAsync(filter);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonModel<RoleResponseModel>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _roleService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost("upsert")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Upsert([FromBody] RoleUpsertModel model)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _roleService.UpsertAsync(model, userId);
            return Ok(result);
        }

        [HttpPost("update-status")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusDto model)
        {
            int userId = GetAuthenticatedUserId();
            model.UserId = userId;
            var result = await _roleService.UpdateStatusAsync(model);
            return Ok(result);
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Delete([FromBody] int[] ids)
        {
            int userId = GetAuthenticatedUserId();
            string idsStr = string.Join(",", ids);
            var result = await _roleService.DeleteAsync(idsStr, userId);
            return Ok(result);
        }
    }
}
