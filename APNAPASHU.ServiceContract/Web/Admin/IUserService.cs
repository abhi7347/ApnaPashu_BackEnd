using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.User;

namespace APNAPASHU.ServiceContract.Web.Admin
{
    public interface IUserService
    {
        Task<JsonModel<List<UserResponseModel>>> GetAllAsync(FilterDto filterDto);
        Task<JsonModel<UserResponseModel>> GetByIdAsync(int id);
        Task<JsonModel<object>> UpsertAsync(UserUpsertModel model, int userId);
        Task<JsonModel<object>> UpdateStatusAsync(UpdateStatusDto model);
        Task<JsonModel<object>> DeleteAsync(List<int> ids, int userId);
    }
}
