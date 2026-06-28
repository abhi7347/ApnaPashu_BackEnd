using Dapper;
using System.Data;
using Microsoft.Extensions.Configuration;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models.Web.Admin.Categories;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace APNAPASHU.Repository.Web.Admin
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(IConfiguration configuration, AppDbContext context) : base(configuration)
        {
            _context = context;
        }

        public async Task<List<CategoryResponseModel>> GetAllAsync(FilterDto filterDto)
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@PageNumber", filterDto.PageNumber, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@PageSize", filterDto.PageSize, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@SearchTerm", filterDto.SearchTerm, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortColumns", filterDto.SortCulumn, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortDirection", filterDto.SortDirection, DbType.String, ParameterDirection.Input);

            return await GetAsyncList<CategoryResponseModel>(
                "[dbo].[usp_GetAllCategories]",
                parameter,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<CategoryResponseModel> GetByIdAsync(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var result = await GetAsyncList<CategoryResponseModel>(
                "[dbo].[usp_Category_GetById]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result.FirstOrDefault()!;
        }

        public async Task<List<CategoryResponseModel>> GetByIdsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any()) return new List<CategoryResponseModel>();
            
            var query = await _context.Categories
                .Where(x => x.Id.HasValue && ids.Contains(x.Id.Value))
                .Select(x => new CategoryResponseModel
                {
                    Id = x.Id ?? 0,
                    CategoryName = x.CategoryName,
                    Description = x.Description,
                    ImagePath = x.ImagePath,
                    IsActive = x.IsActive
                }).ToListAsync();
                
            return query;
        }

        public async Task<SqlResponseModel> UpsertAsync(CategoryUpsertModel model, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@CategoryName", model.CategoryName);
            parameters.Add("@Description", model.Description);
            parameters.Add("@ImagePath", model.ImagePath);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@UserId", userId);

            return await AddAsync<SqlResponseModel>(
                "[dbo].[usp_Category_Upsert]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<SqlResponseModel> UpdateStatusAsync(UpdateStatusDto model)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (category != null)
            {
                category.IsActive = model.Status;
                category.UpdatedBy = model.UserId;
                category.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                
                return new SqlResponseModel { StatusCode = "SUCCESS", Message = "Category status updated successfully" };
            }
            
            return new SqlResponseModel { StatusCode = "ERROR", Message = "Category not found" };
        }

        public async Task<SqlResponseModel> DeleteAsync(string ids, int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Ids", ids);
            parameters.Add("@UserId", userId);

            return await UpdateAsync<SqlResponseModel>(
                "[dbo].[usp_Category_MultiDelete]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }
    }
}
