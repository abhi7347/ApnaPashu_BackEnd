using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Authentication;

namespace APNAPASHU.RepositoryContract.Web
{
    public interface IAuthenticationRepository
    {
        Task<SqlResponseModel> RegisterAsync(RegisterRequestModel model);
        Task<LoginResponseModel> LoginUserAsync(string email, string password);
        Task<SqlResponseModel> ForgotPasswordAsync(string email, string token);
        Task<SqlResponseModel> ResetPasswordAsync(string email, string token, string newPasswordHash);
        Task<LoginResponseModel> GetUserByIdAsync(int userId);
        Task<SqlResponseModel> UpdateProfileAsync(UpdateProfileRequestModel model, int userId, string? passwordHash, string? profileImage);
        Task<SqlResponseModel> ChangePasswordAsync(int userId, string newPasswordHash);
    }
}
