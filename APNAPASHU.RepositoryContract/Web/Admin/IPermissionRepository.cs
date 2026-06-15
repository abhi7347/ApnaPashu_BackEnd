using System.Collections.Generic;
using System.Threading.Tasks;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Permission;

namespace APNAPASHU.RepositoryContract.Web.Admin
{
    public interface IPermissionRepository
    {
        Task<List<RolePermissionModuleModel>> GetRolePermissionsAsync(int roleId);
        Task<SqlResponseModel> SaveRolePermissionsAsync(SaveRolePermissionsRequest request, int userId);
    }
}
