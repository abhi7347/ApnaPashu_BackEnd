using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;

namespace APNAPASHU.RepositoryContract.Web.Buyer
{
    public interface IBrowseAnimalRepository
    {
        Task<List<BrowseAnimalResponseModel>> BrowseAnimalsAsync(BrowseAnimalFilterDto filterDto);

        Task<SqlResponseModel> ToggleFavoritesAnimal(int animalId, int userId);
        
    }

}
