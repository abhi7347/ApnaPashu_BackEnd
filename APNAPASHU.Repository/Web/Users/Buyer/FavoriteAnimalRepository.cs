using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.DataContract.Models.Web.Buyer.FavoriteAnimal;
using APNAPASHU.Repository.Data;
using APNAPASHU.RepositoryContract.Web.Buyer;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace APNAPASHU.Repository.Web.Buyer
{
    public class FavoriteAnimalRepository : BaseRepository, IFavoriteAnimalRepository
    {
        public FavoriteAnimalRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<BrowseAnimalResponseModel>> GetFavoriteAnimalsAsync(FavoriteAnimalFilterDto filterDto)
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@UserId", filterDto.UserId, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@PageNumber", filterDto.PageNumber, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@PageSize", filterDto.PageSize, DbType.Int32, ParameterDirection.Input);

            var result = await GetAsyncList<BrowseAnimalResponseModel>(
                "[dbo].[usp_Buyer_GetFavoriteAnimals]",
                parameter,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result;
        }
    }
}
