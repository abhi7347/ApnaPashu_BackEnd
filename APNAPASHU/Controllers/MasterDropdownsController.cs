using APNAPASHU.DataContract.Models.Web.Categories;
using APNAPASHU.DataContract.Models;
using Microsoft.AspNetCore.Mvc;
using APNAPASHU.ServiceContract;

namespace APNAPASHU.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterDropdownsController : BaseController
    {
        private readonly IMasterDropdownsService _masterDropdownsService;
        private readonly ILogger<MasterDropdownsController> _logger;

        public MasterDropdownsController(
            IMasterDropdownsService masterDropdownsService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            ILogger<MasterDropdownsController> logger)
            : base(httpContextAccessor, configuration)
        {
            _masterDropdownsService = masterDropdownsService;
            _logger = logger;
        }

        /// <summary>
        /// Get all categories dropdowns values
        /// </summary>
        [HttpGet("get-category-dropdowns")]
        [ProducesResponseType(typeof(JsonModel<List<CatetoryResponseModel>>), 200)]
        public async Task<IActionResult> GetAllCategoryDropdowns()
        {
            var result = await _masterDropdownsService.GetCategoriesDropDowns();
            return Ok(result);
        }
    }
}
