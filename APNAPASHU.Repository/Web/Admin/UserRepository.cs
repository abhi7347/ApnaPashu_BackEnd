using Dapper;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models.Web.Admin.User;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.Repository.Data;
using APNAPASHU.Common.Messages;

namespace APNAPASHU.Repository.Web.Admin
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(IConfiguration configuration, AppDbContext context) : base(configuration)
        {
            _context = context;
        }

        public async Task<List<UserResponseModel>> GetAllAsync(FilterDto filterDto)
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@PageNumber", filterDto.PageNumber, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@PageSize", filterDto.PageSize, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@SearchTerm", filterDto.SearchTerm, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortColumns", filterDto.SortCulumn, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortDirection", filterDto.SortDirection, DbType.String, ParameterDirection.Input);

            return await GetAsyncList<UserResponseModel>(
                "[dbo].[usp_User_GetAll]",
                parameter,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<UserResponseModel> GetByIdAsync(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var result = await GetAsyncList<UserResponseModel>(
                "[dbo].[usp_User_GetById]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result.FirstOrDefault()!;
        }

        public async Task<SqlResponseModel> UpsertAsync(UserUpsertModel model, int userId, string? passwordHash)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@FirstName", model.FirstName);
            parameters.Add("@MiddleName", model.MiddleName);
            parameters.Add("@LastName", model.LastName);
            parameters.Add("@Email", model.Email);
            parameters.Add("@Phone", model.Phone);
            parameters.Add("@RoleId", model.RoleId);
            parameters.Add("@PasswordHash", passwordHash);
            parameters.Add("@Address", model.Address);
            parameters.Add("@City", model.City);
            parameters.Add("@PinCode", model.PinCode);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@UserId", userId);

            return await AddAsync<SqlResponseModel>(
                "[dbo].[usp_User_Upsert]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<SqlResponseModel> UpdateStatusAsync(UpdateStatusDto model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (user != null)
            {
                user.IsActive = model.Status;
                user.UpdatedBy = model.UserId;
                user.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                
                return new SqlResponseModel { StatusCode = "SUCCESS", Message = ResponseMessages.statusUpdated };
            }
            
            return new SqlResponseModel { StatusCode = "ERROR", Message = ResponseMessages.NotFound };
        }

        public async Task<SqlResponseModel> DeleteAsync(string ids, int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Ids", ids);
            parameters.Add("@UserId", userId);

            return await UpdateAsync<SqlResponseModel>(
                "[dbo].[usp_User_MultiDelete]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }
    }
}
