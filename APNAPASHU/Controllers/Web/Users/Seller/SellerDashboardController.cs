using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Seller.Dashboard;
using APNAPASHU.ServiceContract.Web.Seller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace APNAPASHU.Controllers.Web.Seller
{
    [Route("api/web/seller/dashboard")]
    [ApiController]
    [Authorize]
    public class SellerDashboardController : ControllerBase
    {
        private readonly ISellerDashboardService _sellerDashboardService;

        public SellerDashboardController(ISellerDashboardService sellerDashboardService)
        {
            _sellerDashboardService = sellerDashboardService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            var result = await _sellerDashboardService.GetDashboardStatsAsync(userId);
            
            return Ok(new JsonModel<SellerDashboardResponseModel>
            {
                Data = result,
                Message = "Stats fetched successfully",
                StatusCode = 200
            });
        }
    }
}
