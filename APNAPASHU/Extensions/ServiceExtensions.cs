using APNAPASHU.Repository.Web;
using APNAPASHU.RepositoryContract.Web;
using APNAPASHU.Service.Web;
using APNAPASHU.ServiceContract.Web;
using Microsoft.Extensions.DependencyInjection;

namespace APNAPASHU.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            // Web APIs
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryService, CategoryService>();

            // Mobile APIs

            return services;
        }
    }
}
