using Dapper;
using System.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models.Web.Admin.Staff;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.Repository.Data;

using APNAPASHU.Common.Messages;

namespace APNAPASHU.Repository.Web.Admin
{
    public class StaffRepository : BaseRepository, IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(IConfiguration configuration, AppDbContext context) : base(configuration)
        {
            _context = context;
        }

        public async Task<List<StaffResponseModel>> GetAllAsync(FilterDto filterDto)
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@PageNumber", filterDto.PageNumber, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@PageSize", filterDto.PageSize, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@SearchTerm", filterDto.SearchTerm, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortColumns", filterDto.SortCulumn, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortDirection", filterDto.SortDirection, DbType.String, ParameterDirection.Input);

            return await GetAsyncList<StaffResponseModel>(
                "[dbo].[usp_Staff_GetAll]",
                parameter,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<StaffResponseModel> GetByIdAsync(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var result = await GetAsyncList<StaffResponseModel>(
                "[dbo].[usp_Staff_GetById]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result.FirstOrDefault()!;
        }

        public async Task<SqlResponseModel> UpsertAsync(StaffUpsertModel model, int userId, string? passwordHash)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@FirstName", model.FirstName);
            parameters.Add("@LastName", model.LastName);
            parameters.Add("@Email", model.Email);
            parameters.Add("@Phone", model.Phone);
            parameters.Add("@RoleId", model.RoleId);
            parameters.Add("@PasswordHash", passwordHash);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@UserId", userId);

            return await AddAsync<SqlResponseModel>(
                "[dbo].[usp_Staff_Upsert]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<SqlResponseModel> UpdateStatusAsync(UpdateStatusDto model)
        {
            var staff = await _context.Users.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (staff != null)
            {
                staff.IsActive = model.Status;
                staff.UpdatedBy = model.UserId;
                staff.UpdatedDate = DateTime.Now;

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
                "[dbo].[usp_Staff_MultiDelete]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }
    }
}
