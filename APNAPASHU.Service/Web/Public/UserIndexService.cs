#nullable disable
using APNAPASHU.DataContract.Models.Web.Public.UserIndex;
using APNAPASHU.RepositoryContract.Web.Public;
using APNAPASHU.ServiceContract.Web.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace APNAPASHU.Service.Web.Public
{
    public class UserIndexService : BaseService, IUserIndexService
    {
        private readonly IUserIndexRepository _userIndexRepository;

        public UserIndexService(
            IUserIndexRepository userIndexRepository, 
            IHttpContextAccessor accessor,
            IConfiguration configuration) : base(accessor, configuration)
        {
            _userIndexRepository = userIndexRepository;
        }

        public async Task<UserIndexResponseModel> GetIndexAnimalsAsync()
        {
            var result = await _userIndexRepository.GetIndexAnimalsAsync();

            // Process Featured Animals
            if (result.FeaturedAnimals != null && result.FeaturedAnimals.Any())
            {
                foreach (var animal in result.FeaturedAnimals)
                {
                    if (!string.IsNullOrWhiteSpace(animal.ImagesJson))
                    {
                        var images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(animal.ImagesJson);
                        if (images != null && images.Any())
                        {
                            animal.Images = images;
                            var urls = await _uploader.GetFileUrlsAsync(new List<string> { images.First() }, $"posted-animals/{animal.Id}");
                            animal.ImageUrl = urls?.FirstOrDefault();
                        }
                    }
                }
            }

            // Process Recent Animals
            if (result.RecentAnimals != null && result.RecentAnimals.Any())
            {
                foreach (var animal in result.RecentAnimals)
                {
                    if (!string.IsNullOrWhiteSpace(animal.ImagesJson))
                    {
                        var images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(animal.ImagesJson);
                        if (images != null && images.Any())
                        {
                            animal.Images = images;
                            var urls = await _uploader.GetFileUrlsAsync(new List<string> { images.First() }, $"posted-animals/{animal.Id}");
                            animal.ImageUrl = urls?.FirstOrDefault();
                        }
                    }
                }
            }

            return result;
        }
    }
}
