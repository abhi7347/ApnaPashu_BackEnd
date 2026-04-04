using APNAPASHU.DataContract.Models;
using APNAPASHU.RepositoryContract;

namespace APNAPASHU.ServiceContract
{
    public class MasterDropdownsService : IMasterDropdownsService
    {

        private readonly IMasterDropdownsRepository _masterDropdownsRepo;

        public MasterDropdownsService(IMasterDropdownsRepository masterDropdownsRepo)
        {
            _masterDropdownsRepo = masterDropdownsRepo;
        }

        public async Task<List<MasterDropdownsModels>> GetCategoriesDropDowns()
        {
            return await _masterDropdownsRepo.GetCategoriesDropDowns();
        }
    }
}
