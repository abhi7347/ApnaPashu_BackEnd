using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Categories;

namespace APNAPASHU.RepositoryContract.Web
{
    /// <summary>
    /// Category Repository Interface for Web - CRUD Template
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Get all categories with filtering and pagination
        /// </summary>
        Task<List<CatetoryResponseModel>> GetAllAsync(FilterDto filterDto);

        /// <summary>
        /// Get category by ID
        /// </summary>
        Task<CatetoryResponseModel> GetByIdAsync(int categoryId);

        /// <summary>
        /// Create category
        /// </summary>
        Task<SqlResponseModel> UpsertAsync(CategoryUpsertModel model, int userId);

        /// <summary>
        /// Update category (full properties)
        /// </summary>
        Task<SqlResponseModel> UpdateStatusAsync(UpdateStatusDto updateStatusDto);

        /// <summary>
        /// Delete category (soft delete)
        /// </summary>
        Task<SqlResponseModel> DeleteAsync(int categoryId, int userId);

    }
}
