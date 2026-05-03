using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.DataContract.Models.Web.Buyer.FavoriteAnimal;

namespace APNAPASHU.RepositoryContract.Web.Buyer
{
    public interface IFavoriteAnimalRepository
    {
        Task<List<BrowseAnimalResponseModel>> GetFavoriteAnimalsAsync(FavoriteAnimalFilterDto filterDto);
    }
}
