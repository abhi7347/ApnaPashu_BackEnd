using System.Collections.Generic;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;

namespace APNAPASHU.DataContract.Models.Web.Public.UserIndex
{
    public class UserIndexResponseModel
    {
        public List<BrowseAnimalResponseModel> FeaturedAnimals { get; set; } = new List<BrowseAnimalResponseModel>();
        public List<BrowseAnimalResponseModel> RecentAnimals { get; set; } = new List<BrowseAnimalResponseModel>();
    }
}
