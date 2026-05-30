using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.ServiceContract.Web.Buyer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace APNAPASHU.API.Controllers.Web.Buyer
{
    [Route("api/web/buyer/[controller]")]
    [ApiController]
    public class BrowseAnimalController : BaseController
    {
        private readonly IBrowseAnimalService _service;

        public BrowseAnimalController(
            IBrowseAnimalService service,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _service = service;
        }

        [HttpPost("search")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonModel<List<BrowseAnimalResponseModel>>), 200)]
        public async Task<IActionResult> Search([FromBody] BrowseAnimalFilterDto filter)
        {
            int userId = GetAuthenticatedUserId();
            if (userId != 0)
            {
                filter.UserId = userId;
            }
            var result = await _service.BrowseAnimalsAsync(filter);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpPost("toggle-favorite-animal")]
        [Authorize]
        [ProducesResponseType(typeof(JsonModel<List<BrowseAnimalResponseModel>>), 200)]
        public async Task<IActionResult> ToggleFavoriteAnimal(int animalId)
        {
            int userId = GetAuthenticatedUserId();

            var result = await _service.ToggleFavoritesAnimal(animalId, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(JsonModel<AnimalDetailsResponseModel>), 200)]
        public async Task<IActionResult> GetById(int id)
        {
            int userId = GetAuthenticatedUserId();
            var result = await _service.GetAnimalDetailsByIdAsync(id, userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpGet("my-inquiries")]
        [Authorize]
        [ProducesResponseType(typeof(JsonModel<List<BuyerInquiryResponseModel>>), 200)]
        public async Task<IActionResult> GetMyInquiries([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            int userId = GetAuthenticatedUserId();
            if (userId == 0) return Unauthorized();

            var result = await _service.GetMyInquiriesAsync(userId, pageNumber, pageSize);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }

        [HttpGet("recently-viewed")]
        [Authorize]
        [ProducesResponseType(typeof(JsonModel<List<BrowseAnimalResponseModel>>), 200)]
        public async Task<IActionResult> GetRecentlyViewed()
        {
            int userId = GetAuthenticatedUserId();
            if (userId == 0) return Unauthorized();

            var result = await _service.GetRecentlyViewedAnimalsAsync(userId);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }
    }
}
