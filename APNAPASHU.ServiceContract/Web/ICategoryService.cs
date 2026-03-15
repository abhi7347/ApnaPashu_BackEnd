using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Categories;

namespace APNAPASHU.ServiceContract.Web
{
    /// <summary>
    /// Category Service Interface for Web - CRUD Template
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Get all categories with filtering and pagination
        /// </summary>
        Task<JsonModel<List<CatetoryResponseModel>>> GetAllAsync(FilterDto filterDto);

        /// <summary>
        /// Create or Update category (Upsert)
        /// </summary>
        Task<JsonModel<CatetoryResponseModel>> GetByIdAsync(int categoryId);

        /// <summary>
        /// Update category status (IsActive)
        /// </summary>
        Task<JsonModel<object>> UpsertAsync(CategoryUpsertModel model, int userId);

        /// <summary>
        /// Delete category (soft delete)
        /// </summary>
        Task<JsonModel<object>> DeleteAsync(int categoryId, int userId);

        Task<JsonModel<object>> UpdateStatusAsync(UpdateStatusDto model);

    }
}
