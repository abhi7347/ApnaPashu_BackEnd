using System.Collections.Generic;
using System.Threading.Tasks;
using APNAPASHU.DataContract.Models.Web.Admin.Sidebar;

namespace APNAPASHU.RepositoryContract.Web.Admin
{
    public interface IAdminSidebarRepository
    {
        Task<List<SidebarModuleModel>> GetAdminSidebarMenuAsync(int userId);
    }
}
