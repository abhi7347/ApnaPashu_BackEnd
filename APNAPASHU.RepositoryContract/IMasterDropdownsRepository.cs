using APNAPASHU.DataContract.Models;

namespace APNAPASHU.RepositoryContract
{
    public interface IMasterDropdownsRepository
    {
        public Task<List<MasterDropdownsModels>> GetCategoriesDropDowns();
    }
}
