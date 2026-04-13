using APNAPASHU.DataContract.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using APNAPASHU.Common.Messages;
using APNAPASHU.DataContract.Models.Web.Admin.Categories;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.ServiceContract.Web.Admin;

namespace APNAPASHU.Service.Web.Admin
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
        public async Task<JsonModel<List<CatetoryResponseModel>>> GetAllAsync(FilterDto filterDto)
        {
            var data = await _categoryRepository.GetAllAsync(filterDto);

            return new JsonModel<List<CatetoryResponseModel>>(
                data,
                ResponseMessages.fetchedSuccessfully,
                200
            );
        }

        /// <summary>
        /// Create or Update category (Upsert)
        /// </summary>
        public async Task<JsonModel<CatetoryResponseModel>> GetByIdAsync(int categoryId)
        {
            var data = await _categoryRepository.GetByIdAsync(categoryId);

            if (data == null)
            {
                return new JsonModel<CatetoryResponseModel>(
                    null,
                    ResponseMessages.NotFound,
                    404
                );
            }

            return new JsonModel<CatetoryResponseModel>(
                data,
                ResponseMessages.fetchedSuccessfully,
                200
            );
        }

        public async Task<JsonModel<object>> UpsertAsync(CategoryUpsertModel model, int userId)
        {
            var result = await _categoryRepository.UpsertAsync(model, userId);

            return new JsonModel<object>(
                result,
                result.Message,
                result.StatusCode == "SUCCESS" ? 200 : 400
            );
        }

        public async Task<JsonModel<object>> UpdateStatusAsync(UpdateStatusDto model)
        {
            var result = await _categoryRepository.UpdateStatusAsync(model);

            return new JsonModel<object>(
                result,
                result.Message,
                result.StatusCode == "SUCCESS" ? 200 : 400
            );
        }

        public async Task<JsonModel<object>> DeleteAsync(int categoryId, int userId)
        {
            var result = await _categoryRepository.DeleteAsync(categoryId, userId);

            return new JsonModel<object>(
                result,
                result.Message,
                result.StatusCode == "SUCCESS" ? 200 : 400
            );
        }
    }
}
