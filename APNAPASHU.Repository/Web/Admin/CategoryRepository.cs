#nullable disable
using Dapper;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models.Web.Admin.Categories;
using APNAPASHU.RepositoryContract.Web.Admin;

namespace APNAPASHU.Repository.Web.Admin
{
    /// <summary>
    /// Category Repository Implementation for Web - CRUD Template
    /// </summary>
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(IConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Get all categories with filtering and pagination
        /// </summary>
        public async Task<List<CatetoryResponseModel>> GetAllAsync(FilterDto filterDto)
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@PageNumber", filterDto.PageNumber, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@PageSize", filterDto.PageSize, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@SearchTerm", filterDto .SearchTerm, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortColumns", filterDto.SortCulumn, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortDirection", filterDto.SortDirection, DbType.String, ParameterDirection.Input);

            return await GetAsyncList<CatetoryResponseModel>(
                "[dbo].[usp_GetAllCategories]",
                parameter,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        public async Task<CatetoryResponseModel> GetByIdAsync(int categoryId)
        {
            string query = @"SELECT CategoryId, CategoryName, Description, IconUrl,
                    CreatedDate, UpdatedDate, IsActive
                    FROM Categories";

            var result = await GetAsyncList<CatetoryResponseModel>(
                query,
                null,
                CommandType.Text,
                DataBaseNameEnum.APNAPASHU
            );

            return result.FirstOrDefault(x => x.Id == categoryId);
        }

        /// <summary>
        /// Create category
        /// </summary>
        public async Task<SqlResponseModel> UpsertAsync(CategoryUpsertModel model, int userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@CategoryId", model.Id);
            parameters.Add("@CategoryName", model.CategoryName);
            parameters.Add("@Description", model.Description);
            parameters.Add("@ImagePage", model.ImagePath);
            parameters.Add("@UserId", userId);


            return await AddAsync<SqlResponseModel>(
                "usp_UpsertCategory",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        /// <summary>
        /// Update category status (IsActive)
        /// </summary>
        public async Task<SqlResponseModel> UpdateStatusAsync(UpdateStatusDto model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@IsActive", model.Status ? 1 : 0);
            parameters.Add("@UserId", model.UserId);


            return await UpdateAsync<SqlResponseModel>(
                "dbo.usp_UpdateCategoryStatus",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<SqlResponseModel> DeleteAsync(int categoryId, int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", categoryId);

            return await UpdateAsync<SqlResponseModel>(
                "dbo.usp_DeleteCategory",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

    }
}
