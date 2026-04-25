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
            var result = await _service.BrowseAnimalsAsync(filter);
            return StatusCode(result.StatusCode ?? (int)HttpStatusCode.OK, result);
        }
    }
}
