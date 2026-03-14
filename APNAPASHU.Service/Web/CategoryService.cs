using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Category;
using APNAPASHU.RepositoryContract.Web;
using APNAPASHU.ServiceContract.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace APNAPASHU.Service.Web
{
    /// <summary>
    /// Category Service Implementation for Web - CRUD Template
    /// </summary>
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _categoryRepository = categoryRepository;
        }

        /// <summary>
        /// Get all categories with filtering and pagination
        /// </summary>
        public async Task<JsonModel<CategoryListResponseDto>> GetAllAsync(CategoryFilterDto filterDto)
        {
            try
            {
                if (filterDto == null)
                    return new JsonModel<CategoryListResponseDto>(null, "Filter is required", 400, "INVALID_FILTER");

                if (filterDto.PageNumber <= 0 || filterDto.PageSize <= 0)
                    return new JsonModel<CategoryListResponseDto>(null, "Invalid pagination parameters", 400, "INVALID_PARAMS");

                var categories = await _categoryRepository.GetAllAsync(filterDto);
                var totalCount = await _categoryRepository.GetTotalCountAsync(filterDto);

                var response = new CategoryListResponseDto
                {
                    Data = categories,
                    TotalRecords = totalCount,
                    PageNumber = filterDto.PageNumber,
                    PageSize = filterDto.PageSize
                };

                return new JsonModel<CategoryListResponseDto>(response, "Categories retrieved successfully", 200);
            }
            catch (Exception ex)
            {
                return new JsonModel<CategoryListResponseDto>(null, "Error retrieving categories", 500, ex.Message);
            }
        }

        /// <summary>
        /// Create or Update category (Upsert)
        /// </summary>
        public async Task<JsonModel<CategoryResponseDto>> UpsertAsync(CategoryUpsertDto upsertDto)
        {
            try
            {
                if (upsertDto == null)
                    return new JsonModel<CategoryResponseDto>(null, "Invalid category data", 400, "INVALID_DATA");

                if (string.IsNullOrWhiteSpace(upsertDto.CategoryName))
                    return new JsonModel<CategoryResponseDto>(null, "Category name is required", 400, "REQUIRED_FIELD");

                int categoryId;

                if (upsertDto.CategoryId.HasValue && upsertDto.CategoryId > 0)
                {
                    // Update existing
                    var existingCategory = await _categoryRepository.GetByIdAsync(upsertDto.CategoryId.Value);
                    if (existingCategory == null)
                        return new JsonModel<CategoryResponseDto>(null, "Category not found", 404, "NOT_FOUND");

                    bool updated = await _categoryRepository.UpdateAsync(upsertDto);
                    if (!updated)
                        return new JsonModel<CategoryResponseDto>(null, "Failed to update category", 500, "UPDATE_FAILED");

                    categoryId = upsertDto.CategoryId.Value;
                }
                else
                {
                    // Create new
                    categoryId = await _categoryRepository.CreateAsync(upsertDto);
                    if (categoryId <= 0)
                        return new JsonModel<CategoryResponseDto>(null, "Failed to create category", 500, "CREATE_FAILED");
                }

                var result = await _categoryRepository.GetByIdAsync(categoryId);
                return new JsonModel<CategoryResponseDto>(result, "Category saved successfully", upsertDto.CategoryId.HasValue ? 200 : 201);
            }
            catch (Exception ex)
            {
                return new JsonModel<CategoryResponseDto>(null, "Error saving category", 500, ex.Message);
            }
        }

        /// <summary>
        /// Update category status (IsActive)
        /// </summary>
        public async Task<JsonModel<bool>> UpdateStatusAsync(CategoryStatusUpdateDto statusDto)
        {
            try
            {
                if (statusDto == null)
                    return new JsonModel<bool>(false, "Invalid status data", 400, "INVALID_DATA");

                if (statusDto.CategoryId <= 0)
                    return new JsonModel<bool>(false, "Invalid Category ID", 400, "INVALID_ID");

                var existingCategory = await _categoryRepository.GetByIdAsync(statusDto.CategoryId);
                if (existingCategory == null)
                    return new JsonModel<bool>(false, "Category not found", 404, "NOT_FOUND");

                bool result = await _categoryRepository.UpdateStatusAsync(statusDto);
                return result
                    ? new JsonModel<bool>(true, "Category status updated successfully", 200)
                    : new JsonModel<bool>(false, "Failed to update status", 500, "UPDATE_FAILED");
            }
            catch (Exception ex)
            {
                return new JsonModel<bool>(false, "Error updating status", 500, ex.Message);
            }
        }

        /// <summary>
        /// Delete category (soft delete - sets IsActive to false)
        /// </summary>
        public async Task<JsonModel<bool>> DeleteAsync(int categoryId)
        {
            try
            {
                if (categoryId <= 0)
                    return new JsonModel<bool>(false, "Invalid Category ID", 400, "INVALID_ID");

                var existingCategory = await _categoryRepository.GetByIdAsync(categoryId);
                if (existingCategory == null)
                    return new JsonModel<bool>(false, "Category not found", 404, "NOT_FOUND");

                bool result = await _categoryRepository.DeleteAsync(categoryId);
                return result
                    ? new JsonModel<bool>(true, "Category deleted successfully", 200)
                    : new JsonModel<bool>(false, "Failed to delete category", 500, "DELETE_FAILED");
            }
            catch (Exception ex)
            {
                return new JsonModel<bool>(false, "Error deleting category", 500, ex.Message);
            }
        }
    }
}
