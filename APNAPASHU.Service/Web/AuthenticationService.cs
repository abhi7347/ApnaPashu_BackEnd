using APNAPASHU.Common;
using APNAPASHU.DataContract.Models;
using APNAPASHU.DataContract.Models.Web.Authentication;
using APNAPASHU.RepositoryContract.Web;
using APNAPASHU.ServiceContract.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace APNAPASHU.Service.Web
{
    public class AuthenticationService : BaseService, IAuthenticationService
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IWebHostEnvironment _environment;

        public AuthenticationService(
            IAuthenticationRepository authenticationRepository,
            IHttpContextAccessor accessor,
            IConfiguration configuration,
            IWebHostEnvironment environment) : base(accessor, configuration)
        {
            _authenticationRepository = authenticationRepository;
            _environment = environment;
        }

        public async Task<JsonModel<SqlResponseModel>> RegisterAsync(RegisterRequestModel model)
        {
            model.Password = EncryptionDecryption.CreateHash(model.Password);

            var result = await _authenticationRepository.RegisterAsync(model);

            if (result == null)
            {
                return new JsonModel<SqlResponseModel>
                {
                    Data = null,
                    Message = "Something went wrong",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            // ? If SP failed
            if (result.StatusCode != "SUCCESS")
            {
                return new JsonModel<SqlResponseModel>
                {
                    Data = result,
                    Message = result.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            // ? Success ? Send Welcome Email
            try
            {
                string htmlTemplatePath = Path.Combine(_environment.WebRootPath, "EmailTemplate/RegistrationTemplate.html");
                if (File.Exists(htmlTemplatePath))
                {
                    string emailString = await System.IO.File.ReadAllTextAsync(htmlTemplatePath);

                    emailString = emailString.Replace("#Name", model.FirstName + " " + model.LastName);
                    emailString = emailString.Replace("#Email", model.Email);
                    emailString = emailString.Replace("#CopyrightYear", DateTime.Now.Year.ToString());

                    EmailModel emailModel = new EmailModel()
                    {
                        Body = emailString,
                        To = model.Email,
                        Subject = "Welcome to ApnaPashu - Registration Successful"
                    };

                    bool emailSent = await SendEmailAsync(emailModel);

                    if (emailSent)
                    {
                        return new JsonModel<SqlResponseModel>
                        {
                            Data = result,
                            Message = result.Message,
                            StatusCode = (int)HttpStatusCode.OK
                        };
                    }
                }

                return new JsonModel<SqlResponseModel>
                {
                    Data = result,
                    Message = result.Message,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new JsonModel<SqlResponseModel>
                {
                    Data = result,
                    Message = $"{result.Message} (email error: {ex.Message})",
                    StatusCode = (int)HttpStatusCode.OK
                };
            }
        }

        public async Task<JsonModel<LoginResponseModel>> LoginUserAsync(string email, string password)
        {

            var user = await _authenticationRepository.LoginUserAsync(email, password);

            if (user == null)
            {
                return new JsonModel<LoginResponseModel>
                {
                    Data = null,
                    Message = "Service error: Login failed",
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
            }

            // Check if the SP returned Success
            if (user.StatusCode == "SUCCESS")
            {
                // Generate JWT Token
                user.Token = GenerateJwtToken(user);
                
                // Clear password hash from response for security
                user.PasswordHash = string.Empty;

                return new JsonModel<LoginResponseModel>
                {
                    Data = user,
                    Message = user.Message,
                    StatusCode = (int)HttpStatusCode.OK
                };
            }

            // Return SP's error message (e.g., "User not found" or "Invalid credentials")
            return new JsonModel<LoginResponseModel>
            {
                Data = null,
                Message = user.Message,
                StatusCode = (int)HttpStatusCode.Unauthorized
            };
        }

        private string GenerateJwtToken(LoginResponseModel user)
        {
            var jwtSettings = Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "1440");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(expiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
