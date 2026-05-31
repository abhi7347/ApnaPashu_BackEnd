using APNAPASHU.DataContract.Models.Web.Seller.Dashboard;
using APNAPASHU.RepositoryContract.Web.Seller;
using APNAPASHU.ServiceContract.Web.Seller;
using System.Threading.Tasks;

namespace APNAPASHU.Service.Web.Seller
{
    public class SellerDashboardService : ISellerDashboardService
    {
        private readonly ISellerDashboardRepository _sellerDashboardRepository;

        public SellerDashboardService(ISellerDashboardRepository sellerDashboardRepository)
        {
            _sellerDashboardRepository = sellerDashboardRepository;
        }

        public async Task<SellerDashboardResponseModel> GetDashboardStatsAsync(int userId)
        {
            return await _sellerDashboardRepository.GetDashboardStatsAsync(userId);
        }
    }
}
