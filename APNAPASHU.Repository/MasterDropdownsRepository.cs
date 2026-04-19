using APNAPASHU.DataContract.Models;
using APNAPASHU.Repository.Data;
using APNAPASHU.RepositoryContract;
using Microsoft.EntityFrameworkCore;

namespace APNAPASHU.Repository
{
    public class MasterDropdownsRepository : IMasterDropdownsRepository
    {
        private readonly AppDbContext _context;

        public MasterDropdownsRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MasterDropdownsModels>> GetCategoriesDropDowns()
        {
            return await _context.Categories
                .Where(x => (x.IsActive ?? true) && !(x.IsDeleted ?? false))
                .Select(x => new MasterDropdownsModels
                {
                    Id = x.Id ?? 0,
                    Name = x.CategoryName
                })
                .ToListAsync();
        }
        
        public async Task<List<MasterDropdownsModels>> GetRolesDropDowns()
        {
            return await _context.Roles
                .Where(x => (x.IsActive ?? true) && !(x.IsDeleted ?? false))
                .Select(x => new MasterDropdownsModels
                {
                    Id = x.Id ?? 0,
                    Name = x.RoleName
                })
                .ToListAsync();
        }
    }
}
