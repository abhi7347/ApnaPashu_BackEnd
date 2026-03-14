using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace APNAPASHU.Repository
{
    /// <summary>
    /// Base Repository with common database operations
    /// </summary>
    public abstract class BaseRepository
    {
        protected readonly IConfiguration Configuration;

        public BaseRepository(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Get database connection
        /// </summary>
        protected IDbConnection GetConnection()
        {
            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Database connection string is not configured");

            return new SqlConnection(connectionString);
        }

        /// <summary>
        /// Get connection string safely
        /// </summary>
        protected string GetConnectionString()
        {
            return Configuration.GetConnectionString("DefaultConnection");
        }
    }
}