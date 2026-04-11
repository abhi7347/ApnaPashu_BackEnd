using System.Data;
using APNAPASHU.DataContract.Models;
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

        public async Task<SqlResponseModel> RegisterAsync(RegisterRequestModel model)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@RoleId", model.RoleId);
            parameters.Add("@FirstName", model.FirstName);
            parameters.Add("@LastName", model.LastName);
            parameters.Add("@Email", model.Email);
            parameters.Add("@Phone", model.PhoneNumber);
            parameters.Add("@PasswordHash", model.Password); // Already hashed in Service
            parameters.Add("@IsTermsAccepted", model.IsTermsAccepted);

            // Using AddAsync<T> from BaseRepository for Registration
            return await AddAsync<SqlResponseModel>("usp_RegisterUser", parameters, CommandType.StoredProcedure);
        }

        public async Task<LoginResponseModel> LoginUserAsync(string email, string password)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);
            parameters.Add("@Password", password);

            // Using GetQuerySingleOrDefaultAsync<T> from BaseRepository for Login
            return await GetQuerySingleOrDefaultAsync<LoginResponseModel>("usp_UserLogin", parameters, CommandType.StoredProcedure);
        }
    }
}
