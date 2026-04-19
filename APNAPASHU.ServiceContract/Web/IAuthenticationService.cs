using APNAPASHU.DataContract.Models.Web.Authentication;
using APNAPASHU.DataContract.Models;

namespace APNAPASHU.ServiceContract.Web
{
    public interface IAuthenticationService
    {
        Task<JsonModel<SqlResponseModel>> RegisterAsync(RegisterRequestModel model);
        Task<JsonModel<LoginResponseModel>> LoginUserAsync(string email, string password);
        Task<JsonModel<SqlResponseModel>> ForgotPasswordAsync(string email);
        Task<JsonModel<SqlResponseModel>> ResetPasswordAsync(string email, string token, string newPassword);
        Task<JsonModel<LoginResponseModel>> UpdateProfileAsync(UpdateProfileRequestModel model, int userId);
        Task<JsonModel<LoginResponseModel>> GetUserProfileAsync(int userId);
        Task<JsonModel<SqlResponseModel>> ChangePasswordAsync(ChangePasswordRequestModel model, int userId);
    }
}
