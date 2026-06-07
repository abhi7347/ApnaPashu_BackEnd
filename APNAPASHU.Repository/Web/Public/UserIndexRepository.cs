#nullable disable
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.DataContract.Models.Web.Public.UserIndex;
using APNAPASHU.Repository.Data;
using APNAPASHU.RepositoryContract.Web.Public;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace APNAPASHU.Repository.Web.Public
{
    public class UserIndexRepository : BaseRepository, IUserIndexRepository
    {
        public UserIndexRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<UserIndexResponseModel> GetIndexAnimalsAsync()
        {
            var response = new UserIndexResponseModel();

            var (featured, recent) = await QueryMultipleAsync<BrowseAnimalResponseModel, BrowseAnimalResponseModel>(
                "[dbo].[usp_Public_GetIndexAnimals]",
                null,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            response.FeaturedAnimals = featured?.ToList() ?? new List<BrowseAnimalResponseModel>();
            response.RecentAnimals = recent?.ToList() ?? new List<BrowseAnimalResponseModel>();

            return response;
        }
    }
}
