using APNAPASHU.DataContract.Models;

namespace APNAPASHU.ServiceContract
{
    public interface IMasterDropdownsService
    {
        public Task<List<MasterDropdownsModels>> GetCategoriesDropDowns();
        public Task<List<MasterDropdownsModels>> GetRolesDropDowns();

    }
}
