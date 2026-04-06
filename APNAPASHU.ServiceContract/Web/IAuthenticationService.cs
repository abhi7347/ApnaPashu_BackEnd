using APNAPASHU.DataContract.Models.Web.Authentication;
using APNAPASHU.DataContract.Models;

namespace APNAPASHU.ServiceContract.Web
{
    public interface IAuthenticationService
    {
        Task<JsonModel<AuthResponseModel>> RegisterAsync(RegisterRequestModel model);
        Task<JsonModel<AuthResponseModel>> LoginAsync(LoginRequestModel model);
    }
}
