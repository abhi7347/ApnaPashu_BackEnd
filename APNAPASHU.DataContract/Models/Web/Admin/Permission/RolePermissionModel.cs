using System.Collections.Generic;

namespace APNAPASHU.DataContract.Models.Web.Admin.Permission
{
    public class RolePermissionModuleModel
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }
        public string ModuleKey { get; set; }
        public bool IsGranted { get; set; }
        public List<RolePermissionScreenModel> Screens { get; set; } = new List<RolePermissionScreenModel>();
    }

    public class RolePermissionScreenModel
    {
        public int ScreenId { get; set; }
        public int ModuleId { get; set; }
        public string ScreenName { get; set; }
        public string ScreenKey { get; set; }
        public bool IsGranted { get; set; }
        public List<RolePermissionActionModel> Actions { get; set; } = new List<RolePermissionActionModel>();
    }

    public class RolePermissionActionModel
    {
        public int ActionId { get; set; }
        public int ScreenId { get; set; }
        public string ActionName { get; set; }
        public string ActionKey { get; set; }
        public bool IsGranted { get; set; }
    }
}
