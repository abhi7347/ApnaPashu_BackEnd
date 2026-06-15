using System.Collections.Generic;

namespace APNAPASHU.DataContract.Models.Web.Admin.Sidebar
{
    public class SidebarModuleModel
    {
        public int Id { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleKey { get; set; } = string.Empty;
        public string ModuleIcon { get; set; } = string.Empty;
        public string NavigationLink { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public List<SidebarScreenModel> SubItems { get; set; } = new List<SidebarScreenModel>();
    }

    public class SidebarScreenModel
    {
        public int Id { get; set; }
        public int ModuleId { get; set; }
        public string ScreenName { get; set; } = string.Empty;
        public string ScreenKey { get; set; } = string.Empty;
        public string ScreenIcon { get; set; } = string.Empty;
        public string NavigationLink { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
