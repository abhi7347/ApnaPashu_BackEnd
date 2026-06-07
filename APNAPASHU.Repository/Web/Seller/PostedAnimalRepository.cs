using System.Data;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Seller.PostedAnimal;
using APNAPASHU.Repository.Data;
using APNAPASHU.RepositoryContract.Web.Seller;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace APNAPASHU.Repository.Web.Seller
{
    public class PostedAnimalRepository : BaseRepository, IPostedAnimalRepository
    {
        private readonly AppDbContext _context;

        public PostedAnimalRepository(IConfiguration configuration, AppDbContext context) : base(configuration)
        {
            _context = context;
        }

        public async Task<List<PostedAnimalResponseModel>> GetAllAsync(FilterDto filterDto, int userId)
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@PageNumber", filterDto.PageNumber, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@PageSize", filterDto.PageSize, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@SearchTerm", filterDto.SearchTerm, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortColumns", filterDto.SortCulumn, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortDirection", filterDto.SortDirection, DbType.String, ParameterDirection.Input);
            parameter.Add("@UserId", userId, DbType.Int32, ParameterDirection.Input);

            var result = await GetAsyncList<PostedAnimalResponseModel>(
                "[dbo].[usp_GetAllPostedAnimals]",
                parameter,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result;
        }

        public async Task<PostedAnimalResponseModel> GetByIdAsync(int id)
        {
            DynamicParameters parameter = new DynamicParameters();
            parameter.Add("@Id", id, DbType.Int32, ParameterDirection.Input);

            var result = await GetAsyncList<PostedAnimalResponseModel>(
                "[dbo].[usp_GetPostedAnimalById]",
                parameter,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result.FirstOrDefault() ?? new PostedAnimalResponseModel();
        }

        public async Task<List<PostedAnimalResponseModel>> GetByIdsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any()) return new List<PostedAnimalResponseModel>();

            // Fetch data first (EF Core cannot translate JSON serialization to SQL)
            var data = await _context.PostedAnimals
                .Where(pa => ids.Contains(pa.Id))
                .Select(pa => new
                {
                    pa.Id,
                    Images = pa.Images.Select(img => new { value = img.ImageName }).ToList()
                })
                .ToListAsync();

            // Perform serialization in-memory
            return data.Select(x => new PostedAnimalResponseModel
            {
                Id = x.Id,
                ImagesJson = System.Text.Json.JsonSerializer.Serialize(x.Images)
            }).ToList();
        }

        public async Task<SqlResponseModel> UpsertAsync(PostedAnimalUpsertModel model, int userId)
        {
            var parameters = new DynamicParameters();

            parameters.Add("@Id", model.Id);
            parameters.Add("@Name", model.Name);
            parameters.Add("@CategoryId", model.CategoryId);
            parameters.Add("@Breed", model.Breed);
            parameters.Add("@AgeInMonths", model.AgeInMonths);
            parameters.Add("@GenderId", model.GenderId);
            parameters.Add("@Price", model.Price);
            parameters.Add("@Location", model.Location);
            parameters.Add("@Description", model.Description);

            parameters.Add("@ImagesJson", model.ImageNames != null && model.ImageNames.Any()
                ? System.Text.Json.JsonSerializer.Serialize(model.ImageNames)
                : null);

            parameters.Add("@IsFeatured", model.IsFeatured);
            parameters.Add("@FeaturedTill", model.FeaturedTill);

            parameters.Add("@UserId", userId);

            return await AddAsync<SqlResponseModel>(
                "usp_UpsertPostedAnimal",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }



        public async Task<SqlResponseModel> DeleteAsync(List<int> ids, int userId)
        {
            var idsCsv = string.Join(",", ids);

            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Ids", idsCsv);
            parameters.Add("@UserId", userId);

            return await UpdateAsync<SqlResponseModel>(
                "dbo.usp_DeletePostedAnimal",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<int> UpdateSoldStatusAsync(int id, bool isSold, int userId)
        {
            var animal = await _context.PostedAnimals.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
            if (animal != null)
            {
                animal.IsSold = isSold;
                animal.SoldDate = isSold ? DateTime.Now : null;
                animal.UpdatedBy = userId;
                animal.UpdatedDate = DateTime.Now;

                return await _context.SaveChangesAsync();
            }
            return 0;
        }
    }
}
