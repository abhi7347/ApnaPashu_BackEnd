using System.Collections.Generic;
using System.Threading.Tasks;
using APNAPASHU.DataContract.Models.Web.Admin.Sidebar;

namespace APNAPASHU.ServiceContract.Web.Admin
{
    public interface IAdminSidebarService
    {
        Task<List<SidebarModuleModel>> GetAdminSidebarMenuAsync(int userId);
    }
}
