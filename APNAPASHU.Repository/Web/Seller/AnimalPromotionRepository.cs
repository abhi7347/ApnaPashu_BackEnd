using System.Data;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Seller.PostedAnimal;
using APNAPASHU.Repository.Data;
using APNAPASHU.RepositoryContract.Web.Seller;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace APNAPASHU.Repository.Web.Seller
{
    public class AnimalPromotionRepository : BaseRepository, IAnimalPromotionRepository
    {
        public AnimalPromotionRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<SqlResponseModel> AddPromotionAsync(AnimalPromotionUpsertModel model, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AnimalId", model.AnimalId);
            parameters.Add("@UserId", userId);
            parameters.Add("@PlanId", model.PlanId);
            parameters.Add("@DurationDays", model.DurationDays);
            parameters.Add("@AmountPaid", model.AmountPaid);

            // We will use a stored procedure for this end-to-end logic as per user requirement
            return await AddAsync<SqlResponseModel>(
                "usp_AddAnimalPromotion",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<List<AnimalPromotionResponseModel>> GetPromotionHistoryAsync(int animalId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@AnimalId", animalId);

            return await GetAsyncList<AnimalPromotionResponseModel>(
                "usp_GetAnimalPromotionHistory",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }
    }
}
