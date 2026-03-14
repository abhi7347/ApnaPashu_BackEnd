using Microsoft.Extensions.Configuration;

namespace APNAPASHU.Repository
{
    public class ExceptionLogRepository : BaseRepository
    {
        public ExceptionLogRepository(IConfiguration configuration) : base(configuration)
        {
        }

        // Exception logging repository implementation
    }
}