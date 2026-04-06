using APNAPASHU.Common;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Authentication;
using APNAPASHU.RepositoryContract.Web;
using APNAPASHU.ServiceContract.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace APNAPASHU.Service.Web
{
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationService(
            IAuthenticationRepository authenticationRepository,
            IHttpContextAccessor accessor,
            IConfiguration configuration) : base(accessor, configuration)
        {
            _authenticationRepository = authenticationRepository;
        }

        public async Task<JsonModel<AuthResponseModel>> RegisterAsync(RegisterRequestModel model)
        {
            // Hash the password before saving
            model.Password = EncryptionDecryption.CreateHash(model.Password);

            var result = await _authenticationRepository.RegisterAsync(model);

            if (result != null && result.StatusCode == 1)
            {
                return new JsonModel<AuthResponseModel>
                {
                    Data = result,
                    Message = result.Message,
                    StatusCode = 200 // Success
                };
            }

            return new JsonModel<AuthResponseModel>
            {
                Data = null,
                Message = result?.Message ?? "Registration failed",
                StatusCode = 400 // Bad Request
            };
        }

        public async Task<JsonModel<AuthResponseModel>> LoginAsync(LoginRequestModel model)
        {
            var user = await _authenticationRepository.GetUserByEmailAsync(model.Email);

            if (user == null)
            {
                return new JsonModel<AuthResponseModel>
                {
                    Data = null,
                    Message = "User not found",
                    StatusCode = 404
                };
            }

            // Validate password
            bool isValid = EncryptionDecryption.ValidatePassword(model.Password, user.PasswordHash);

            if (isValid)
            {
                // Clear password hash from response
                user.PasswordHash = null;
                user.Message = "Login successful";
                user.StatusCode = 1;

                return new JsonModel<AuthResponseModel>
                {
                    Data = user,
                    Message = "Login successful",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            return new JsonModel<AuthResponseModel>
            {
                Data = null,
                Message = "Invalid credentials",
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }
    }
}
