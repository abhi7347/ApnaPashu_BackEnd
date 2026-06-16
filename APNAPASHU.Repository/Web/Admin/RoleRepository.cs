#nullable disable
using Dapper;
using System.Data;
using Microsoft.Extensions.Configuration;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Enums;
using APNAPASHU.DataContract.Models.Web.Admin.Roles;
using APNAPASHU.RepositoryContract.Web.Admin;
using APNAPASHU.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace APNAPASHU.Repository.Web.Admin
{
    public class RoleRepository : BaseRepository, IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(IConfiguration configuration, AppDbContext context) : base(configuration)
        {
            _context = context;
        }

        public async Task<List<RoleResponseModel>> GetAllAsync(FilterDto filterDto)
        {
            DynamicParameters parameter = new DynamicParameters();

            parameter.Add("@PageNumber", filterDto.PageNumber, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@PageSize", filterDto.PageSize, DbType.Int32, ParameterDirection.Input);
            parameter.Add("@SearchTerm", filterDto.SearchTerm, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortBy", filterDto.SortCulumn, DbType.String, ParameterDirection.Input);
            parameter.Add("@SortDirection", filterDto.SortDirection, DbType.String, ParameterDirection.Input);

            return await GetAsyncList<RoleResponseModel>(
                "[dbo].[usp_Role_GetAll]",
                parameter,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<RoleResponseModel> GetByIdAsync(int id)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Id", id);

            var result = await GetAsyncList<RoleResponseModel>(
                "[dbo].[usp_Role_GetById]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );

            return result.FirstOrDefault();
        }

        public async Task<SqlResponseModel> UpsertAsync(RoleUpsertModel model, int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Id", model.Id);
            parameters.Add("@RoleName", model.RoleName);
            parameters.Add("@RoleDescription", model.RoleDescription);
            parameters.Add("@IsActive", model.IsActive);
            parameters.Add("@UserId", userId);

            return await AddAsync<SqlResponseModel>(
                "[dbo].[usp_Role_Upsert]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }

        public async Task<SqlResponseModel> UpdateStatusAsync(UpdateStatusDto model)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (role != null)
            {
                role.IsActive = model.Status;
                role.UpdatedBy = model.UserId;
                role.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync();
                
                return new SqlResponseModel { StatusCode = "SUCCESS", Message = "Role status updated successfully" };
            }
            
            return new SqlResponseModel { StatusCode = "ERROR", Message = "Role not found" };
        }

        public async Task<SqlResponseModel> DeleteAsync(string ids, int userId)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@Ids", ids);
            parameters.Add("@UserId", userId);

            return await UpdateAsync<SqlResponseModel>(
                "[dbo].[usp_Role_MultiDelete]",
                parameters,
                CommandType.StoredProcedure,
                DataBaseNameEnum.APNAPASHU
            );
        }
    }
}
