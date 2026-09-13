using Microsoft.AspNetCore.Mvc;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Staff;
using APNAPASHU.ServiceContract.Web.Admin;
using System.Net;

namespace APNAPASHU.API.Controllers.Web.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class StaffController : BaseController
    {
        private readonly IStaffService _staffService;

        public StaffController(
            IStaffService staffService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _staffService = staffService;
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(JsonModel<List<StaffResponseModel>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] FilterDto filter)
        {
            var result = await _staffService.GetAllAsync(filter);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonModel<StaffResponseModel>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _staffService.GetByIdAsync(id);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpPost("upsert")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Upsert([FromBody] StaffUpsertModel model)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _staffService.UpsertAsync(model, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }

        [HttpPost("update-status")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusDto model)
        {
            int userId = GetAuthenticatedUserId();
            model.UserId = userId;
            var result = await _staffService.UpdateStatusAsync(model);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _staffService.DeleteAsync(ids, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }
    }
}
