using APNAPASHU.DataContract.Models.Web.Seller.Dashboard;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.Repository.Data;
using APNAPASHU.RepositoryContract.Web.Seller;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace APNAPASHU.Repository.Web.Seller
{
    public class SellerDashboardRepository : BaseRepository, ISellerDashboardRepository
    {
        public SellerDashboardRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<SellerDashboardResponseModel> GetDashboardStatsAsync(int userId)
        {
            var response = new SellerDashboardResponseModel();
            
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var result = await QueryMultipleAsync<SellerStatsModel, RecentLeadModel>(
                "usp_Seller_GetDashboardStats",
                parameters,
                CommandType.StoredProcedure
            );

            response.Stats = result.Item1?.FirstOrDefault() ?? new SellerStatsModel();
            response.RecentLeads = result.Item2?.ToList() ?? new System.Collections.Generic.List<RecentLeadModel>();

            return response;
        }
    }
}
