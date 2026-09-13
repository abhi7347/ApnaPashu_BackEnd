using System.Collections.Generic;
using System.Threading.Tasks;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.Staff;

namespace APNAPASHU.RepositoryContract.Web.Admin
{
    public interface IStaffRepository
    {
        Task<List<StaffResponseModel>> GetAllAsync(FilterDto filterDto);
        Task<StaffResponseModel> GetByIdAsync(int id);
        Task<SqlResponseModel> UpsertAsync(StaffUpsertModel model, int userId, string? passwordHash);
        Task<SqlResponseModel> UpdateStatusAsync(UpdateStatusDto model);
        Task<SqlResponseModel> DeleteAsync(string ids, int userId);
    }
}
