using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Seller.PostedAnimal;

namespace APNAPASHU.ServiceContract.Web.Seller
{
    public interface IPostedAnimalService
    {
        Task<JsonModel<List<PostedAnimalResponseModel>>> GetAllAsync(FilterDto filterDto, int userId);
        Task<JsonModel<PostedAnimalResponseModel>> GetByIdAsync(int id);
        Task<JsonModel<object>> UpsertAsync(PostedAnimalUpsertModel model, int userId);
        Task<JsonModel<object>> DeleteAsync(List<int> ids, int userId);
        Task<JsonModel<object>> AddPromotionAsync(AnimalPromotionUpsertModel model, int userId);
        Task<JsonModel<List<AnimalPromotionResponseModel>>> GetPromotionHistoryAsync(int animalId);
        Task<JsonModel<object>> UpdateSoldStatusAsync(int id, bool isSold, int userId);
    }
}
