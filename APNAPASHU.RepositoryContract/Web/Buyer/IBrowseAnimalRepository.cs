using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;

namespace APNAPASHU.RepositoryContract.Web.Buyer
{
    public interface IBrowseAnimalRepository
    {
        Task<List<BrowseAnimalResponseModel>> BrowseAnimalsAsync(BrowseAnimalFilterDto filterDto);
    }
}
