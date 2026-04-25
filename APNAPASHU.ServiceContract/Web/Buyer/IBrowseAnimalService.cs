using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.DataContract.Models;

namespace APNAPASHU.ServiceContract.Web.Buyer
{
    public interface IBrowseAnimalService
    {
        Task<JsonModel<List<BrowseAnimalResponseModel>>> BrowseAnimalsAsync(BrowseAnimalFilterDto filter);
    }
}
