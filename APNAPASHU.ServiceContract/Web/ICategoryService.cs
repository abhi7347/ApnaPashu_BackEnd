using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Category;

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
        Task<JsonModel<CategoryListResponseDto>> GetAllAsync(CategoryFilterDto filterDto);

        /// <summary>
        /// Create or Update category (Upsert)
        /// </summary>
        Task<JsonModel<CategoryResponseDto>> UpsertAsync(CategoryUpsertDto upsertDto);

        /// <summary>
        /// Update category status (IsActive)
        /// </summary>
        Task<JsonModel<bool>> UpdateStatusAsync(CategoryStatusUpdateDto statusDto);

        /// <summary>
        /// Delete category (soft delete)
        /// </summary>
        Task<JsonModel<bool>> DeleteAsync(int categoryId);
    }
}
