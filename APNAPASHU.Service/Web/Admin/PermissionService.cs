using System.Collections.Generic;
using System.Threading.Tasks;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Permission;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.ServiceContract.Web.Admin;

namespace APNAPASHU.Service.Web.Admin
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<List<RolePermissionModuleModel>> GetRolePermissionsAsync(int roleId)
        {
            return await _permissionRepository.GetRolePermissionsAsync(roleId);
        }

        public async Task<SqlResponseModel> SaveRolePermissionsAsync(SaveRolePermissionsRequest request, int userId)
        {
            return await _permissionRepository.SaveRolePermissionsAsync(request, userId);
        }
    }
}
