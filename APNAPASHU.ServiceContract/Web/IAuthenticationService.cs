using APNAPASHU.DataContract.Models.Web.Authentication;
using APNAPASHU.DataContract.Models;

namespace APNAPASHU.ServiceContract.Web
{
    public interface IAuthenticationService
    {
        Task<JsonModel<SqlResponseModel>> RegisterAsync(RegisterRequestModel model);
        Task<JsonModel<LoginResponseModel>> LoginUserAsync(string email, string password);
    }
}
