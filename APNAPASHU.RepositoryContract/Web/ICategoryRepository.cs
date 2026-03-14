using APNAPASHU.DataContract.Models.Category;

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
        Task<List<CategoryResponseDto>> GetAllAsync(CategoryFilterDto filterDto);

        /// <summary>
        /// Get category by ID
        /// </summary>
        Task<CategoryResponseDto> GetByIdAsync(int categoryId);

        /// <summary>
        /// Create category
        /// </summary>
        Task<int> CreateAsync(CategoryUpsertDto upsertDto);

        /// <summary>
        /// Update category (full properties)
        /// </summary>
        Task<bool> UpdateAsync(CategoryUpsertDto upsertDto);

        /// <summary>
        /// Update category status (IsActive)
        /// </summary>
        Task<bool> UpdateStatusAsync(CategoryStatusUpdateDto statusDto);

        /// <summary>
        /// Delete category (soft delete)
        /// </summary>
        Task<bool> DeleteAsync(int categoryId);

        /// <summary>
        /// Get total count with filter
        /// </summary>
        Task<int> GetTotalCountAsync(CategoryFilterDto filterDto);
    }
}
