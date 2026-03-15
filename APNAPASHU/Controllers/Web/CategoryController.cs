using Microsoft.AspNetCore.Mvc;
using APNAPASHU.DataContract.Models;
using APNAPASHU.ServiceContract.Web;
using APNAPASHU.DataContract.Models.Web.Categories;

namespace APNAPASHU.API.Controllers.Web
{
    [Route("api/web/[controller]")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ICategoryService categoryService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ILogger<CategoryController> logger)
            : base(httpContextAccessor, configuration)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(JsonModel<List<CatetoryResponseModel>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] FilterDto filter)
        {
            var result = await _categoryService.GetAllAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Get category by Id
        /// </summary>
        [HttpGet("{categoryId}")]
        [ProducesResponseType(typeof(JsonModel<CatetoryResponseModel>), 200)]
        public async Task<IActionResult> GetById(int categoryId)
        {
            var result = await _categoryService.GetByIdAsync(categoryId);
            return Ok(result);
        }

        /// <summary>
        /// Create or Update category
        /// </summary>
        [HttpPost("upsert")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Upsert([FromBody] CategoryUpsertModel model)
        {
            int userId = GetAuthenticatedUserId();

            var result = await _categoryService.UpsertAsync(model, userId);
            return Ok(result);
        }

        /// <summary>
        /// Update category status
        /// </summary>
        [HttpPut("update-status")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> UpdateStatus([FromBody] UpdateStatusDto model)
        {
            int userId = GetAuthenticatedUserId();
            model.UserId = userId;

            var result = await _categoryService.UpdateStatusAsync(model);
            return Ok(result);
        }

        /// <summary>
        /// Delete category (soft delete)
        /// </summary>
        [HttpDelete("{categoryId}")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Delete(int categoryId)
        {
            int userId = GetAuthenticatedUserId(); 

            var result = await _categoryService.DeleteAsync(categoryId, userId);
            return Ok(result);
        }
    }
}