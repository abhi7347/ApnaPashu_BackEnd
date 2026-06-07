using APNAPASHU.DataContract.Models;
using APNAPASHU.ServiceContract.Web.Public;
using Microsoft.AspNetCore.Mvc;

namespace APNAPASHU.API.Controllers.Web.Public
{
    [Route("api/web/public/[controller]")]
    [ApiController]
    public class UserIndexController : BaseController
    {
        private readonly IUserIndexService _userIndexService;

        public UserIndexController(
            IUserIndexService userIndexService,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _userIndexService = userIndexService;
        }

        [HttpGet("GetIndexData")]
        public async Task<IActionResult> GetIndexData()
        {
            try
            {
                var result = await _userIndexService.GetIndexAnimalsAsync();
                return Ok(new JsonModel<object> { Data = result, StatusCode = 200, Message = "Success" });
            }
            catch (System.Exception ex)
            {
                return Ok(new JsonModel<object> { Data = null, StatusCode = 500, Message = ex.Message });
            }
        }
    }
}
