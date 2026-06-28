using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Categories;

namespace APNAPASHU.RepositoryContract.Web.Admin
{
    public interface ICategoryRepository
    {
        Task<List<CategoryResponseModel>> GetAllAsync(FilterDto filterDto);
        Task<CategoryResponseModel> GetByIdAsync(int id);
        Task<List<CategoryResponseModel>> GetByIdsAsync(List<int> ids);
        Task<SqlResponseModel> UpsertAsync(CategoryUpsertModel model, int userId);
        Task<SqlResponseModel> UpdateStatusAsync(UpdateStatusDto model);
        Task<SqlResponseModel> DeleteAsync(string ids, int userId);
    }
}
