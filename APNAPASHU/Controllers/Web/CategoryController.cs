using Microsoft.AspNetCore.Mvc;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Category;
using APNAPASHU.ServiceContract.Web;

namespace APNAPASHU.API.Controllers.Web
{
    /// <summary>
    /// Web Category Controller - CRUD Template (GetAll, Upsert, UpdateStatus, Delete)
    /// </summary>
    [Route("api/web/[controller]")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration, ILogger<CategoryController> logger)
            : base(httpContextAccessor, configuration)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        /// <summary>
        /// Get all categories with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter parameters (SearchTerm, IsActive, PageNumber, PageSize)</param>
        /// <returns>List of categories</returns>
        [HttpPost("get-all")]
        [ProducesResponseType(typeof(JsonModel<CategoryListResponseDto>), 200)]
        public async Task<IActionResult> GetAll([FromBody] CategoryFilterDto filter)
        {
            _logger.LogInformation($"Web: Getting categories - Page {filter?.PageNumber}, Size {filter?.PageSize}");
            var result = await _categoryService.GetAllAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Create or Update category (Upsert)
        /// </summary>
        /// <param name="upsertDto">Category data (CategoryId null for create, populated for update)</param>
        /// <returns>Created or updated category</returns>
        [HttpPost("upsert")]
        [ProducesResponseType(typeof(JsonModel<CategoryResponseDto>), 201)]
        [ProducesResponseType(typeof(JsonModel<CategoryResponseDto>), 200)]
        public async Task<IActionResult> Upsert([FromBody] CategoryUpsertDto upsertDto)
        {
            _logger.LogInformation($"Web: Upserting category: {upsertDto?.CategoryName}");
            var result = await _categoryService.UpsertAsync(upsertDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update category status (IsActive)
        /// </summary>
        /// <param name="statusDto">Category ID and IsActive status</param>
        /// <returns>Success status</returns>
        [HttpPut("update-status")]
        [ProducesResponseType(typeof(JsonModel<bool>), 200)]
        public async Task<IActionResult> UpdateStatus([FromBody] CategoryStatusUpdateDto statusDto)
        {
            _logger.LogInformation($"Web: Updating category status - ID: {statusDto?.CategoryId}, IsActive: {statusDto?.IsActive}");
            var result = await _categoryService.UpdateStatusAsync(statusDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete category (soft delete)
        /// </summary>
        /// <param name="id">Category ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel<bool>), 200)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Web: Deleting category with ID: {id}");
            var result = await _categoryService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}
