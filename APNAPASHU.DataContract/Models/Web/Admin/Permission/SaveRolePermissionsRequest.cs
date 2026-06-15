using System.Collections.Generic;

namespace APNAPASHU.DataContract.Models.Web.Admin.Permission
{
    public class SaveRolePermissionsRequest
    {
        public int RoleId { get; set; }
        public List<PermissionToggleModel> ModulePermissions { get; set; } = new List<PermissionToggleModel>();
        public List<PermissionToggleModel> ScreenPermissions { get; set; } = new List<PermissionToggleModel>();
        public List<PermissionToggleModel> ActionPermissions { get; set; } = new List<PermissionToggleModel>();
    }

    public class PermissionToggleModel
    {
        public int Id { get; set; }
        public bool IsGranted { get; set; }
    }
}
