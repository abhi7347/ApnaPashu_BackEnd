namespace APNAPASHU.API.Extensions
{
    using Microsoft.Extensions.DependencyInjection;
    using APNAPASHU.Repository;

    /// <summary>
    /// Extension methods for services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add environment variable provider
        /// </summary>
        public static IServiceCollection AddEnvironmentVariableProvider(this IServiceCollection services)
        {
            return services;
        }

        /// <summary>
        /// Add application configuration
        /// </summary>
        public static IServiceCollection AddApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            return services;
        }

        /// <summary>
        /// Register Web/Admin Services and Repositories
        /// </summary>
        public static IServiceCollection AddWebServices(this IServiceCollection services)
        {
            // Web Animal Repository & Service
            services.AddScoped<RepositoryContract.Web.IAnimalRepository, Repository.Web.AnimalRepository>();
            services.AddScoped<ServiceContract.Web.IAnimalService, Service.Web.AnimalService>();

            // Web Category Repository & Service
            services.AddScoped<RepositoryContract.Web.ICategoryRepository, Repository.Web.CategoryRepository>();
            services.AddScoped<ServiceContract.Web.ICategoryService, Service.Web.CategoryService>();

            return services;
        }

        /// <summary>
        /// Register Mobile Services and Repositories
        /// </summary>
        public static IServiceCollection AddMobileServices(this IServiceCollection services)
        {
            // Mobile Repository
            services.AddScoped<APNAPASHU.RepositoryContract.Mobile.IAnimalRepository, APNAPASHU.Repository.Mobile.AnimalRepository>();

            // Mobile Service
            services.AddScoped<APNAPASHU.ServiceContract.Mobile.IAnimalService, APNAPASHU.Service.Mobile.AnimalService>();

            return services;
        }

        /// <summary>
        /// Register all common services
        /// </summary>
        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {
            // BaseRepository is abstract and should not be registered directly
            // It's inherited by concrete repositories (AnimalRepository, CategoryRepository, etc.)
            services.AddHttpContextAccessor();
            services.AddLogging();

            return services;
        }
    }

    /// <summary>
    /// Database settings configuration
    /// </summary>
    public class DatabaseSettings
    {
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public int CommandTimeout { get; set; } = 30;
    }

    /// <summary>
    /// JWT settings configuration
    /// </summary>
    public class JwtSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryMinutes { get; set; } = 60;
    }
}