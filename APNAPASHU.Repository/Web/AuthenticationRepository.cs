using System.Data;
using APNAPASHU.DataContract.Models.Web.Authentication;
using APNAPASHU.RepositoryContract.Web;
using Dapper;
using Microsoft.Extensions.Configuration;

namespace APNAPASHU.Repository.Web
{
    public class AuthenticationRepository : BaseRepository, IAuthenticationRepository
    {
        public AuthenticationRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task<AuthResponseModel> RegisterAsync(RegisterRequestModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", model.RoleId);
            parameters.Add("@FirstName", model.FirstName);
            parameters.Add("@MiddleName", model.MiddleName);
            parameters.Add("@LastName", model.LastName);
            parameters.Add("@Email", model.Email);
            parameters.Add("@Phone", model.PhoneNumber);
            parameters.Add("@GenderCode", model.GenderCode);
            parameters.Add("@PasswordHash", model.Password); // Already hashed in Service
            parameters.Add("@Address", model.Address);
            parameters.Add("@DOB", model.DOB);
            parameters.Add("@PinCode", model.PinCode);
            parameters.Add("@City", model.City);
            parameters.Add("@StateId", model.StateId);
            parameters.Add("@CountryId", model.CountryId);
            parameters.Add("@IsTermsAccepted", model.IsTermsAccepted);

            // The SP returns Status and Message. We'll map them to AuthResponseModel
            return await GetFirstOrDefaultAsync<AuthResponseModel>("usp_RegisterUser", parameters, CommandType.StoredProcedure);
        }

        public async Task<AuthResponseModel> GetUserByEmailAsync(string email)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);

            return await GetFirstOrDefaultAsync<AuthResponseModel>("usp_UserLogin", parameters, CommandType.StoredProcedure);
        }
    }
}
