using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Categories;

namespace APNAPASHU.ServiceContract.Web.Admin
{
    public interface ICategoryService
    {
        Task<JsonModel<List<CategoryResponseModel>>> GetAllAsync(FilterDto filter);
        Task<JsonModel<CategoryResponseModel>> GetByIdAsync(int id);
        Task<JsonModel<object>> UpsertAsync(CategoryUpsertModel model, int userId);
        Task<JsonModel<object>> UpdateStatusAsync(UpdateStatusDto model);
        Task<JsonModel<object>> DeleteAsync(List<int> ids, int userId);
    }
}
