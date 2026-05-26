using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.RepositoryContract.Web.Buyer;
using APNAPASHU.ServiceContract.Web.Buyer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Text.Json;

namespace APNAPASHU.Service.Web.Buyer
{
    public class BrowseAnimalService : BaseService, IBrowseAnimalService
    {
        private readonly IBrowseAnimalRepository _repository;

        public BrowseAnimalService(
            IBrowseAnimalRepository repository,
            IHttpContextAccessor accessor,
            IConfiguration configuration) : base(accessor, configuration)
        {
            _repository = repository;
        }

        public async Task<JsonModel<List<BrowseAnimalResponseModel>>> BrowseAnimalsAsync(BrowseAnimalFilterDto filter)
        {
            var result = await _repository.BrowseAnimalsAsync(filter);
            
            // Post-process the ImagesJson into full URLs
            foreach (var item in result)
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
                    catch
                    {
                        // Ignore parse errors, Images will simply remain null
                    }
                }
            }
            
            return new JsonModel<List<BrowseAnimalResponseModel>>(result, "Animals retrieved successfully.", 200);
        }

        public async Task<JsonModel<object>> ToggleFavoritesAnimal(int animalId, int userId)
        {
        
            if (animalId == 0)
            {
                return new JsonModel<object>(null, "No IDs provided for favorite", (int)HttpStatusCode.BadRequest);
            }

            var result = await _repository.ToggleFavoritesAnimal(animalId, userId);

            return new JsonModel<object>(
                result,
                result.Message,
                result.StatusCode == "SUCCESS" ? (int)HttpStatusCode.OK : (int)HttpStatusCode.BadRequest
            );

        }

        public async Task<JsonModel<AnimalDetailsResponseModel>> GetAnimalDetailsByIdAsync(int id, int userId)
        {
            var item = await _repository.GetAnimalDetailsByIdAsync(id, userId);
            if (item == null)
            {
                return new JsonModel<AnimalDetailsResponseModel>(null, "Animal not found.", 404);
            }

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
                catch
                {
                    // Ignore parse errors, Images will remain empty/null
                }
            }

            // Process seller avatar if present and requires signed URL
            if (!string.IsNullOrEmpty(item.SellerAvatar) && !item.SellerAvatar.StartsWith("http"))
            {
                try
                {
                    var avatarUrls = await _uploader.GetFileUrlsAsync(new List<string> { item.SellerAvatar }, $"users/{item.UserId}");
                    item.SellerAvatar = avatarUrls.FirstOrDefault();
                }
                catch
                {
                    // Ignore avatar conversion issues
                }
            }

            return new JsonModel<AnimalDetailsResponseModel>(item, "Animal details retrieved successfully.", 200);
        }

        private class ImageJsonHelper
        {
            public string? ImageName { get; set; }
            public string? Value { get; set; }
        }
    }
}
