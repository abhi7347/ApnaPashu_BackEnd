#nullable disable
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.Repository.Data;
using APNAPASHU.RepositoryContract.Web.Buyer;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace APNAPASHU.Repository.Web.Buyer
{
    public class BrowseAnimalRepository : BaseRepository, IBrowseAnimalRepository
    {
        private readonly AppDbContext _context;
        public BrowseAnimalRepository(IConfiguration configuration, AppDbContext context) : base(configuration)
        {
            _context = context;    
        }

        public async Task<List<BrowseAnimalResponseModel>> BrowseAnimalsAsync(BrowseAnimalFilterDto filterDto)
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@UserId", filterDto.UserId, DbType.Int32, ParameterDirection.Input);
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

        public async Task<SqlResponseModel> ToggleFavoritesAnimal(int animalId, int userId)
        {

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@AnimalId", animalId);
            parameters.Add("@UserId", userId);

            return await UpdateAsync<SqlResponseModel>(
                "usp_ToggleFavoriteAnimal",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<AnimalDetailsResponseModel> GetAnimalDetailsByIdAsync(int id, int userId)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@BuyerUserId", userId == 0 ? null : (int?)userId, DbType.Int32, ParameterDirection.Input);

            var result = await GetAsyncList<AnimalDetailsResponseModel>(
                "[dbo].[usp_Buyer_GetAnimalDetailsById]",
                parameter,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result.FirstOrDefault();
        }

        public async Task<List<BuyerInquiryResponseModel>> GetMyInquiriesAsync(int userId, int pageNumber, int pageSize)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@PageNumber", pageNumber, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@PageSize", pageSize, DbType.Int32, ParameterDirection.Input);

                var result = await GetAsyncList<BuyerInquiryResponseModel>(
                    "[dbo].[usp_Get_UserConversations]",
                    parameter,
                    CommandType.StoredProcedure,
                    DataBaseNameEnum.APNAPASHU
                );

                return result ?? new List<BuyerInquiryResponseModel>();
            }
            catch
            {
                return new List<BuyerInquiryResponseModel>();
            }
        }

        public async Task SaveRecentViewAsync(int userId, int animalId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);
                parameter.Add("@AnimalId", animalId, DbType.Int32, ParameterDirection.Input);

                await AddAsync(
                    "[dbo].[usp_Save_UserRecentView]",
                    parameter,
                    CommandType.StoredProcedure,
                    DataBaseNameEnum.APNAPASHU
                );
            }
            catch
            {
                // Silently ignore tracking errors
            }
        }

        public async Task<List<BrowseAnimalResponseModel>> GetRecentlyViewedAnimalsAsync(int userId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);

                var result = await GetAsyncList<BrowseAnimalResponseModel>(
                    "[dbo].[usp_Buyer_GetRecentlyViewedAnimals]",
                    parameter,
                    CommandType.StoredProcedure,
                    DataBaseNameEnum.APNAPASHU
                );

                return result ?? new List<BrowseAnimalResponseModel>();
            }
            catch
            {
                return new List<BrowseAnimalResponseModel>();
            }
        }

        public async Task<BuyerDashboardStatsResponseModel> GetDashboardStatsAsync(int userId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);

                var result = await GetAsyncList<BuyerDashboardStatsResponseModel>(
                    "[dbo].[usp_Buyer_GetDashboardStats]",
                    parameter,
                    CommandType.StoredProcedure,
                    DataBaseNameEnum.APNAPASHU
                );

                return result?.FirstOrDefault() ?? new BuyerDashboardStatsResponseModel();
            }
            catch
            {
                return new BuyerDashboardStatsResponseModel();
            }
        }

        public async Task<SqlResponseModel> RequestRoleUpgradeAsync(int userId)
        {
            try
            {
                DynamicParameters parameter = new DynamicParameters();
                parameter.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);

                var result = await GetAsyncList<SqlResponseModel>(
                    "[dbo].[usp_Buyer_RequestRoleUpgrade]",
                    parameter,
                    CommandType.StoredProcedure,
                    DataBaseNameEnum.APNAPASHU
                );

                return result?.FirstOrDefault() ?? new SqlResponseModel { StatusCode = "ERROR", Message = "Something went wrong." };
            }
            catch (Exception ex)
            {
                return new SqlResponseModel { StatusCode = "ERROR", Message = ex.Message };
            }
        }
    }
}
