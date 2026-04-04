using APNAPASHU.DataContract.Entity;
using Microsoft.EntityFrameworkCore;

namespace APNAPASHU.Repository.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {
        }

        public DbSet<Categories> Categories { get; set; }
    }
}
