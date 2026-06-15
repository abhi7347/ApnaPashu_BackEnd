using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Permission;
using APNAPASHU.RepositoryContract.Web.Admin;
using Microsoft.Extensions.Configuration;
using Dapper;
using APNAPASHU.DataContract.Enums;
using System.Text.Json;

namespace APNAPASHU.Repository.Web.Admin
{
    public class PermissionRepository : BaseRepository, IPermissionRepository
    {
        public PermissionRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<RolePermissionModuleModel>> GetRolePermissionsAsync(int roleId)
        {
            try
            {
                var query = "[dbo].[usp_GetRolePermissions]";
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", roleId);

                var (modules, screens, actions) = await QueryMultipleAsync<RolePermissionModuleModel, RolePermissionScreenModel, RolePermissionActionModel>(
                    query, 
                    parameters, 
                    CommandType.StoredProcedure, 
                    DataBaseNameEnum.APNAPASHU);

                var moduleList = modules?.ToList() ?? new List<RolePermissionModuleModel>();
                var screenList = screens?.ToList() ?? new List<RolePermissionScreenModel>();
                var actionList = actions?.ToList() ?? new List<RolePermissionActionModel>();

                foreach (var screen in screenList)
                {
                    screen.Actions = actionList.Where(a => a.ScreenId == screen.ScreenId).ToList();
                }

                foreach (var module in moduleList)
                {
                    module.Screens = screenList.Where(s => s.ModuleId == module.ModuleId).ToList();
                }

                return moduleList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<SqlResponseModel> SaveRolePermissionsAsync(SaveRolePermissionsRequest request, int userId)
        {
            try
            {
                var query = "[dbo].[usp_SaveRolePermissions]";
                var parameters = new DynamicParameters();
                parameters.Add("@RoleId", request.RoleId);
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                parameters.Add("@ModulePermissionsJson", JsonSerializer.Serialize(request.ModulePermissions, options));
                parameters.Add("@ScreenPermissionsJson", JsonSerializer.Serialize(request.ScreenPermissions, options));
                parameters.Add("@ActionPermissionsJson", JsonSerializer.Serialize(request.ActionPermissions, options));
                parameters.Add("@UserId", userId);

                var response = await GetFirstOrDefaultAsync<SqlResponseModel>(query, parameters, CommandType.StoredProcedure, DataBaseNameEnum.APNAPASHU);
                return response ?? new SqlResponseModel { StatusCode = "500", Message = "Error saving permissions" };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
