using APNAPASHU.Repository;
using APNAPASHU.Repository.Web;
using APNAPASHU.RepositoryContract;
using APNAPASHU.RepositoryContract.Web;
using APNAPASHU.Service.Web;
using APNAPASHU.ServiceContract;
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

            services.AddScoped<IMasterDropdownsService, MasterDropdownsService>();
            services.AddScoped<IMasterDropdownsRepository, MasterDropdownsRepository>();


            // Mobile APIs

            return services;
        }
    }
}
