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
            // Using GetQuerySingleOrDefaultAsync<T> from BaseRepository for Login
            return await GetQuerySingleOrDefaultAsync<LoginResponseModel>("usp_UserLogin", parameters, CommandType.StoredProcedure);
        }

        public async Task<SqlResponseModel> ForgotPasswordAsync(string email, string token)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);
            parameters.Add("@ResetToken", token);

            return await GetQuerySingleOrDefaultAsync<SqlResponseModel>("usp_ForgotPassword", parameters, CommandType.StoredProcedure);
        }

        public async Task<SqlResponseModel> ResetPasswordAsync(string email, string token, string newPasswordHash)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);
            parameters.Add("@ResetToken", token);
            parameters.Add("@NewPasswordHash", newPasswordHash);

            return await GetQuerySingleOrDefaultAsync<SqlResponseModel>("usp_ResetPassword", parameters, CommandType.StoredProcedure);
        }
        public async Task<LoginResponseModel> GetUserByIdAsync(int userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            return await GetQuerySingleOrDefaultAsync<LoginResponseModel>("usp_GetUserById", parameters, CommandType.StoredProcedure);
        }

        public async Task<SqlResponseModel> UpdateProfileAsync(UpdateProfileRequestModel model, int userId, string? passwordHash, string? profileImage)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@FirstName", model.FirstName);
            parameters.Add("@LastName", model.LastName);
            parameters.Add("@Email", model.Email);
            parameters.Add("@Phone", model.Phone);
            parameters.Add("@Address", model.Address);
            parameters.Add("@PasswordHash", passwordHash);
            parameters.Add("@ProfileImage", profileImage);

            return await GetQuerySingleOrDefaultAsync<SqlResponseModel>("usp_UpdateUserProfile", parameters, CommandType.StoredProcedure);
        }

        public async Task<SqlResponseModel> ChangePasswordAsync(int userId, string newPasswordHash)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@NewPasswordHash", newPasswordHash);

            return await GetQuerySingleOrDefaultAsync<SqlResponseModel>("usp_ChangeUserPassword", parameters, CommandType.StoredProcedure);
        }
    }
}
