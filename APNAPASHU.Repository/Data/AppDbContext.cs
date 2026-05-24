using APNAPASHU.DataContract.Entity;
using APNAPASHU.DataContract.Entity.Admin;
using APNAPASHU.DataContract.Entity.Seller;
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
        public DbSet<Roles> Roles { get; set; }
        public DbSet<PostedAnimal> PostedAnimals { get; set; }
        public DbSet<PostedAnimalImage> PostedAnimalImages { get; set; }
        public DbSet<AnimalPromotion> AnimalPromotions { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }
    }
}
