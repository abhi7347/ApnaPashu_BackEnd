using System.Collections.Generic;
using System.Threading.Tasks;
using APNAPASHU.DataContract.Models.Web.Admin.Sidebar;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.ServiceContract.Web.Admin;

namespace APNAPASHU.Service.Web.Admin
{
    public class AdminSidebarService : IAdminSidebarService
    {
        private readonly IAdminSidebarRepository _adminSidebarRepository;

        public AdminSidebarService(IAdminSidebarRepository adminSidebarRepository)
        {
            _adminSidebarRepository = adminSidebarRepository;
        }

        public async Task<List<SidebarModuleModel>> GetAdminSidebarMenuAsync(int userId)
        {
            return await _adminSidebarRepository.GetAdminSidebarMenuAsync(userId);
        }
    }
}
