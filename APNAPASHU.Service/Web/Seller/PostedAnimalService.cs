using APNAPASHU.Common.Messages;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Seller.PostedAnimal;
using APNAPASHU.RepositoryContract.Web.Seller;
using APNAPASHU.ServiceContract.Web.Seller;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace APNAPASHU.Service.Web.Seller
{
    public class PostedAnimalService : BaseService, IPostedAnimalService
    {
        private readonly IPostedAnimalRepository _repository;

        public PostedAnimalService(IPostedAnimalRepository repository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
            : base(httpContextAccessor, configuration)
        {
            _repository = repository;
        }

        public async Task<JsonModel<List<PostedAnimalResponseModel>>> GetAllAsync(FilterDto filterDto, int userId)
        {
            var data = await _repository.GetAllAsync(filterDto, userId);

            foreach (var item in data)
            {
                await ProcessImages(item);
            }

            return new JsonModel<List<PostedAnimalResponseModel>>(
                data,
                ResponseMessages.fetchedSuccessfully,
                (int)HttpStatusCode.OK
            );
        }

        private async Task ProcessImages(PostedAnimalResponseModel item)
        {
            if (!string.IsNullOrEmpty(item.ImagesJson))
            {
                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var jsonImages = JsonSerializer.Deserialize<List<ImageJsonHelper>>(item.ImagesJson, options);
                    if (jsonImages != null && jsonImages.Any())
                    {
                        var names = jsonImages
                            .Select(x => x.Value ?? x.ImageName)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList();

                        if (names.Any())
                        {
                            item.Images = await _uploader.GetFileUrlsAsync(names!, $"posted-animals/{item.Id}");
                            item.ImageUrl = item.Images.FirstOrDefault();
                        }
                    }
                }
                catch (Exception)
                {
                    // Fallback or log log deserialization failure
                }
            }
        }

        private class ImageJsonHelper
        {
            public string? ImageName { get; set; } // matches older versions or other SPs
            public string? Value { get; set; } // matches user's provided [value] alias
        }

        public async Task<JsonModel<PostedAnimalResponseModel>> GetByIdAsync(int id)
        {
            var data = await _repository.GetByIdAsync(id);

            if (data == null)
            {
                return new JsonModel<PostedAnimalResponseModel>(
                    null,
                    ResponseMessages.NotFound,
                    (int)HttpStatusCode.NotFound
                );
            }

            await ProcessImages(data);

            return new JsonModel<PostedAnimalResponseModel>(
                data,
                ResponseMessages.fetchedSuccessfully,
                (int)HttpStatusCode.OK
            );
        }

        public async Task<JsonModel<object>> UpsertAsync(PostedAnimalUpsertModel model, int userId)
        {
            var imageNames = new List<string>();

            // INSERT → images REQUIRED
            if (model.Id == null || model.Id == 0)
            {
                if (model.NewImages == null || !model.NewImages.Any())
                {
                    return new JsonModel<object>(null, "Images are required", (int)HttpStatusCode.BadRequest);
                }

                imageNames = model.NewImages.Select(file =>
                {
                    var ext = Path.GetExtension(file.FileName);
                    return $"{Guid.NewGuid()}{ext}";
                }).ToList();
            }
            else
            {
                // UPDATE FLOW - Merge existing kept images with new uploads
                var currentAnimal = await _repository.GetByIdAsync(model.Id.Value);
                var originalNames = new List<string>();

                if (currentAnimal != null && !string.IsNullOrEmpty(currentAnimal.ImagesJson))
                {
                    try
                    {
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var jsonImages = JsonSerializer.Deserialize<List<ImageJsonHelper>>(currentAnimal.ImagesJson, options);
                        originalNames = jsonImages?
                            .Select(x => x.Value ?? x.ImageName)
                            .Where(x => !string.IsNullOrEmpty(x))
                            .ToList() ?? new List<string>();
                    }
                    catch { }
                }

                // 1. Identify what the user chose to KEEP (extract filenames from URLs)
                var keptNames = model.ExistingImages?.Select(x =>
                {
                    try { return Path.GetFileName(new Uri(x).LocalPath); }
                    catch { return x; }
                }).Where(x => !string.IsNullOrEmpty(x)).ToList() ?? new List<string>();

                // 2. Identify what to DELETE from storage (Original - Kept)
                var toDelete = originalNames.Where(n => !keptNames.Contains(n)).ToList();
                if (toDelete.Any())
                {
                    await _uploader.DeleteFilesAsync(toDelete!, $"posted-animals/{model.Id}");
                }

                // 3. Start building the new ImageNames list with kept files
                imageNames.AddRange(keptNames!);

                // 4. Generate names for NEW images and add to the list
                if (model.NewImages != null && model.NewImages.Any())
                {
                    var newNames = model.NewImages.Select(file =>
                    {
                        var ext = Path.GetExtension(file.FileName);
                        return $"{Guid.NewGuid()}{ext}";
                    }).ToList();
                    imageNames.AddRange(newNames);
                    
                    // Store new names temporarily for physical upload late
                    model.ImageNames = newNames; 
                }
            }

            model.ImageNames = imageNames;

            // CALL SP (NO EXTRA PARAM)
            var result = await _repository.UpsertAsync(model, userId);
            int animalId = result.Id ?? 0;

            if (animalId <= 0)
            {
                return new JsonModel<object>(result, result.Message, (int)HttpStatusCode.BadRequest);
            }

            // UPLOAD FILES (ONLY IF NEW)
            if (model.NewImages != null && model.NewImages.Any())
            {
                // We need the names that correspond ONLY to the new images
                var newNamesOnly = imageNames.Skip(imageNames.Count - model.NewImages.Count()).ToList();

                await _uploader.UploadFilesAsync(
                    model.NewImages.ToList(),
                    newNamesOnly,
                    $"posted-animals/{animalId}"
                );
            }

            return new JsonModel<object>(
                result,
                result.Message,
                (int)HttpStatusCode.OK
            );
        }



        public async Task<JsonModel<object>> DeleteAsync(List<int> ids, int userId)
        {
            if (ids == null || !ids.Any())
            {
                return new JsonModel<object>(null, "No IDs provided for deletion", (int)HttpStatusCode.BadRequest);
            }

            // 1. Fetch animals to be deleted to get their image lists
            var animals = await _repository.GetByIdsAsync(ids);

            // 2. PURGE entire folder from cloud storage
            foreach (var animalId in ids)
            {
                try
                {
                    await _uploader.DeleteFolderAsync($"posted-animals/{animalId}");
                }
                catch
                {
                    // Log failure but continue with DB deletion
                }
            }

            // 3. Call DB deletion (SP)
            var result = await _repository.DeleteAsync(ids, userId);

            return new JsonModel<object>(
                result,
                result.Message,
                result.StatusCode == "SUCCESS" ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest
            );
        }
    }
}
