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
                .Where(x => x.IsActive && !x.IsDeleted)
                .Select(x => new MasterDropdownsModels
                {
                    Id = x.Id,
                    Name = x.CategoryName
                })
                .ToListAsync();
        }
    }
}
