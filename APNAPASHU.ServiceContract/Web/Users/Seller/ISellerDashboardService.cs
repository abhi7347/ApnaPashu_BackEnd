using System.Threading.Tasks;
using APNAPASHU.DataContract.Models.Web.Seller.Dashboard;

namespace APNAPASHU.ServiceContract.Web.Seller
{
    public interface ISellerDashboardService
    {
        Task<SellerDashboardResponseModel> GetDashboardStatsAsync(int userId);
    }
}
