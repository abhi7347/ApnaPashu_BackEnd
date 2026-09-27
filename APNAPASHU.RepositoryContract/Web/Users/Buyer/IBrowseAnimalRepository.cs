using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;

namespace APNAPASHU.RepositoryContract.Web.Buyer
{
    public interface IBrowseAnimalRepository
    {
        Task<List<BrowseAnimalResponseModel>> BrowseAnimalsAsync(BrowseAnimalFilterDto filterDto);

        Task<SqlResponseModel> ToggleFavoritesAnimal(int animalId, int userId);
        Task<AnimalDetailsResponseModel> GetAnimalDetailsByIdAsync(int id, int userId);
        Task<List<BuyerInquiryResponseModel>> GetMyInquiriesAsync(int userId, int pageNumber, int pageSize);
        
        Task SaveRecentViewAsync(int userId, int animalId);
        Task<List<BrowseAnimalResponseModel>> GetRecentlyViewedAnimalsAsync(int userId);
        Task<BuyerDashboardStatsResponseModel> GetDashboardStatsAsync(int userId);
        Task<SqlResponseModel> RequestRoleUpgradeAsync(int userId);
    }
}
