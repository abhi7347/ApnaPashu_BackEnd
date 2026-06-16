using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Roles;

namespace APNAPASHU.RepositoryContract.Web.Admin
{
    public interface IRoleRepository
    {
        Task<List<RoleResponseModel>> GetAllAsync(FilterDto filter);
        Task<RoleResponseModel> GetByIdAsync(int id);
        Task<SqlResponseModel> UpsertAsync(RoleUpsertModel model, int userId);
        Task<SqlResponseModel> UpdateStatusAsync(UpdateStatusDto model);
        Task<SqlResponseModel> DeleteAsync(string ids, int userId);
    }
}
