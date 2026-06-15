using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using APNAPASHU.DataContract.Models.Web.Admin.Sidebar;
using APNAPASHU.RepositoryContract.Web.Admin;
using Microsoft.Extensions.Configuration;
using Dapper;
using APNAPASHU.DataContract.Enums;

namespace APNAPASHU.Repository.Web.Admin
{
    public class AdminSidebarRepository : BaseRepository, IAdminSidebarRepository
    {
        public AdminSidebarRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<SidebarModuleModel>> GetAdminSidebarMenuAsync(int userId)
        {
            try
            {
                var query = "[dbo].[usp_GetAdminSidebarMenu]";
                
                var parameters = new DynamicParameters();
                parameters.Add("@UserId", userId);

                var (modules, screens) = await QueryMultipleAsync<SidebarModuleModel, SidebarScreenModel>(
                    query, 
                    parameters, 
                    CommandType.StoredProcedure, 
                    DataBaseNameEnum.APNAPASHU);

                var moduleList = modules?.ToList() ?? new List<SidebarModuleModel>();
                var screenList = screens?.ToList() ?? new List<SidebarScreenModel>();

                foreach (var module in moduleList)
                {
                    module.SubItems = screenList.Where(s => s.ModuleId == module.Id).OrderBy(s => s.DisplayOrder).ToList();
                }

                return moduleList;
            }
            catch (Exception)
            {
                // In a real scenario, log the exception here
                throw;
            }
        }
    }
}
