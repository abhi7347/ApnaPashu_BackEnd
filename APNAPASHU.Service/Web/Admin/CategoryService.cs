using APNAPASHU.Common.Messages;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Categories;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.ServiceContract.Web.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace APNAPASHU.Service.Web.Admin
{
    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(
            ICategoryRepository repository,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _repository = repository;
        }

        public async Task<JsonModel<List<CategoryResponseModel>>> GetAllAsync(FilterDto filter)
        {
            var data = await _repository.GetAllAsync(filter);
            
            // Map image URLs if necessary
            foreach (var item in data)
            {
                if (!string.IsNullOrEmpty(item.ImagePath))
                {
                    try
                    {
                        var urls = await _uploader.GetFileUrlsAsync(new List<string> { item.ImagePath }, $"categories/{item.Id}");
                        item.ImageUrl = urls.FirstOrDefault();
                    }
                    catch
                    {
                        // Ignore mapping failure
                    }
                }
            }

            return new JsonModel<List<CategoryResponseModel>>(data, ResponseMessages.fetchedSuccessfully, (int)HttpStatusCode.OK);
        }

        public async Task<JsonModel<CategoryResponseModel>> GetByIdAsync(int id)
        {
            var data = await _repository.GetByIdAsync(id);
            if (data == null)
            {
                return new JsonModel<CategoryResponseModel>(null, ResponseMessages.NotFound, (int)HttpStatusCode.NotFound);
            }
            
            if (!string.IsNullOrEmpty(data.ImagePath))
            {
                try
                {
                    var urls = await _uploader.GetFileUrlsAsync(new List<string> { data.ImagePath }, $"categories/{data.Id}");
                    data.ImageUrl = urls.FirstOrDefault();
                }
                catch
                {
                    // Ignore mapping failure
                }
            }

            return new JsonModel<CategoryResponseModel>(data, ResponseMessages.fetchedSuccessfully, (int)HttpStatusCode.OK);
        }

        public async Task<JsonModel<object>> UpsertAsync(CategoryUpsertModel model, int userId)
        {
            string? newImageName = null;
            bool isNewImage = false;
            
            if (model.Id == null || model.Id == 0)
            {
                // New category
                if (model.Image != null)
                {
                    var ext = Path.GetExtension(model.Image.FileName);
                    newImageName = $"{Guid.NewGuid()}{ext}";
                    model.ImagePath = newImageName;
                    isNewImage = true;
                }
            }
            else
            {
                // Update category
                var currentCategory = await _repository.GetByIdAsync(model.Id.Value);
                if (currentCategory != null)
                {
                    if (model.Image != null)
                    {
                        // Uploading a new image, delete old one if exists
                        if (!string.IsNullOrEmpty(currentCategory.ImagePath))
                        {
                            await _uploader.DeleteFilesAsync(new List<string> { currentCategory.ImagePath }, $"categories/{model.Id}");
                        }
                        
                        var ext = Path.GetExtension(model.Image.FileName);
                        newImageName = $"{Guid.NewGuid()}{ext}";
                        model.ImagePath = newImageName;
                        isNewImage = true;
                    }
                    else if (!string.IsNullOrEmpty(model.ExistingImage))
                    {
                        // Kept existing image
                        model.ImagePath = currentCategory.ImagePath;
                    }
                    else
                    {
                        // Removed image
                        if (!string.IsNullOrEmpty(currentCategory.ImagePath))
                        {
                            await _uploader.DeleteFilesAsync(new List<string> { currentCategory.ImagePath }, $"categories/{model.Id}");
                        }
                        model.ImagePath = null;
                    }
                }
            }

            var result = await _repository.UpsertAsync(model, userId);
            
            int categoryId = result.Id ?? 0;
            if (categoryId <= 0)
            {
                return new JsonModel<object>(result, result.Message, (int)HttpStatusCode.BadRequest);
            }

            // Upload the new image if there is one
            if (isNewImage && model.Image != null && !string.IsNullOrEmpty(newImageName))
            {
                await _uploader.UploadFilesAsync(
                    new List<IFormFile> { model.Image },
                    new List<string> { newImageName },
                    $"categories/{categoryId}"
                );
            }

            bool isSuccess = string.Equals(result.StatusCode, "SUCCESS", StringComparison.OrdinalIgnoreCase) 
                             || result.StatusCode == "200" 
                             || (string.IsNullOrEmpty(result.StatusCode) && result.Message?.Contains("success", StringComparison.OrdinalIgnoreCase) == true);
                             
            int statusCode = isSuccess ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest;
            return new JsonModel<object>(result, result.Message, statusCode);
        }

        public async Task<JsonModel<object>> UpdateStatusAsync(UpdateStatusDto model)
        {
            var result = await _repository.UpdateStatusAsync(model);
            int statusCode = string.Equals(result.StatusCode, "SUCCESS", StringComparison.OrdinalIgnoreCase) ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest;
            return new JsonModel<object>(result, result.Message, statusCode);
        }

        public async Task<JsonModel<object>> DeleteAsync(List<int> ids, int userId)
        {
            if (ids == null || !ids.Any())
            {
                return new JsonModel<object>(null, "No IDs provided for deletion", (int)HttpStatusCode.BadRequest);
            }

            // Purge images for all deleted categories
            foreach (var categoryId in ids)
            {
                try
                {
                    await _uploader.DeleteFolderAsync($"categories/{categoryId}");
                }
                catch
                {
                    // Ignore failure to delete folder
                }
            }

            string idsStr = string.Join(",", ids);
            var result = await _repository.DeleteAsync(idsStr, userId);
            
            bool isSuccess = string.Equals(result.StatusCode, "SUCCESS", StringComparison.OrdinalIgnoreCase) 
                             || result.StatusCode == "200" 
                             || (string.IsNullOrEmpty(result.StatusCode) && result.Message?.Contains("success", StringComparison.OrdinalIgnoreCase) == true);
                             
            int statusCode = isSuccess ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest;
            return new JsonModel<object>(result, result.Message, statusCode);
        }
    }
}
