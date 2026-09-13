using Microsoft.AspNetCore.Mvc;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.User;
using APNAPASHU.ServiceContract.Web.Admin;
using System.Net;

namespace APNAPASHU.API.Controllers.Web.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(
            IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _userService = userService;
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(JsonModel<List<UserResponseModel>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] FilterDto filter)
        {
            var result = await _userService.GetAllAsync(filter);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonModel<UserResponseModel>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetByIdAsync(id);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpPost("upsert")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Upsert([FromBody] UserUpsertModel model)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _userService.UpsertAsync(model, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }

        [HttpPost("update-status")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusDto model)
        {
            int userId = GetAuthenticatedUserId();
            model.UserId = userId;
            var result = await _userService.UpdateStatusAsync(model);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _userService.DeleteAsync(ids, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }
    }
}
