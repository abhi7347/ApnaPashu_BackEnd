using Microsoft.AspNetCore.Mvc;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Animal;
using APNAPASHU.ServiceContract.Mobile;

namespace APNAPASHU.API.Controllers.Mobile
{
    /// <summary>
    /// Mobile Animal Controller - CRUD Template (GetAll, Upsert, UpdateStatus, Delete)
    /// </summary>
    [Route("api/mobile/[controller]")]
    [ApiController]
    public class AnimalController : BaseController
    {
        private readonly IAnimalService _animalService;
        private readonly ILogger<AnimalController> _logger;

        public AnimalController(IAnimalService animalService, IHttpContextAccessor httpContextAccessor, 
            IConfiguration configuration, ILogger<AnimalController> logger)
            : base(httpContextAccessor, configuration)
        {
            _animalService = animalService;
            _logger = logger;
        }

        /// <summary>
        /// Get all animals with filtering and pagination
        /// </summary>
        /// <param name="filter">Filter parameters (Category, Location, SearchTerm, IsActive, PageNumber, PageSize)</param>
        /// <returns>List of animals</returns>
        [HttpPost("get-all")]
        [ProducesResponseType(typeof(JsonModel<AnimalListResponseDto>), 200)]
        public async Task<IActionResult> GetAll([FromBody] AnimalFilterDto filter)
        {
            _logger.LogInformation($"Mobile: Getting animals - Page {filter?.PageNumber}, Size {filter?.PageSize}");
            var result = await _animalService.GetAllAsync(filter);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Create or Update animal (Upsert)
        /// </summary>
        /// <param name="upsertDto">Animal data (AnimalId null for create, populated for update)</param>
        /// <returns>Created or updated animal</returns>
        [HttpPost("upsert")]
        [ProducesResponseType(typeof(JsonModel<AnimalResponseDto>), 201)]
        [ProducesResponseType(typeof(JsonModel<AnimalResponseDto>), 200)]
        public async Task<IActionResult> Upsert([FromBody] AnimalUpsertDto upsertDto)
        {
            _logger.LogInformation($"Mobile: Upserting animal: {upsertDto?.AnimalName}");
            var result = await _animalService.UpsertAsync(upsertDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Update animal status (IsActive)
        /// </summary>
        /// <param name="statusDto">Animal ID and IsActive status</param>
        /// <returns>Success status</returns>
        [HttpPut("update-status")]
        [ProducesResponseType(typeof(JsonModel<bool>), 200)]
        public async Task<IActionResult> UpdateStatus([FromBody] AnimalStatusUpdateDto statusDto)
        {
            _logger.LogInformation($"Mobile: Updating animal status - ID: {statusDto?.AnimalId}, IsActive: {statusDto?.IsActive}");
            var result = await _animalService.UpdateStatusAsync(statusDto);
            return StatusCode(result.StatusCode, result);
        }

        /// <summary>
        /// Delete animal (soft delete)
        /// </summary>
        /// <param name="id">Animal ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(JsonModel<bool>), 200)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Mobile: Deleting animal with ID: {id}");
            var result = await _animalService.DeleteAsync(id);
            return StatusCode(result.StatusCode, result);
        }
    }
}