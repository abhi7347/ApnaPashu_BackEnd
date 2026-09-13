using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Admin.User;

namespace APNAPASHU.RepositoryContract.Web.Admin
{
    public interface IUserRepository
    {
        Task<List<UserResponseModel>> GetAllAsync(FilterDto filterDto);
        Task<UserResponseModel> GetByIdAsync(int id);
        Task<SqlResponseModel> UpsertAsync(UserUpsertModel model, int userId, string? passwordHash);
        Task<SqlResponseModel> UpdateStatusAsync(UpdateStatusDto model);
        Task<SqlResponseModel> DeleteAsync(string ids, int userId);
    }
}
