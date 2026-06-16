using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Roles;

namespace APNAPASHU.ServiceContract.Web.Admin
{
    public interface IRoleService
    {
        Task<JsonModel<List<RoleResponseModel>>> GetAllAsync(FilterDto filter);
        Task<JsonModel<RoleResponseModel>> GetByIdAsync(int id);
        Task<JsonModel<object>> UpsertAsync(RoleUpsertModel model, int userId);
        Task<JsonModel<object>> UpdateStatusAsync(UpdateStatusDto model);
        Task<JsonModel<object>> DeleteAsync(string ids, int userId);
    }
}
