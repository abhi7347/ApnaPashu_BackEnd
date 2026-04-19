using APNAPASHU.DataContract.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using System.Net;
using APNAPASHU.DataContract.Models.Web.Seller.PostedAnimal;
using APNAPASHU.ServiceContract.Web.Seller;

namespace APNAPASHU.API.Controllers.Web.Seller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostedAnimalController : BaseController
    {
        private readonly IPostedAnimalService _service;
        private readonly ILogger<PostedAnimalController> _logger;

        public PostedAnimalController(
            IPostedAnimalService service,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ILogger<PostedAnimalController> logger)
            : base(httpContextAccessor, configuration)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(JsonModel<List<PostedAnimalResponseModel>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] FilterDto filter)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _service.GetAllAsync(filter, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonModel<PostedAnimalResponseModel>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpPost("upsert")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Upsert([FromForm] PostedAnimalUpsertModel model)
        {
            int userId = GetAuthenticatedUserId();

            var result = await _service.UpsertAsync(model, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }



        [HttpPost("delete")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> Delete([FromBody] List<int> ids)
        {
            int userId = GetAuthenticatedUserId();

            var result = await _service.DeleteAsync(ids, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }

        [HttpPost("add-promotion")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> AddPromotion([FromBody] AnimalPromotionUpsertModel model)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _service.AddPromotionAsync(model, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }

        [HttpGet("promotion-history/{animalId}")]
        [ProducesResponseType(typeof(JsonModel<List<AnimalPromotionResponseModel>>), 200)]
        public async Task<IActionResult> GetPromotionHistory(int animalId)
        {
            var result = await _service.GetPromotionHistoryAsync(animalId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpPost("update-sold-status/{id}")]
        [ProducesResponseType(typeof(JsonModel<object>), 200)]
        public async Task<IActionResult> UpdateSoldStatus(int id, [FromQuery] bool isSold)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _service.UpdateSoldStatusAsync(id, isSold, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.BadRequest, result);
        }
    }
}
