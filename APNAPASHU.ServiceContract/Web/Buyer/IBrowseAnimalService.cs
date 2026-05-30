using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.DataContract.Models;

namespace APNAPASHU.ServiceContract.Web.Buyer
{
    public interface IBrowseAnimalService
    {
        Task<JsonModel<List<BrowseAnimalResponseModel>>> BrowseAnimalsAsync(BrowseAnimalFilterDto filter);
        Task<JsonModel<object>> ToggleFavoritesAnimal(int animalId, int userId);
        Task<JsonModel<AnimalDetailsResponseModel>> GetAnimalDetailsByIdAsync(int id, int userId);
        Task<JsonModel<List<BuyerInquiryResponseModel>>> GetMyInquiriesAsync(int userId, int pageNumber, int pageSize);
        Task<JsonModel<List<BrowseAnimalResponseModel>>> GetRecentlyViewedAnimalsAsync(int userId);
    }
}
