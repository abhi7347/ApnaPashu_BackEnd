using Dapper;
using APNAPASHU.DataContract.Models.Category;
using APNAPASHU.RepositoryContract.Web;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace APNAPASHU.Repository.Web
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
        public async Task<List<CategoryResponseDto>> GetAllAsync(CategoryFilterDto filterDto)
        {
            try
            {
                int offset = (filterDto.PageNumber - 1) * filterDto.PageSize;
                string query = @"
                    SELECT CategoryId, CategoryName, Description, IconUrl, 
                           CreatedDate, UpdatedDate, IsActive
                    FROM Categories
                    WHERE 1=1";

                // Add IsActive filter
                if (filterDto.IsActive.HasValue)
                    query += " AND IsActive = @IsActive";

                // Add search filter
                if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
                    query += @" AND (CategoryName LIKE '%' + @SearchTerm + '%' 
                             OR Description LIKE '%' + @SearchTerm + '%')";

                query += @" ORDER BY CategoryName
                           OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    if (filterDto.IsActive.HasValue)
                        parameters.Add("@IsActive", filterDto.IsActive.Value ? 1 : 0);
                    if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
                        parameters.Add("@SearchTerm", filterDto.SearchTerm);
                    parameters.Add("@Offset", offset);
                    parameters.Add("@PageSize", filterDto.PageSize);

                    var result = await conn.QueryAsync<CategoryResponseDto>(query, parameters);
                    return result.ToList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving categories: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get category by ID
        /// </summary>
        public async Task<CategoryResponseDto> GetByIdAsync(int categoryId)
        {
            try
            {
                string query = @"
                    SELECT CategoryId, CategoryName, Description, IconUrl, 
                           CreatedDate, UpdatedDate, IsActive
                    FROM Categories
                    WHERE CategoryId = @CategoryId";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@CategoryId", categoryId);

                    return await conn.QueryFirstOrDefaultAsync<CategoryResponseDto>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving category: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Create category
        /// </summary>
        public async Task<int> CreateAsync(CategoryUpsertDto upsertDto)
        {
            try
            {
                string query = @"
                    INSERT INTO Categories (CategoryName, Description, IconUrl, CreatedDate, UpdatedDate, IsActive)
                    VALUES (@CategoryName, @Description, @IconUrl, @CreatedDate, @UpdatedDate, 1);
                    SELECT CAST(SCOPE_IDENTITY() as int)";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@CategoryName", upsertDto.CategoryName);
                    parameters.Add("@Description", upsertDto.Description ?? string.Empty);
                    parameters.Add("@IconUrl", upsertDto.IconUrl ?? string.Empty);
                    parameters.Add("@CreatedDate", DateTime.UtcNow);
                    parameters.Add("@UpdatedDate", DateTime.UtcNow);

                    return await conn.ExecuteScalarAsync<int>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating category: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update category (full properties)
        /// </summary>
        public async Task<bool> UpdateAsync(CategoryUpsertDto upsertDto)
        {
            try
            {
                string query = @"
                    UPDATE Categories
                    SET CategoryName = @CategoryName,
                        Description = @Description,
                        IconUrl = @IconUrl,
                        UpdatedDate = @UpdatedDate
                    WHERE CategoryId = @CategoryId";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@CategoryId", upsertDto.CategoryId);
                    parameters.Add("@CategoryName", upsertDto.CategoryName);
                    parameters.Add("@Description", upsertDto.Description ?? string.Empty);
                    parameters.Add("@IconUrl", upsertDto.IconUrl ?? string.Empty);
                    parameters.Add("@UpdatedDate", DateTime.UtcNow);

                    int rowsAffected = await conn.ExecuteAsync(query, parameters);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating category: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Update category status (IsActive)
        /// </summary>
        public async Task<bool> UpdateStatusAsync(CategoryStatusUpdateDto statusDto)
        {
            try
            {
                string query = @"
                    UPDATE Categories
                    SET IsActive = @IsActive, UpdatedDate = @UpdatedDate
                    WHERE CategoryId = @CategoryId";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@CategoryId", statusDto.CategoryId);
                    parameters.Add("@IsActive", statusDto.IsActive ? 1 : 0);
                    parameters.Add("@UpdatedDate", DateTime.UtcNow);

                    int rowsAffected = await conn.ExecuteAsync(query, parameters);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating status: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete category (soft delete - sets IsActive to 0)
        /// </summary>
        public async Task<bool> DeleteAsync(int categoryId)
        {
            try
            {
                string query = @"
                    UPDATE Categories
                    SET IsActive = 0, UpdatedDate = @UpdatedDate
                    WHERE CategoryId = @CategoryId";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@CategoryId", categoryId);
                    parameters.Add("@UpdatedDate", DateTime.UtcNow);

                    int rowsAffected = await conn.ExecuteAsync(query, parameters);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting category: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Get total count with filter
        /// </summary>
        public async Task<int> GetTotalCountAsync(CategoryFilterDto filterDto)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM Categories WHERE 1=1";

                if (filterDto.IsActive.HasValue)
                    query += " AND IsActive = @IsActive";

                if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
                    query += @" AND (CategoryName LIKE '%' + @SearchTerm + '%' 
                             OR Description LIKE '%' + @SearchTerm + '%')";

                using (IDbConnection conn = GetConnection())
                {
                    var parameters = new DynamicParameters();
                    if (filterDto.IsActive.HasValue)
                        parameters.Add("@IsActive", filterDto.IsActive.Value ? 1 : 0);
                    if (!string.IsNullOrWhiteSpace(filterDto.SearchTerm))
                        parameters.Add("@SearchTerm", filterDto.SearchTerm);

                    return await conn.ExecuteScalarAsync<int>(query, parameters);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error counting categories: {ex.Message}", ex);
            }
        }
    }
}
