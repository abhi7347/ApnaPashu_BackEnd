using System.Threading.Tasks;
using APNAPASHU.DataContract.Models.Web.Seller.Dashboard;

namespace APNAPASHU.RepositoryContract.Web.Seller
{
    public interface ISellerDashboardRepository
    {
        Task<SellerDashboardResponseModel> GetDashboardStatsAsync(int userId);
    }
}
