using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Seller.PostedAnimal;

namespace APNAPASHU.RepositoryContract.Web.Seller
{
    public interface IAnimalPromotionRepository
    {
        Task<SqlResponseModel> AddPromotionAsync(AnimalPromotionUpsertModel model, int userId);
        Task<List<AnimalPromotionResponseModel>> GetPromotionHistoryAsync(int animalId);
    }
}
