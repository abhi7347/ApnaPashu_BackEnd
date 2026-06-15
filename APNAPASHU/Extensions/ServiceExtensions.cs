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
using APNAPASHU.Repository.Web.Buyer;
using APNAPASHU.RepositoryContract.Web.Buyer;
using APNAPASHU.Service.Web.Buyer;
using APNAPASHU.ServiceContract.Web.Buyer;
using APNAPASHU.Repository.Web.Public;
using APNAPASHU.RepositoryContract.Web.Public;
using APNAPASHU.Service.Web.Public;
using APNAPASHU.ServiceContract.Web.Public;
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
            services.AddScoped<IAnimalPromotionRepository, AnimalPromotionRepository>();
            services.AddScoped<ISellerDashboardRepository, SellerDashboardRepository>();
            services.AddScoped<ISellerDashboardService, SellerDashboardService>();

            services.AddScoped<IBrowseAnimalRepository, BrowseAnimalRepository>();
            services.AddScoped<IBrowseAnimalService, BrowseAnimalService>();

            services.AddScoped<IFavoriteAnimalRepository, FavoriteAnimalRepository>();
            services.AddScoped<IFavoriteAnimalService, FavoriteAnimalService>();

            services.AddScoped<IConversationRepository, ConversationRepository>();
            services.AddScoped<IConversationService, ConversationService>();

            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageService, MessageService>();
            
            services.AddScoped<IUserIndexRepository, UserIndexRepository>();
            services.AddScoped<IUserIndexService, UserIndexService>();

            services.AddScoped<IAdminSidebarRepository, AdminSidebarRepository>();
            services.AddScoped<IAdminSidebarService, AdminSidebarService>();

            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IPermissionService, PermissionService>();

            // Mobile APIs

            return services;
        }
    }
}
