using APNAPASHU.DataContract.Models.Web.Authentication;

namespace APNAPASHU.RepositoryContract.Web
{
    public interface IAuthenticationRepository
    {
        Task<AuthResponseModel> RegisterAsync(RegisterRequestModel model);
        Task<AuthResponseModel> GetUserByEmailAsync(string email);
    }
}
