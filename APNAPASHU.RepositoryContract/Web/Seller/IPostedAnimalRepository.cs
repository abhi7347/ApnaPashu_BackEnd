using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Seller.PostedAnimal;

namespace APNAPASHU.RepositoryContract.Web.Seller
{
    public interface IPostedAnimalRepository
    {
        Task<List<PostedAnimalResponseModel>> GetAllAsync(FilterDto filterDto, int userId);
        Task<PostedAnimalResponseModel> GetByIdAsync(int id);
        Task<List<PostedAnimalResponseModel>> GetByIdsAsync(List<int> ids);
        Task<SqlResponseModel> UpsertAsync(PostedAnimalUpsertModel model, int userId);
        Task<SqlResponseModel> DeleteAsync(List<int> ids, int userId);
    }
}
