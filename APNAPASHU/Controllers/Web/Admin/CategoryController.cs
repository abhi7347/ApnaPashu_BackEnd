using Microsoft.AspNetCore.Mvc;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Categories;
using APNAPASHU.ServiceContract.Web.Admin;
using System.Net;

namespace APNAPASHU.API.Controllers.Web.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(
            ICategoryService categoryService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(JsonModel<List<CategoryResponseModel>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] FilterDto filter)
        {
            var result = await _categoryService.GetAllAsync(filter);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(JsonModel<CategoryResponseModel>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryService.GetByIdAsync(id);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpPost("upsert")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Upsert([FromForm] CategoryUpsertModel model)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _categoryService.UpsertAsync(model, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }

        [HttpPost("update-status")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusDto model)
        {
            int userId = GetAuthenticatedUserId();
            model.UserId = userId;
            var result = await _categoryService.UpdateStatusAsync(model);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }

        [HttpPost("delete")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _categoryService.DeleteAsync(ids, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }
    }
}