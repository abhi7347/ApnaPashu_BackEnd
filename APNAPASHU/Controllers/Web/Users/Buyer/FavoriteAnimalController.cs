using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.DataContract.Models.Web.Buyer.FavoriteAnimal;
using APNAPASHU.ServiceContract.Web.Buyer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace APNAPASHU.API.Controllers.Web.Buyer
{
    [Route("api/web/buyer/[controller]")]
    [ApiController]
    public class FavoriteAnimalController : BaseController
    {
        private readonly IFavoriteAnimalService _service;

        public FavoriteAnimalController(
            IFavoriteAnimalService service,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _service = service;
        }

        [HttpGet("get-all")]
        [Authorize]
        [ProducesResponseType(typeof(JsonModel<List<BrowseAnimalResponseModel>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] FavoriteAnimalFilterDto filter)
        {
            int userId = GetAuthenticatedUserId();
            filter.UserId = userId; // Securely map from token

            var result = await _service.GetFavoriteAnimalsAsync(filter);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }
    }
}
