using System.Data;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.RepositoryContract.Web.Buyer;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace APNAPASHU.Repository.Web.Buyer
{
    public class BrowseAnimalRepository : BaseRepository, IBrowseAnimalRepository
    {
        public BrowseAnimalRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<List<BrowseAnimalResponseModel>> BrowseAnimalsAsync(BrowseAnimalFilterDto filterDto)
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@PageNumber", filterDto.PageNumber, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@PageSize", filterDto.PageSize, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@SearchTerm", filterDto.SearchTerm, DbType.String, ParameterDirection.Input);

            string? categoryIds = filterDto.CategoryIds != null && filterDto.CategoryIds.Any() 
                ? string.Join(",", filterDto.CategoryIds) : null;
            parameter.Add("@CategoryIds", categoryIds, DbType.String, ParameterDirection.Input);

            parameter.Add("@MinPrice", filterDto.MinPrice, DbType.Decimal, ParameterDirection.Input);
            parameter.Add("@MaxPrice", filterDto.MaxPrice, DbType.Decimal, ParameterDirection.Input);

            string? ageRanges = filterDto.AgeRanges != null && filterDto.AgeRanges.Any() 
                ? string.Join(",", filterDto.AgeRanges) : null;
            parameter.Add("@AgeRanges", ageRanges, DbType.String, ParameterDirection.Input);

            string? genders = filterDto.Genders != null && filterDto.Genders.Any() 
                ? string.Join(",", filterDto.Genders) : null;
            parameter.Add("@Genders", genders, DbType.String, ParameterDirection.Input);

            parameter.Add("@Location", string.IsNullOrWhiteSpace(filterDto.Location) ? null : filterDto.Location, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortBy", filterDto.SortBy, DbType.String, ParameterDirection.Input);

            var result = await GetAsyncList<BrowseAnimalResponseModel>(
                "[dbo].[usp_Buyer_BrowseAnimals]",
                parameter,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result;
        }
    }
}
