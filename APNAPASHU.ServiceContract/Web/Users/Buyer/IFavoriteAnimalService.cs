using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.DataContract.Models.Web.Buyer.FavoriteAnimal;

namespace APNAPASHU.ServiceContract.Web.Buyer
{
    public interface IFavoriteAnimalService
    {
        Task<JsonModel<List<BrowseAnimalResponseModel>>> GetFavoriteAnimalsAsync(FavoriteAnimalFilterDto filter);
    }
}
