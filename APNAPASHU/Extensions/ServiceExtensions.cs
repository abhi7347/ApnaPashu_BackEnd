using APNAPASHU.Repository;
using APNAPASHU.Repository.Web;
using APNAPASHU.Repository.Web.Admin;
using APNAPASHU.Repository.Web.Seller;
using APNAPASHU.RepositoryContract;
using APNAPASHU.RepositoryContract.Web;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.RepositoryContract.Web.Seller;
using APNAPASHU.Service.Web;
using APNAPASHU.Service.Web.Admin;
using APNAPASHU.Service.Web.Seller;
using APNAPASHU.ServiceContract;
using APNAPASHU.ServiceContract.Web;
using APNAPASHU.ServiceContract.Web.Admin;
using APNAPASHU.ServiceContract.Web.Seller;
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

            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            services.AddScoped<IPostedAnimalRepository, PostedAnimalRepository>();
            services.AddScoped<IPostedAnimalService, PostedAnimalService>();


            // Mobile APIs

            return services;
        }
    }
}
