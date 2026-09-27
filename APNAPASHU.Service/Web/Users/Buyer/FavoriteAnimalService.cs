using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Buyer.BrowseAnimal;
using APNAPASHU.DataContract.Models.Web.Buyer.FavoriteAnimal;
using APNAPASHU.RepositoryContract.Web.Buyer;
using APNAPASHU.ServiceContract.Web.Buyer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace APNAPASHU.Service.Web.Buyer
{
    public class FavoriteAnimalService : BaseService, IFavoriteAnimalService
    {
        private readonly IFavoriteAnimalRepository _repository;

        public FavoriteAnimalService(
            IFavoriteAnimalRepository repository,
            IHttpContextAccessor accessor,
            IConfiguration configuration) : base(accessor, configuration)
        {
            _repository = repository;
        }

        public async Task<JsonModel<List<BrowseAnimalResponseModel>>> GetFavoriteAnimalsAsync(FavoriteAnimalFilterDto filter)
        {
            var result = await _repository.GetFavoriteAnimalsAsync(filter);
            
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
            
            return new JsonModel<List<BrowseAnimalResponseModel>>(result, "Favorite animals retrieved successfully.", 200);
        }

        private class ImageJsonHelper
        {
            public string? ImageName { get; set; }
            public string? Value { get; set; }
        }
    }
}
